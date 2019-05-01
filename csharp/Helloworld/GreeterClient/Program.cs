using System;
using System.Threading;
using Grpc.Core;
using Chatter;
using Google.Protobuf.Collections;

namespace GreeterClient
{
  class Program
  {
    /// <summary>
    ///	Print messages received and any client or server acks
    /// </summary>
    static void ListenForMessages(Greeter.GreeterClient client, ClientPrompt clientPrompt, string userId)
    {
      while (true)
      {
        // Print received messages
        clientPrompt.PrintReceivedMessage(client.GetFirstUnreadMessage(new GetMessageRequest { RecipientId = userId }));

        // Print client acks
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

    public static void Main()
    {
      Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

      var client = new Greeter.GreeterClient(channel);
      var clientPrompt = new ClientPrompt();
      var userId = client.GetUserId(new GetUserIdRequest { }).UserId;
      var chatValid = true;

      var thread = new Thread(() => ListenForMessages(client, clientPrompt, userId));
      thread.Start();

      clientPrompt.Start(userId);

      while (chatValid)
      {
        var userInput = Console.ReadLine();
        var userInputIsInvalid = !clientPrompt.ValidateUserInput(userInput);

        if (userInputIsInvalid)
        {
          Console.WriteLine($"Please format your message correctly and try again.{Environment.NewLine}");
          continue;
        }

        var messageStatusList = new RepeatedField<MessageStatus>();
        var sendMessageRequest = clientPrompt.CreateSendMessageRequest(userId, userInput);

        // Send or broadcast a message
        try
        {
          messageStatusList = client.SendMessage(sendMessageRequest).MessageStatuses;
        }
        // Print errors if any
        catch (RpcException e)
        {
          if (e.StatusCode != StatusCode.InvalidArgument)
          {
            Console.WriteLine($"Something went wrong. Please contact your admin.{Environment.NewLine}");
          }
          Console.WriteLine($"{e.Status.Detail}{Environment.NewLine}");
        }

        // Print server acks
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