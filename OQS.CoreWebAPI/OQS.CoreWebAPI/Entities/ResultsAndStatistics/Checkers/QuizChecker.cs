﻿using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.Checkers
{
    public abstract class QuizChecker
    {
        public static async Task<Result> CheckQuizAsync(QuizSubmission toBeChecked, ApplicationDbContext dbContext)
        {
            Quiz quizFromDb = await dbContext.Quizzes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == toBeChecked.QuizId);
            if (quizFromDb is null)
            {
                Console.WriteLine("Error: Quiz not found in database");
                return Result.Failure(Error.NullValue);
            }

            var questions = await dbContext.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == toBeChecked.QuizId)
                .ToListAsync();

            Guid newResultId = Guid.NewGuid();

            Result<List<QuestionResultBase>> questionsResults = await CheckAndStoreAllQuestionsAsync(newResultId, toBeChecked, questions, dbContext);
            if (questionsResults.IsFailure)
            {
                Console.WriteLine($"Error: {questionsResults.Error}");
                return Result.Failure(questionsResults.Error);
            }
            return await BuildAndStoreQuizResultHeaderAsync(newResultId, toBeChecked, questionsResults.Value, dbContext);
        }

        private static async Task<Result<List<QuestionResultBase>>> CheckAndStoreAllQuestionsAsync(Guid newResultId, QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb, ApplicationDbContext dbContext)
        {
            List<Guid> questionIds = questionsFromDb.Select(q => q.Id).ToList();
            bool qaPairNotBelongingToQuiz = toBeChecked.QuestionAnswerPairs
                .Any(qaPair => !questionIds.Contains(qaPair.QuestionId));

            if (qaPairNotBelongingToQuiz)
            {
                Console.WriteLine("Error: QuizSubmission contains answer to question not belonging to this quiz.");
                return Result.Failure<List<QuestionResultBase>>(
                    new Error("QuizChecker.StrayAnswer",
                    "QuizSubmission contains answer to question not belonging to this quiz."));
            }

            List<QuestionResultBase> questionsResults = [];
            foreach (var question in questionsFromDb)
            {
                QuestionAnswerPairBase qaPair = toBeChecked.QuestionAnswerPairs
                    .FirstOrDefault(qaPair => qaPair.QuestionId == question.Id);
                QuestionResultBase questionResult = QuestionChecker.CheckQuestion(toBeChecked.TakenBy, qaPair, question);
                questionResult.ResultId = newResultId;
                await StoreQuestionResultExtension.StoreQuestionResultAsync(dbContext, questionResult);
                questionsResults.Add(questionResult);
            }

            return questionsResults;
        }

        private static async Task<Result> BuildAndStoreQuizResultHeaderAsync(Guid newResultId, QuizSubmission toBeChecked,
            List<QuestionResultBase> questionsResults,
            ApplicationDbContext dbContext)
        {
            QuizResultHeader resultHeader = new(newResultId,
                toBeChecked.QuizId,
                toBeChecked.TakenBy);
            resultHeader.Score = 0;
            foreach (var questionResult in questionsResults)
            {
                resultHeader.Score += questionResult.Score;
                if (questionResult is ReviewNeededQuestionResult result &&
                    result.ReviewNeededResult == AnswerResult.Pending)
                    resultHeader.ReviewPending = true;
            }
            try
            {
                dbContext.QuizResultHeaders.Add(resultHeader);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught during result header building: {e.Message}");
                return Result.Failure(new Error("QuizChecker.QuizResultHeaderSaveError", e.Message));
            }
            return Result.Success();
        }
    }
}