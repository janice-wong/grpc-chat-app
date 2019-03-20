using System.Collections.Generic;
namespace GreeterServer
{
  public static class ChatValidator
  {
    public static int CharLimit = 100;

    // Message must be between 0 and 100 characters
    public static bool ValidateMessageCharLimit(string message)
    {
      return message.Length <= CharLimit && message.Length > 0;
    }

    // Recipient cannot be the sender and must be in the list of existing users
    public static bool ValidateRecipient(List<string> userIds, string senderId, string recipientId)
    {
      return userIds.IndexOf(recipientId) > -1 && senderId != recipientId;
    }
  }
}
