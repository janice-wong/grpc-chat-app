## grpc-chat-app
This chat app supports the following functions:

- Allows users to chat with one another
- Allows users to broadcast a message to all existing users
- Returns send- and receive-acknowledgements to users for the messages they send


### Running the app
To spin up the server, run:

```cd csharp/Helloworld/GreeterServer && dotnet run -f netcoreapp2.1```

To spin up an instance of a client, open another tab and `cd` into `csharp/Helloworld/GreeterClient` and run `dotnet run -f netcoreapp2.1`. You can spin up as many instances of `GreeterClient` as you would like.


### Unsupported features (for now)
- Ensuring messages sent are received in order
- Storing any data (users, messages, acknowledgements) in a database
- Having safeguards against failed client-server connections
- Returning practical message fields (i.e. timestamps) from server to client
