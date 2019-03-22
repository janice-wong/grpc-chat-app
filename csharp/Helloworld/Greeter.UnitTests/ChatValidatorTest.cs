using System.Collections.Generic;
using GreeterServer;
using Xunit;

namespace Greeter.UnitTests
{
	public class ChatValidatorTest
	{
		private readonly List<string> _users = new List<string> { "0", "1" };

		[Theory]
		[InlineData("this is less than 100 characters")]
		public void Given_Message_Of_Valid_Length_Return_True(string message)
		{
			Assert.True(ChatValidator.ValidateMessageCharLimit(message));
		}

		[Theory]
		[InlineData("this is 101 characters this is 101 characters this is 101 characters this is 101 characters this is 1")]
		public void Given_Message_Of_Invalid_Length_Return_False(string message)
		{
			Assert.False(ChatValidator.ValidateMessageCharLimit(message));
		}
		
		[Theory]
		[InlineData("All")]
		[InlineData("0")]
		public void Given_Valid_Recipient_Return_True(string recipientId)
		{
			Assert.True(ChatValidator.ValidateRecipient(_users, "1", recipientId));
		}

		[Theory]
		[InlineData("2")]
		[InlineData("1")]
		public void Given_Invalid_Recipient_Return_False(string recipientId)
		{
			Assert.False(ChatValidator.ValidateRecipient(_users, "1", recipientId));
		}
	}
}
