# lagom-dotnet

This is a **work in progress** .NET core port of Lightbend's Lagom framework using Akka.NET.

## CI/CD

[![CircleCI](https://circleci.com/gh/nagytech/lagom-dotnet.svg?style=shield)](https://circleci.com/gh/nagytech/lagom-dotnet)

NuGet package: https://www.nuget.org/packages/wyvern.api

## Features

So far, the list of supported features are:

- REST API (with Swagger generation)
- Journal and Snapshot persistence (SQL Server)
- Read side implementation (SQL Server)
- Topics of event streams (via AMQP)
- WebSocket based event streams
- Support for custom message serialization in topics
- Roughly 40% unit test coverage

Additionally, the following features may be handy:

- Akka cluster visualizer

Things that have been skipped, not fully implemented, or may need rework:

- Entities can be partitioned, but Topics and Read Side do not support partitioning yet
- Partial support for header transformations

## Getting started with the example

This set of steps walks you through how to get the 'Hello, World!'
example up and running.

### Setup

- Install dotnetcore 2.1
- Install docker and docker-compose
- Update `docker-compose.yml` and `./docker/db/Dockerfile` with an appropriate password for SQL Server
- Run `docker-compose up` to start the SQL Server, ActiveMQ and API containers

### Usage

- Open http://localhost:5000/ and use the Swagger UI to make several POST / GET calls
- To see websockets in action, open a connection to ws://localhost:5000/ws/hello
  (sample code below)
- To view the event journal, connect to the 'db' docker container and browse the dbo.EventJournal table
- To see the event bus, browse to http://localhost:8161/admin/queues.jsp using `admin:admin` and click on Queues and/or Topics to view what's been published

#### Websocket example

To be used for the example above.

```
var ws = new WebSocket("ws://localhost:5000/ws/hello");
ws.onopen = function() {
  console.log("connection open");
};
ws.onmessage = function (evt) {
  var received_msg = evt.data;
  console.log(evt.data);
};
ws.onclose = function() {
  console.log("Connection closed");
};
```
