using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedExpiredActiveQuizExtension
{
    public static void SeedExpiredActiveQuizzes(this ApplicationDBContext context)
    {
        if (context.ActiveQuizzes.Any(quiz=>quiz.Id == Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972451")))
        {
            return;
        }
        var quiz = context.Quizzes.First(q => q.Id == Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"));
        var user = context.Users.First(u => u.Id == Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d").ToString());
        var ActiveQuiz = new ActiveQuiz()
        {
            Id = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972451"),
            Quiz = quiz,
            User = user,
            StartedAt = DateTime.UtcNow.AddMinutes(-1*(quiz.TimeLimitMinutes+1)),
            };
        
        
        
        context.ActiveQuizzes.Add(ActiveQuiz);
        context.SaveChanges();
    }
}