namespace Maki.Tests
{
    using Client;
    using Xunit;

    public class MakiWebsocketClientTests
    {
        [Fact(DisplayName = "Can convert HTTP URLs to websocket URLs.")]
        public void WebsocketUriReplacementWorks()
        {
            Assert.Equal("ws://127.0.0.1/", MakiHttpClient
                .ConstructWebsocketUriFromHttpUri(
                    "http://127.0.0.1/",
                    null).ToString());
            Assert.Equal("ws://127.0.0.1/stream", MakiHttpClient
                .ConstructWebsocketUriFromHttpUri(
                    "http://127.0.0.1/",
                    "stream").ToString());
        }
        
        [Fact(DisplayName = "Can convert HTTPS URLs to websocket URLs.")]
        public void WebsocketUriReplacementWorkForSSL()
        {
            Assert.Equal("wss://127.0.0.1/", MakiHttpClient
                .ConstructWebsocketUriFromHttpUri(
                    "https://127.0.0.1/",
                    null).ToString());
            Assert.Equal("wss://127.0.0.1/stream", MakiHttpClient
                .ConstructWebsocketUriFromHttpUri(
                    "https://127.0.0.1/",
                    "stream").ToString());
        }
    }
}