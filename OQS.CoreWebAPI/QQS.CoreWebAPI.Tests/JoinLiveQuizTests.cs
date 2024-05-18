    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using OQS.CoreWebAPI.Entities;
    using OQS.CoreWebAPI.Features.WebSocket;
    using OQS.CoreWebAPI.Tests.SetUp;
    using Xunit;

    namespace QQS.CoreWebAPI.Tests;

    public class JoinLiveQuizTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task JoinRoomTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Seeder>());
            var hubContext = server.Host.Services.GetService<IHubContext<QuizRoomHub>>();
            var client = new HubConnectionBuilder()
                .WithUrl("http://localhost/quizroomhub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => server.CreateHandler();
                })
                .Build();

            await client.StartAsync();

            // Act
            await client.InvokeAsync("JoinRoom", new UserConnection
            {
                UserId = new Guid("5b048913-5df0-429f-a42b-051904672e4d"),
                ConnectionId =  client.ConnectionId         
                /* set properties here */
            });

            // Assert
            var userConnection = await _context.UserConnections.FindAsync(client.ConnectionId);
            Assert.NotNull(userConnection); // Verifies that the UserConnection was saved in the database
            Assert.Equal("5b048913-5df0-429f-a42b-051904672e4d", userConnection.UserId); // Verifies that the UserId is correct
        }

    }
