using Xunit;
using Chatter;
using FluentAssertions;

namespace GreeterClient.Tests
{
	public class ClientPromptTest
	{
		private readonly ClientPrompt _subjectUnderTest;

		public ClientPromptTest()
		{
			_subjectUnderTest = new ClientPrompt();
		}

		[Fact]
		public void Given_User_Input_Create_SendMessageRequest_For_Direct_Message()
		{
			var senderId = "0";
			var recipientId = "1";
			var userInput = "1: hello one";

			var sendMessageRequest = _subjectUnderTest.CreateSendMessageRequest(senderId, userInput);
			var expectedSendMessageRequest = new SendMessageRequest
			{
				SenderId = senderId,
				RecipientType = RecipientType.Single,
				RecipientId = recipientId,
				Content = "hello one"
			};

			sendMessageRequest.Should().Be(expectedSendMessageRequest);
		}

		[Fact]
		public void Given_User_Input_Create_SendMessageRequest_For_Broadcast()
		{
			var senderId = "0";
			var userInput = "All: hello errbody";

			var sendMessageRequest = _subjectUnderTest.CreateSendMessageRequest(senderId, userInput);
			var expectedSendMessageRequest = new SendMessageRequest
			{
				SenderId = senderId,
				RecipientType = RecipientType.All,
				RecipientId = "All",
				Content = "hello errbody"
			};

			sendMessageRequest.Should().Be(expectedSendMessageRequest);
		}

		[Theory]
		[InlineData("All: hi all")]
		[InlineData("1: hi one")]
		public void Given_Valid_User_Input_Return_True(string userInput)
		{
			var result = _subjectUnderTest.ValidateUserInput(userInput);
			Assert.True(result);
		}

		[Theory]
		[InlineData("hi all")]
		[InlineData("hi: one")]
		public void Given_Invalid_User_Input_Return_False(string userInput)
		{
			var result = _subjectUnderTest.ValidateUserInput(userInput);
			Assert.False(result);
		}
	}
}