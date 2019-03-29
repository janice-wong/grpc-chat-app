using System.Collections.Generic;
using GreeterServer.RequestHandlers;
using Xunit;
using Chatter;
using FluentAssertions;

namespace GreeterServer.Tests.Unit.RequestHandlers
{
  public class GetUserIdTest
  {
    private readonly UserRequestHandler _subjectUnderTest;
    private readonly List<string> _users;

    public GetUserIdTest()
    {
      _users = new List<string>();
      _subjectUnderTest = new UserRequestHandler(_users);
    }

    /// <summary>
    /// Given the user already exists in users, return the same user id.
    /// </summary>
    [Fact]
    public void Given_User_Exists_Return_Same_User_Id()
    {
      _subjectUnderTest.GetUserId(new GetUserIdRequest { } );
      var userIdRequest = new GetUserIdRequest { UserId = "0" };
      var userIdResponse = _subjectUnderTest.GetUserId(userIdRequest);
      var userId = userIdResponse.Result.UserId;

      var expectedUserId = "0";
      var expectedUsers = new List<string> { "0" };

      userId.Should().Be(expectedUserId);
      _users.Should().Equal(expectedUsers);
    }

    /// <summary>
    /// Given the user does not exist in users, return a new user id.
    /// </summary>
    [Fact]
    public void Given_User_Does_Not_Exist_Return_New_User_Id()
    {
      _subjectUnderTest.GetUserId(new GetUserIdRequest { } );
      var userIdRequest = new GetUserIdRequest { };
      var userIdResponse = _subjectUnderTest.GetUserId(userIdRequest);
      var userId = userIdResponse.Result.UserId;

      var expectedUserId = "1";
      var expectedUsers = new List<string> { "0", "1" };

      userId.Should().Be(expectedUserId);
      _users.Should().Equal(expectedUsers);
    }
  }
}
