using Chatter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreeterServer.RequestHandlers
{
  public class MessageRequestHandler
  {
    private readonly List<string> _users;
    private List<Message> _messages;

    public MessageRequestHandler(List<string> users, List<Message> messages)
    {
      _users = users;
      _messages = messages;
    }

		/// <summary>
		/// Return client acks if recipient received messages sent
		/// </summary>
    public Task<GetMessageStatusResponse> GetMessageStatus(GetMessageStatusRequest request)
    {
      var getMessageStatusResponse = new GetMessageStatusResponse { };

      foreach (var message in _messages)
      {
        var messageWasDeliveredToRecipient = message.SenderId == request.SenderId && message.DeliveredToRecipient;

        if (messageWasDeliveredToRecipient)
        {
          _messages.Remove(message);

          var clientAck = new MessageStatus
          {
            Content = message.Content,
            DeliveredTo = DeliveredTo.Recipient,
            RecipientId = message.RecipientId
          };

          getMessageStatusResponse.MessageStatuses.Add(clientAck);
          return Task.FromResult(getMessageStatusResponse);
        }
      }

      return Task.FromResult(getMessageStatusResponse);
    }

		/// <summary>
		/// Send message(s) by adding Message object(s) to message queue and return server ack(s)
		/// </summary>
    public Task<GetMessageStatusResponse> SendMessage(SendMessageRequest request)
    {
      request.ValidateSendMessageRequest(_users);

      var response = new GetMessageStatusResponse { };

      if (request.RecipientType == RecipientType.Single)
      {
        var message = new Message(request.SenderId, request.RecipientId, request.Content);
        var messageStatus = SendSingleMessage(message);
        response.MessageStatuses.Add(messageStatus);
        return Task.FromResult(response);
      }

      foreach (var user in _users)
      {
        if (request.SenderId != user)
        {
          var message = new Message(request.SenderId, user, request.Content);
          var messageStatus = SendSingleMessage(message);
          response.MessageStatuses.Add(messageStatus);
        }
      }

      return Task.FromResult(response);
    }

		/// <summary>
		/// Return first unread message and mark Message object as received
		/// </summary>
    public Task<GetMessageResponse> GetFirstUnreadMessage(GetMessageRequest request)
    {
      foreach (var msg in _messages)
      {
        if (msg.RecipientId == request.RecipientId && !msg.DeliveredToRecipient)
        {
          var receivedMessage = msg;
					msg.MarkAsReceived();

          return Task.FromResult(new GetMessageResponse { SenderId = msg.SenderId, Content = msg.Content });
        }
      }

      return Task.FromResult(new GetMessageResponse());
    }

		/// <summary>
		/// Add Message object to message queue and return server ack
		/// </summary>
    private MessageStatus SendSingleMessage(Message message)
    {
      _messages.Add(message);

      var messageStatus = new MessageStatus
      {
        Content = message.Content,
        DeliveredTo = DeliveredTo.Server,
        RecipientId = message.RecipientId
      };

      return messageStatus;
    }
  }
}
