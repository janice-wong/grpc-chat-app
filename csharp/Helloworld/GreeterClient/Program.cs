using System;
using System.Threading;
using Grpc.Core;
using Chatter;
using System.Runtime.InteropServices;

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
        var response = Console.ReadLine();

        int colonIndex;
        string recipient = null;
        string messageContent = null;

        try
        {
          colonIndex = response.IndexOf(": ");
          recipient = response.Substring(0, colonIndex);
          messageContent = response.Substring(colonIndex + 2);
        }
        catch (Exception)
        {
          Console.WriteLine("Invalid message");
        }

        var messageStatusList = new Google.Protobuf.Collections.RepeatedField<MessageStatus>();

        // How do I get the below not to run if the above try block fails? Currently if the above try block fails, the client crashes.

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