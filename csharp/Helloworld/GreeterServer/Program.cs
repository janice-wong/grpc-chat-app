using System;
using Grpc.Core;
using Chatter;
using GreeterServer.ServiceImplementation;

namespace GreeterServer
{
<<<<<<< HEAD
  class Program
  {
    const int Port = 50051;

    public static void Main(string[] args)
    {
      Server server = new Server
      {
        Services = { Greeter.BindService(new ChatterImpl()) },
        Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
      };
      server.Start();

      Console.WriteLine("Greeter server listening on port " + Port);
      Console.WriteLine("Press any key to stop the server...");
      Console.ReadKey();

      server.ShutdownAsync().Wait();
    }
  }
=======
	class GreeterImpl : Greeter.GreeterBase
	{
		private List<string> _users;
		private readonly List<Message> _messages;

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

		public GreeterImpl()
		{
			_users = new List<string>();
			_messages = new List<Message>();
		}

		public override Task<GetUserIdResponse> GetUserId(GetUserIdRequest request, ServerCallContext context)
		{
			// Return same user ID if user calls this method more than once
			if (_users.IndexOf(request.UserId) > -1)
			{
				return Task.FromResult(new GetUserIdResponse { UserId = request.UserId });
			}

			_users.Add(Convert.ToString(_users.Count));
			return Task.FromResult(new GetUserIdResponse { UserId = _users[_users.Count - 1] });

		}

		public override Task<GetMessageStatusResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
		{

			if (!ChatValidator.ValidateRecipient(_users, request.SenderId, request.RecipientId))
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "You specified an invalid recipient."));
			}

			if (!ChatValidator.ValidateMessageCharLimit(request.Content))
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Message must be less than " + ChatValidator.CharLimit + " characters."));
			}

			var response = new GetMessageStatusResponse { };

			// TODO: Pull the below logic into separate classes to more easily write unit tests

			if (request.RecipientType == RecipientType.Single)
			{
				var message = new Message(request.SenderId, request.RecipientId, request.Content);
				response.MessageStatuses.Add(SendSingleMessage(message));
				return Task.FromResult(response);
			}

			foreach (var user in _users)
			{
				if (request.SenderId == user)
				{
					continue;
				}

				var message = new Message(request.SenderId, user, request.Content);
				response.MessageStatuses.Add(SendSingleMessage(message));
			}

			return Task.FromResult(response);
		}

		public override Task<GetMessageResponse> GetFirstUnreadMessage(GetMessageRequest request, ServerCallContext context)
		{
			foreach (var msg in _messages)
			{
				if (msg.RecipientId == request.RecipientId && !msg.DeliveredToRecipient)
				{
					var receivedMessage = msg;
					msg.DeliveredToRecipient = true;

					return Task.FromResult(new GetMessageResponse { SenderId = msg.SenderId, Content = msg.Content });
				}
			}

			return Task.FromResult(new GetMessageResponse());
		}

		public override Task<GetMessageStatusResponse> GetMessageStatus(GetMessageStatusRequest request, ServerCallContext context)
		{
			var response = new GetMessageStatusResponse { };

			foreach (var message in _messages)
			{
				if (message.SenderId == request.SenderId && message.DeliveredToRecipient)
				{
					_messages.Remove(message);

					var messageStatus = new MessageStatus
					{
						Content = message.Content,
						DeliveredTo = DeliveredTo.Recipient,
						RecipientId = message.RecipientId
					};

					response.MessageStatuses.Add(messageStatus);
					return Task.FromResult(response);
				}
			}

			return Task.FromResult(response);
		}

		class Program
		{
			const int Port = 50051;

			public static void Main(string[] args)
			{
				Server server = new Server
				{
					Services = { Greeter.BindService(new GreeterImpl()) },
					Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
				};
				server.Start();

				Console.WriteLine("Greeter server listening on port " + Port);
				Console.WriteLine("Press any key to stop the server...");
				Console.ReadKey();

				server.ShutdownAsync().Wait();
			}
		}
	}
>>>>>>> d1579226dabd290a3aeca50c8ef54ce1da948791
}