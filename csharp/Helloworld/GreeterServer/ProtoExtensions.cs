using System.Collections.Generic;
using Grpc.Core;
using Chatter;
using System.Text.RegularExpressions;

namespace GreeterServer
{
  public static class ProtoExtensions
  {
		private static readonly int _charLimit = 100;

		public static bool ValidateSendMessageRequest(this SendMessageRequest request, List<string> userIds)
		{
			var senderIsSpecified = !string.IsNullOrWhiteSpace(request.SenderId);
			if (!senderIsSpecified)
			{
        throw new RpcException(new Status(StatusCode.InvalidArgument, "You did not specify a sender. Please try again."));
			}

			var rxValidRecipientFormat = new Regex(@"(All|[\d]+)");
			var recipientHasValidRecipientFormat = rxValidRecipientFormat.IsMatch(request.RecipientId);

			var recipientIsValid = recipientHasValidRecipientFormat && (request.RecipientId == "All" && userIds.Count > 1) || (userIds.IndexOf(request.RecipientId) > -1 && request.SenderId != request.RecipientId);
      if (!recipientIsValid)
      {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "You specified an invalid recipient. Please try again."));
      }

			var messageIsWithinCharLimit = request.Content.Length <= _charLimit || string.IsNullOrWhiteSpace(request.Content);
      if (!messageIsWithinCharLimit)
      {
        throw new RpcException(new Status(StatusCode.InvalidArgument, $"Message must be 1-{_charLimit} characters. Please try again."));
      }

			return senderIsSpecified && recipientHasValidRecipientFormat && recipientIsValid && messageIsWithinCharLimit;
    }
  }
}
