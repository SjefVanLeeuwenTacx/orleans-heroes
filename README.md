# Orleans Heroes Sample App
*This example project is still work in progress...*

## Technologies

### Server
- .NET 6.0
- Microsoft Orleans 3.6.2
- SignalR
- [Microsoft Orleans SignalR Backplane by Sketch7 3.x](https://github.com/sketch7/SignalR.Orleans)
- GraphQL & GraphiQL

### Client
- Angular 10.x
- TypeScript
- [Sketch7 - Signalr Client 3.x](https://github.com/sketch7/signalr-client)

*Based on https://github.com/sketch7/angular-skeleton-app*


## Helpful links
- [Orleans 2: Silo and Client](Https://dotnet.github.io/orleans/Documentation/Getting-Started-With-Orleans/Running-the-Application.html)


# Development

## Setup Redis locally

```bash
docker run -p 6379:6379 --name redis5-under-the-hood -d redis:5.0.6
```