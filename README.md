# Maki

### What is Maki?

#### Maki is the software that runs inside a Docker container to interact with the UCI chess engine. It exposes a few APIs over an HTTP connection, in addition to a websocket interface for subscribing to a realtime stream of the engine output. At the time of writing, the supported APIs are:

* GetEngineId
* ListSupportedOption
* SetEngineDebugFlag
* StartEvaluation
* SubscribeToEngineStream
* UnsubscribeToEngineStream

The most up-to-date list can most likely be obtained by looking at [IMakiClient source](blob/master/packages/projects/Maki.Client/Maki.Client/IMakiClient.cs).