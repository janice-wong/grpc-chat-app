using System.Collections.Generic;
using Grpc.Core;
using Chatter;

namespace GreeterServer
{
	public static class ProtoExtensions
	{
		public static void ValidateRequest(this SendMessageRequest request, List<string> users)
		{
			var isValidRecipient = ChatValidator.ValidateRecipient(users, request.SenderId, request.RecipientId);
			if (!isValidRecipient)
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "You specified an invalid recipient."));
			}

			var messageIsWithinCharLimit = ChatValidator.ValidateMessageCharLimit(request.Content);
			if (!messageIsWithinCharLimit)
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Message must be less than " + ChatValidator.CharLimit + " characters."));
			}

		}
	}
}
