﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class StartLiveQuiz
{
    public record StartQuizCommand(string connectionID) : IRequest<Result<string>>;
    
    public class StartLiveQuizValidator : AbstractValidator<StartQuizCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public StartLiveQuizValidator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            RuleFor(x => x.connectionID)
                .MustAsync(AdminConnectionIdMatches)
                .WithMessage("You are not authorized to start the quiz.");
        }
        private async Task<bool> AdminConnectionIdMatches(string connectionId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var liveQuiz = await context.LiveQuizzes.Include(q => q.Connections).Include(q=>q.CreatedBy)
                .FirstOrDefaultAsync(q => q.Connections.Any(c => c.ConnectionId == connectionId));
            if (liveQuiz == null)
            {
                return false;
            }
            var adminConnectionId = await liveQuiz.getAdminConnectionId();
            return connectionId == adminConnectionId;
        }
        
    }
    public sealed class Handler : IRequestHandler<StartQuizCommand, Result<string>>
    {
        private readonly ApplicationDbContext _context;
        private readonly StartLiveQuizValidator _validator;
        private readonly IHubContext<LiveQuizzesHub> _hubContext;
        private readonly ISender _sender;

        public Handler(ApplicationDbContext context, StartLiveQuizValidator validator, IHubContext<LiveQuizzesHub> hubContext, ISender sender)
        {
            _context = context;
            _validator = validator;
            _hubContext = hubContext;
            _sender = sender;
        }
 

        public async Task<Result<string>> Handle(StartQuizCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var error=new Error("StartQuiz.BadRequest", validationResult.ToString());
                await _hubContext.Clients.Client(request.connectionID).SendAsync("Error", error);
                return Result.Failure<string>(error);
            }
            var connectionResult = await _context.UserConnections
                .Include(q => q.LiveQuizz)
                .Include(q=>q.LiveQuizz.Connections)
                .Include(q=>q.LiveQuizz.Connections)
                .Include(q=>q.LiveQuizz.Quiz)
                .FirstOrDefaultAsync(q => q.ConnectionId == request.connectionID);

            var liveQuiz =  connectionResult.LiveQuizz;
          
            List<Task> tasks = [];
            var connections= await _context.UserConnections
                .Include(q=>q.User)
                .Include(q=>q.LiveQuizz)
                .Include(q=>q.LiveQuizz.Quiz)
                .Where(q=>q.LiveQuizz.Code==liveQuiz.Code).ToListAsync();
            foreach (var connection in connections)
            {
                if(connection.ConnectionId!=request.connectionID)
                 tasks.Add(NotifyClients(connection));
                else
                {
                    _context.UserConnections.Remove(connection);
                    _context.SaveChanges();
                }
            }
            await Task.WhenAll(tasks);
            _context.LiveQuizzes.Remove(liveQuiz);
            _context.SaveChanges();
            await _hubContext.Clients.Client(request.connectionID).SendAsync("QuizStartedAdmin", Result.Success<Guid>(liveQuiz.Quiz.Id));
            return Result.Success("Quiz Started Successfully");
        }
        private async Task NotifyClients(UserConnection connection)
        {
            var activeQuiz = new ActiveQuiz
            {
                Id = Guid.NewGuid(),
                Quiz = connection.LiveQuizz.Quiz,
                User = connection.User,
                StartedAt = DateTime.UtcNow
            };
            await _context.ActiveQuizzes.AddAsync(activeQuiz);
            _context.UserConnections.Remove(connection);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("QuizStarted", Result.Success<Guid>(activeQuiz.Id));
           
        }
    }
    
   
}