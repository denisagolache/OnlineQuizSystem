﻿
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultHeader
    {
        public Guid QuizId { get; }
        public Guid UserId { get; }
        public DateTime SubmittedAt { get; set; }
        public int CompletionTime { get; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }

        public QuizResultHeader(Guid quizID, Guid userID, int completionTime)
        {
            QuizId = quizID;
            UserId = userID;
            CompletionTime = completionTime;
            SubmittedAt = DateTime.Now;
        }
        public void UpdateUponAnswerReview(QuizResultBody updatedBody)
        {
            if (updatedBody.QuizId != QuizId || updatedBody.UserId != UserId)
            {
                throw new ArgumentException("The quiz result body does not match the quiz result header");
            }

            float newScore = 0;
            foreach (var question in updatedBody.QuestionResults)
            {
                newScore += question.Score;
            }

            Score = newScore;

            bool hasPendingQuestions = updatedBody.QuestionResults
                .Any(q => q is ReviewNeededQuestionResult && 
                    (((ReviewNeededQuestionResult)q).ReviewNeededResult == AnswerResult.Pending));

            ReviewPending = hasPendingQuestions;
         
            //update in DB
        
        }
    }
}
