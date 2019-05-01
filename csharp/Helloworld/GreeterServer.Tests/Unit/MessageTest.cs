using Xunit;
using FluentAssertions;
namespace GreeterServer.Tests.Unit
{
  public class MessageTest
  {
    private readonly Message _messageDeliveredToServer;

    public MessageTest()
    {
      _messageDeliveredToServer = new Message("0", "1", "0 to 1: Delivered to the server");
    }

    [Fact]
    public void Given_A_Message_Is_Marked_As_Received_Return_True()
    {
      _messageDeliveredToServer.MarkAsReceived();
      _messageDeliveredToServer.DeliveredToRecipient.Should().BeTrue();
    }
  }
}
