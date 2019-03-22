using System;
using System.Threading;
using Grpc.Core;
using Chatter;
using System.Text.RegularExpressions;

namespace GreeterClient
{
  class Program
  {
    static void ListenForMessages(Greeter.GreeterClient client, ClientPrompt clientPrompt, string userId)
    {
      while (true)
      {
        clientPrompt.PrintReceivedMessage(client.GetFirstUnreadMessage(new GetMessageRequest { RecipientId = userId }));

        var messageStatusList = client.GetMessageStatus(new GetMessageStatusRequest { SenderId = userId }).MessageStatuses;
        if (messageStatusList.Count > 0)
        {
          foreach (var messageStatus in messageStatusList)
          {
            clientPrompt.PrintAck(messageStatus);
          }
        }

        Thread.Sleep(1500);
      }
    }

    public static void Main(string[] args)
    {
      Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

      var client = new Greeter.GreeterClient(channel);
      var clientPrompt = new ClientPrompt();
      var userId = client.GetUserId(new GetUserIdRequest { }).UserId;
      var chatValid = true;

      Thread thread = new Thread(() => ListenForMessages(client, clientPrompt, userId));
      thread.Start();

      clientPrompt.Start(userId);

      while (chatValid)
      {
        var userInput = Console.ReadLine();
        var rxValidUserInput = new Regex(@"(All|[\d]+): (?s).*");
				if (!rxValidUserInput.IsMatch(userInput))
				{
					Console.WriteLine("Invalid message");
					continue;
				}

        var colonIndex = userInput.IndexOf(": ");
        var recipient = userInput.Substring(0, colonIndex);
        var messageContent = userInput.Substring(colonIndex + 2);
     
        var messageStatusList = new Google.Protobuf.Collections.RepeatedField<MessageStatus>();

        try
        {
          if (recipient == "All")
          {
            messageStatusList = client.SendMessage(new SendMessageRequest
            {
              SenderId = userId,
              RecipientType = RecipientType.All,
              Content = messageContent
            }).MessageStatuses;
          }
          else
          {
            // Send a message to a single user
            messageStatusList = client.SendMessage(new SendMessageRequest
            {
              SenderId = userId,
              RecipientType = RecipientType.Single,
              RecipientId = recipient,
              Content = messageContent
            }).MessageStatuses;
          }
        }
        catch (RpcException e)
        {
          Console.WriteLine(e.Status.Detail);
        }

        foreach (var messageStatus in messageStatusList)
        {
          clientPrompt.PrintAck(messageStatus);
        }
      }

      channel.ShutdownAsync().Wait();
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
  }
}