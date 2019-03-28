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

		public Task<GetMessageStatusResponse> SendMessage(SendMessageRequest request)
		{
			request.ValidateRequest(_users);

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
