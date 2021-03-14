namespace Maki.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Maki.Embedded;
    using Client;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Xunit;
    using Xunit.Abstractions;

    public abstract class AbstractMakiContainerTest : IAsyncLifetime
    {
        private IDockerClient docker;
        private string containerId;
        private readonly ITestOutputHelper logger;
        private readonly EmbeddedMaki maki;

        protected IMakiClient Client { get; private set; }
        protected ITestOutputHelper Logger => logger;

        public AbstractMakiContainerTest(ITestOutputHelper logger)
        {
            this.logger = logger;
            this.maki = EmbeddedMakiFactory.Create();
        }

        public async Task InitializeAsync()
        {
            //await this.maki.StartAsync();

            Client = new MakiHttpClient(new MakiHttpClientOptions()
            {
                HttpUri = new Uri("http://3.128.197.130:8080/"),
            });
        }

        /*
        private async Task<string> StartContainerAsync()
        {
            var imageId = await GetLatestMakiImageTagAsync();
            await Task.Delay(1000);
            var createResponse = await docker.Containers.CreateContainerAsync(new CreateContainerParameters()
                {
                    Image = imageId,
                    ExposedPorts = new Dictionary<string, EmptyStruct>
                    {
                        {
                            "5000", default(EmptyStruct)
                        }
                    },
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            {"5000", new List<PortBinding> {new PortBinding {HostPort = "5000"}}}
                        },
                        PublishAllPorts = true
                    }
                },
                CancellationToken.None);
            logger.WriteLine($"Created container with ID '{createResponse.ID}'.");
            var cid = createResponse.ID;
            await docker.Containers.StartContainerAsync(cid, new ContainerStartParameters(), CancellationToken.None);
            logger.WriteLine($"Started container with ID '{createResponse.ID}'.");
            await Task.Delay(3000);
            return cid;
        }

        private async Task<string> GetLatestMakiImageTagAsync()
        {
            var images = await docker.Images.ListImagesAsync(new ImagesListParameters()
            {
                MatchName = "maki"
            });
            var mostRecent = (from img in images orderby img.Created select img).First();
            string imageId = mostRecent.ID.Split(":")[1].Substring(0, 12);
            logger.WriteLine($"Using image ID '{mostRecent.ID}'.");
            return imageId;
        }

        */

        public async Task DisposeAsync()
        {
            await ((MakiHttpClient) Client).DisposeAsync();
            //this.maki.Dispose();
        }

    }
}