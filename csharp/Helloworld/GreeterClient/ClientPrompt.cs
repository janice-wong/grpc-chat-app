using System;
using System.Text.RegularExpressions;
using Chatter;
using Google.Protobuf.Collections;

namespace GreeterClient
{
  public class ClientPrompt
  {
    public void Start(string userId)
    {
      Console.WriteLine("Your user ID is: " + userId);
      Console.WriteLine();
      Console.WriteLine("To send a message, use either of the following (without the chevrons): " + "\n" +
        "<User Id>: <Message>" + "\n" +
        "All: <Message>" + "\n");
    }

    public void PrintAck(MessageStatus messageStatus)
    {
      if (messageStatus.DeliveredTo == DeliveredTo.Server)
      {
        Console.WriteLine("Your message to " + messageStatus.RecipientId + " was sent: " + messageStatus.Content);
      }
      else if (messageStatus.DeliveredTo == DeliveredTo.Recipient)
      {
        Console.WriteLine("Your message to " + messageStatus.RecipientId + " was read: " + messageStatus.Content);
      }
    }

    public void PrintReceivedMessage(GetMessageResponse messageResponse)
    {
      if (!string.IsNullOrEmpty(messageResponse.Content))
      {
        Console.WriteLine(messageResponse.SenderId + ": " + messageResponse.Content);
      }
    }

    public bool ValidateUserInput(string userInput)
    {
      var rxValidUserInput = new Regex(@"(All|[\d]+): (?s).*");
      return rxValidUserInput.IsMatch(userInput);
    }

    public SendMessageRequest CreateSendMessageRequest(string userId, string userInput)
    {
      var colonIndex = userInput.IndexOf(": ");
      var recipientInput = userInput.Substring(0, colonIndex);
      var recipientType = recipientInput == "All" ? RecipientType.All : RecipientType.Single;
      var messageContent = userInput.Substring(colonIndex + 2);
      var messageStatusList = new RepeatedField<MessageStatus>();

      return new SendMessageRequest
      {
        SenderId = userId,
        RecipientType = recipientType,
        RecipientId = recipientInput,
        Content = messageContent
      };
    }
  }
}
