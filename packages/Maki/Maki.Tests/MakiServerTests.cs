namespace Maki.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Maki.Model;
    using Xunit;
    using Xunit.Abstractions;

    public class MakiServerTests : AbstractMakiContainerTest
    {
        public MakiServerTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {

        }

        [Fact(DisplayName = "Eval with max time ends on time")]
        public async Task EvalWithMaxTimeWorks()
        {
            var startTime = DateTime.UtcNow;
            var evalReq = new StartEvaluationRequest()
            {
                Position = new StartingPosition()
                {
                    Fen = "4kbnr/8/8/8/8/8/8/1N1QK3 w - - 0 1",
                },
                CompletionCriteria = new CompletionCriteria()
                {
                    PonderTime = TimeSpan.FromMinutes(1)
                },
            };
            var setResponse = await Client.StartEvaluationAsync(evalReq);

            // Made in time
            var actualDuration = DateTime.UtcNow - startTime;
            Assert.True((actualDuration - evalReq.CompletionCriteria.PonderTime!.Value).TotalMilliseconds < 1000);

            // Move Correct
            var line = setResponse.Lines[1];
            Assert.StartsWith("Nc6", line.LineSan);
            Assert.StartsWith("d4c6", line.LineUci);
        }

        [Fact(DisplayName = "Health check returns svc-up")]
        public async Task HealthCheckWorks()
        {
            var setResponse = await Client.HealthCheckAsync();
            Assert.True("svc-up" == setResponse, "Health check returns svc-up");
        }

        [Fact(DisplayName = "Server allows listing UCI options.")]
        public async Task CanListOptions()
        {
            var listOptionsResponse = await Client.ListSupportedOptionsAsync();
            Assert.True(listOptionsResponse.Options.Count == 21);
        }

        [Fact(DisplayName = "Server allows setting debug flag.")]
        public async Task CanSetDebugFlag()
        {
            await Client.SetEngineDebugFlagAsync(new SetEngineDebugFlagRequest()
            {
                EnableDebugMode = true
            });
        }

        [Fact(DisplayName = "Server allows getting engine ID information.")]
        public async Task CanGetEngineIdInfo()
        {
            var engineIdResponse = await Client.GetEngineIdAsync();
            Assert.Equal("Stockfish 090720 64", engineIdResponse.Name);
            Assert.Equal("T. Romstad, M. Costalba, J. Kiiski, G. Linscott", engineIdResponse.Author);
        }
    }
}