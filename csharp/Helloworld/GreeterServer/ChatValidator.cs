using System.Collections.Generic;

namespace GreeterServer
{
	public static class ChatValidator
	{
		public static int CharLimit = 100;

		// Message must be less than 100 characters
		public static bool ValidateMessageCharLimit(string message)
		{
			return message.Length <= CharLimit;
		}

		// Recipient cannot be the sender and must be in the list of existing users
		public static bool ValidateRecipient(List<string> userIds, string senderId, string recipientId)
		{
			return recipientId == "All" && userIds.Count > 1 || userIds.IndexOf(recipientId) > -1 && senderId != recipientId;
		}
	}
}