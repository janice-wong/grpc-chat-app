using System;
using System.Text.RegularExpressions;
using Chatter;

namespace GreeterClient
{
  public class ClientPrompt
  {
		/// <summary>
		/// Print start prompt
		/// </summary>
    public void Start(string userId)
    {
      Console.WriteLine($"Welcome to this gRPC chat app!{Environment.NewLine}{Environment.NewLine}You can send a message to one specific user (say with user ID 3) or broadcast to all users using the formats below:{Environment.NewLine}{Environment.NewLine}3: Hello {Environment.NewLine}All: Hi everyone{Environment.NewLine}--{Environment.NewLine}Your user ID: {userId}{Environment.NewLine}--{Environment.NewLine}");
    }

		/// <summary>
		/// Print client or server acks if they exist
		/// </summary>
    public void PrintAck(MessageStatus messageStatus)
    {
			if (messageStatus.DeliveredTo == DeliveredTo.Server)
			{
				Console.WriteLine($"Your message to {messageStatus.RecipientId} was sent: { messageStatus.Content}{Environment.NewLine}");
			}
			else if (messageStatus.DeliveredTo == DeliveredTo.Recipient)
			{
				Console.WriteLine($"Your message to {messageStatus.RecipientId} was read: {messageStatus.Content}{Environment.NewLine}");
      }
    }

		/// <summary>
		/// Print any messages received
		/// </summary>
    public void PrintReceivedMessage(GetMessageResponse messageResponse)
    {
      if (!string.IsNullOrEmpty(messageResponse.Content))
      {
        Console.WriteLine($"{messageResponse.SenderId}: {messageResponse.Content}{Environment.NewLine}");
      }
    }

		/// <summary>
		/// Create SendMessageRequest for gRPC method SendMessage
		/// </summary>
    public SendMessageRequest CreateSendMessageRequest(string userId, string userInput)
    {
			var colonIndex = userInput.IndexOf(": ");
			var recipientInput = userInput.Substring(0, colonIndex);
      var recipientType = recipientInput == "All" ? RecipientType.All : RecipientType.Single;
      var messageContent = userInput.Substring(colonIndex + 2);

      return new SendMessageRequest
      {
        SenderId = userId,
        RecipientType = recipientType,
        RecipientId = recipientInput,
        Content = messageContent
      };
    }

		/// <summary>
		/// Validate user input with Regex
		/// </summary>
    public bool ValidateUserInput(string userInput)
    {
      var rxValidUserInput = new Regex(@"(All|[\d]+): (?s).*");
      return rxValidUserInput.IsMatch(userInput);
    }
  }
}
