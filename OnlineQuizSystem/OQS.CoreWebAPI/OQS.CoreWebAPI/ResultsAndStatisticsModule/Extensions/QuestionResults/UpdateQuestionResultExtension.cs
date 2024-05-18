﻿using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class UpdateQuestionResultExtension
    {
        public static async Task<Result> UpdateQuestionResultAsync(this WebApplication application, Guid userId, Guid questionId, float score)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await UpdateQuestionResultAsync(dbContext, userId, questionId, score);
        }

        public static async Task<Result> UpdateQuestionResultAsync(ApplicationDbContext dbContext, Guid userId, Guid questionId, float score)
        {
            var uncastedQuestionResult = await FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId);
            if (uncastedQuestionResult is null)
            {
                return Result.Failure(Error.NullValue);
            }

            if (uncastedQuestionResult is not ReviewNeededQuestionResult)
            {
                return Result.Failure(Error.InvalidType);
            }

            var questionResult = uncastedQuestionResult as ReviewNeededQuestionResult;
            var questionFromDb = await dbContext
                .Questions
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (score < 0 || score > questionFromDb.AllocatedPoints)
            {
                return Result.Failure(Error.OutOfBoundsValue);
            }

            if (questionResult.ReviewNeededResult != AnswerResult.Pending)
            {
                return Result.Failure(Error.ConditionNotMet);
            }

            questionResult.Score = score;
            if (score == 0)
            {
                questionResult.ReviewNeededResult = AnswerResult.Wrong;
            }
            else
            {
                if (score == questionFromDb.AllocatedPoints)
                {
                    questionResult.ReviewNeededResult = AnswerResult.Correct;
                }
                else
                {
                    questionResult.ReviewNeededResult = AnswerResult.PartiallyCorrect;
                }
            }

            dbContext.QuestionResults.Update(questionResult);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
    }
}