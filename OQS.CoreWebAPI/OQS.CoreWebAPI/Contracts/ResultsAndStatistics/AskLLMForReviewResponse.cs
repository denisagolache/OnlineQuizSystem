﻿namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class AskLLMForReviewResponse(string review, float grade)
    {
        public string Review { get; set; } = review;
        public float Grade { get; set; } = grade;
    }
}
