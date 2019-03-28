using System.Collections.Generic;
using GreeterServer.RequestHandlers;
using Xunit;
using FluentAssertions;
using Chatter;
using Google.Protobuf.Collections;

namespace GreeterServer.Tests.Unit.RequestHandlers
{
  public class GetMessageStatusTest
  {
    private readonly MessageRequestHandler _subjectUnderTest;
    private readonly List<string> _users;
    private readonly List<Message> _messages;
    private readonly Message _messageDeliveredToRecipient;
    private readonly Message _messageDeliveredToServer;

    public GetMessageStatusTest()
    {
      _users = new List<string> { "0", "1", "2" };
      _messages = new List<Message>();
      _messageDeliveredToRecipient = new Message("0", "1", "0 to 1: Delivered to 1") { DeliveredToRecipient = true };
      _messageDeliveredToServer = new Message("0", "1", "0 to 1: Delivered to the server");

      _messages.Add(_messageDeliveredToRecipient);
      _messages.Add(_messageDeliveredToServer);

      _subjectUnderTest = new MessageRequestHandler(_users, _messages);
    }

    [Fact]
    public void Given_One_Message_Was_Delivered_To_A_Recipient_Return_One_Client_Ack()
    {
      var getMessageStatusResponse = _subjectUnderTest.GetMessageStatus(new GetMessageStatusRequest { SenderId = "0" });
      var messageStatuses = getMessageStatusResponse.Result.MessageStatuses;

      var expectedClientAck = new MessageStatus
      {
        Content = _messageDeliveredToRecipient.Content,
        DeliveredTo = DeliveredTo.Recipient,
        RecipientId = _messageDeliveredToRecipient.RecipientId
      };
      var expectedMessageStatuses = new RepeatedField<MessageStatus> { expectedClientAck };
      var expectedMessages = new List<Message> { new Message("0", "1", "0 to 1: Delivered to the server") { Id = _messageDeliveredToServer.Id } };

      messageStatuses.Should().Equal(expectedMessageStatuses);
      _messages.Should().BeEquivalentTo(expectedMessages);
    }

    [Fact]
    public void Given_No_Messages_Were_Delivered_To_A_Recipient_Return_No_Client_Acks()
    {
      var anotherMessageDeliveredToRecipient = new Message("2", "0", "2 to 1: Delivered to server");
      _messages.Add(anotherMessageDeliveredToRecipient);

      var getMessageStatusResponse = _subjectUnderTest.GetMessageStatus(new GetMessageStatusRequest { SenderId = "2" });
      var messageStatuses = getMessageStatusResponse.Result.MessageStatuses;

      // TODO: Determine if I need to copy this explicitly - I think so because _messages is a reference type.
      var expectedMessages = new List<Message>
      {
        new Message("0", "1", "0 to 1: Delivered to 1") { DeliveredToRecipient = true, Id = _messageDeliveredToRecipient.Id },
        new Message("0", "1", "0 to 1: Delivered to the server") { Id = _messageDeliveredToServer.Id },
        new Message("2", "0", "2 to 1: Delivered to server") { Id = anotherMessageDeliveredToRecipient.Id }
      };
      var expectedMessageStatuses = new RepeatedField<MessageStatus> { };

      messageStatuses.Should().Equal(expectedMessageStatuses);
      _messages.Should().BeEquivalentTo(expectedMessages);
    }
  }
}
