using System;
namespace GreeterServer
{
  public class Message
  {
    public Guid Id { get; private set; }
    public string SenderId { get; private set; }
    public string RecipientId { get; private set; }
    public string Content { get; private set; }
    public bool DeliveredToRecipient { get; private set; }

    /// <summary>
    /// Message specified the sender, recipient, content, and defaults DeliveredToRecipient to false
    /// </summary>
    public Message(string senderId, string recipientId, string content, Guid? id = null)
    {
      Id = id == null ? Guid.NewGuid() : id.Value;
      SenderId = senderId;
      RecipientId = recipientId;
      Content = content;
      DeliveredToRecipient = false;
    }

    /// <summary>
    /// Updates the message's DeliveredToRecipient property from false to true
    /// </summary>
    public bool MarkAsReceived()
    {
      DeliveredToRecipient = true;
      return DeliveredToRecipient;
    }
  }
}

