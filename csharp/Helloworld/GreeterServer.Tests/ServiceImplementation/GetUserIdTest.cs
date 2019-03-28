// using Xunit;
// using Chatter;
// using GreeterServer;
// using FluentAssertions;

// namespace GreeterServer.Tests.ServiceImplementation
// {
//   public class GetUserIdTest
//   {
//     private ServiceImplementation.ChatterImpl _subjectUnderTest;

//     public GetUserIdTest()
//     {
//       _subjectUnderTest = new ServiceImplementation.ChatterImpl();
//     }
//     // Returns same user ID if users already includes user id - expect _users to contain xx, expect response to be xx
//     /// <summary>
//     /// Given the user is already in users return same user identifier.
//     /// </summary>
//     [Fact]
//     public void Given_User_Exists_Return_Same_User_Id()
//     {
//       //_subjectUnderTest.GetUserId(new GetUserIdRequest { } );


//       //var userIdRequest = new GetUserIdRequest { UserId = "1" };
//       //var userIdResponse = new GetUserIdResponse { UserId = "1" };

//     }

//     // Returns next user ID if user does not exist in list of users - expect _users to contain xx, expect response to be xx
//     [Fact]
//     public void Given_User_Is_Not_In_Users_Return_Next_User_Id()
//     {
//       //var userIdRequest = new GetUserIdRequest();
//       //var userIdResponse = new GetUserIdResponse { UserId = "0" };
//     }
//   }
// }
