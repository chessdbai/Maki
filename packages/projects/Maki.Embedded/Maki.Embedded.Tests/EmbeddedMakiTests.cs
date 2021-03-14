namespace Maki.Embedded.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class EmbeddedMakiTests
    {
        [Fact]
        public async Task CreatesEmbeddedMakiClientAsync()
        {
            var embeddedMakiInstances = new List<EmbeddedMaki>();
            var startupTasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                var embedded1 = EmbeddedMakiFactory.Create();
                startupTasks.Add(embedded1.StartAsync());
                embeddedMakiInstances.Add(embedded1);
            }

            await Task.WhenAll(startupTasks);

            foreach (var em in embeddedMakiInstances)
            {
                await using var client = em.CreateClient();
                var health = await client.HealthCheckAsync();
                Assert.Equal("svc-up", health);
                await em.StopAsync();
                em.Dispose();
            }
        }
    }
}