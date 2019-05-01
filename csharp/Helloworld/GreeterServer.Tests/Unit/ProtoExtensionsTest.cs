using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Chatter;

namespace GreeterServer.Tests.Unit
{
  public class ProtoExtensionsTest
  {
    private readonly List<string> _oneUser;
    private readonly List<string> _twoUsers;

    public ProtoExtensionsTest()
    {
      _oneUser = new List<string> { "0" };
      _twoUsers = new List<string> { "0", "1" };
    }

    [Fact]
    public void Given_A_Sender_Is_Specified_Return_True()
    {
      var sendMessageRequest = new SendMessageRequest
      {
        SenderId = "0",
        RecipientType = RecipientType.Single,
        RecipientId = "1",
        Content = "some message"
      };

      var result = sendMessageRequest.ValidateSendMessageRequest(_twoUsers);
      result.Should().BeTrue();
    }

    [Theory]
    [InlineData("All")] // The recipient is 'All' and there are at least two users
    [InlineData("1")]   // The recipient is '0' and the sender is '1'
    public void Given_Valid_Recipient_Return_True(string recipientId)
    {
      var sendMessageRequest = new SendMessageRequest
      {
        SenderId = "0",
        RecipientType = recipientId == "All" ? RecipientType.All : RecipientType.Single,
        RecipientId = recipientId,
        Content = "some message"
      };

      var result = sendMessageRequest.ValidateSendMessageRequest(_twoUsers);
      result.Should().BeTrue();
    }

    [Fact]
    public void Given_Message_Of_Valid_Length_Return_True()
    {
      var sendMessageRequest = new SendMessageRequest
      {
        SenderId = "0",
        RecipientType = RecipientType.Single,
        RecipientId = "1",
        Content = "this is less than 100 characters"
      };

      var result = sendMessageRequest.ValidateSendMessageRequest(_twoUsers);
      result.Should().BeTrue();
    }
  }
}