using System.Collections.Generic;
using GreeterServer;
using Xunit;

namespace Greeter.UnitTests
{
	public class ChatValidatorTest
	{
		private readonly List<string> _twoUsers = new List<string> { "0", "1" };
		private readonly List<string> _oneUser = new List<string> { "0" };

		[Fact]
		public void Given_Message_Of_Valid_Length_Return_True()
		{
			Assert.True(ChatValidator.ValidateMessageCharLimit("this is less than 100 characters"));
		}

		[Fact]
		public void Given_Message_Of_Invalid_Length_Return_False()
		{
			Assert.False(ChatValidator.ValidateMessageCharLimit("this is 101 characters this is 101 characters this is 101 characters this is 101 characters this is 1"));
		}
		
		[Theory]
		[InlineData("All")] // The recipient is 'All' and there are at least two users
		[InlineData("0")]		// The recipient is '0' and the sender is '1'
		public void Given_Valid_Recipient_Return_True(string recipientId)
		{
			Assert.True(ChatValidator.ValidateRecipient(_twoUsers, "1", recipientId));
		}

		[Theory]
		[InlineData("2")]		// The recipient is '2', but '2' is not a user
		[InlineData("1")]		// The recipient and the sender are both '1'
		public void Given_Invalid_Recipient_Return_False(string recipientId)
		{
			Assert.False(ChatValidator.ValidateRecipient(_twoUsers, "1", recipientId));
		}

		[Fact]
		public void Given_User_Sends_Message_To_All_But_There_Are_No_Other_Users_Return_False()
		{
			Assert.False(ChatValidator.ValidateRecipient(_oneUser, "0", "All"));
		}
	}
}
