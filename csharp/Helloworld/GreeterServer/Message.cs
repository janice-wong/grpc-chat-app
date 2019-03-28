using System;
namespace GreeterServer
{
  public class Message
  {
    public Guid Id { get; set; }
    public string SenderId { get; set; }
    public string RecipientId { get; set; }
    public string Content { get; set; }
    public bool DeliveredToRecipient { get; set; }

    public Message(string senderId, string recipientId, string content)
    {
      Id = Guid.NewGuid();
      SenderId = senderId;
      RecipientId = recipientId;
      Content = content;
      DeliveredToRecipient = false;
    }
  }
}

