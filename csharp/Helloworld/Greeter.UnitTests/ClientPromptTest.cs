using GreeterClient;
using Xunit;

namespace Greeter.UnitTests
{
  public class ClientPromptTest
  {
		[Theory]
		[InlineData("All: hi all")]
		[InlineData("1: hi one")]
    public void Given_Valid_User_Input_Return_True(string userInput)
    {
			var clientPrompt = new ClientPrompt();
			var result = clientPrompt.ValidateUserInput(userInput);
			Assert.True(result);
		}

		[Theory]
		[InlineData("hi all")]
		[InlineData("hi: one")]
    public void Given_Invalid_User_Input_Return_False(string userInput)
    {
			var clientPrompt = new ClientPrompt();
			var result = clientPrompt.ValidateUserInput(userInput);
			Assert.False(result);
		}
	}
}