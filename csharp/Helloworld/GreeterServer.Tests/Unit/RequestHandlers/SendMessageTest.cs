using System.Collections.Generic;
using GreeterServer.RequestHandlers;
using Xunit;
using Chatter;
using Google.Protobuf.Collections;
using FluentAssertions;
using Grpc.Core;

namespace GreeterServer.Tests.Unit.RequestHandlers
{
	public class SendMessageTest
	{
		private readonly MessageRequestHandler _subjectUnderTest;
		private readonly List<string> _users;
		private readonly List<Message> _messages;

		public SendMessageTest()
		{
			_users = new List<string> { "0", "1", "2" };
			_messages = new List<Message>();
			_subjectUnderTest = new MessageRequestHandler(_users, _messages);
		}

		/// <summary>
		/// Given a single message is sent, return one server ack.
		/// Confirm state of messages and response.
		/// </summary>
		[Fact]
		public void Given_A_Single_Message_Is_Sent_Return_One_Server_Ack()
		{
			var sendMessageRequest = new SendMessageRequest
			{
				SenderId = "0",
				RecipientType = RecipientType.Single,
				RecipientId = "1",
				Content = "Hi 1"
			};
			var sendMessageResponse = _subjectUnderTest.SendMessage(sendMessageRequest);
			var messageStatuses = sendMessageResponse.Result.MessageStatuses;

			var expectedServerAck = new MessageStatus
			{
				Content = sendMessageRequest.Content,
				DeliveredTo = DeliveredTo.Server,
				RecipientId = sendMessageRequest.RecipientId
			};
			var expectedMessageStatuses = new RepeatedField<MessageStatus> { expectedServerAck };

			messageStatuses.Should().Equal(expectedMessageStatuses);
			_messages.Should().HaveCount(1);
			_messages[0].SenderId.Should().Be(sendMessageRequest.SenderId);
			_messages[0].RecipientId.Should().Be(sendMessageRequest.RecipientId);
			_messages[0].Content.Should().Be(sendMessageRequest.Content);
			_messages[0].DeliveredToRecipient.Should().Be(false);
		}

		/// <summary>
		/// Given a broadcast is sent, return multiple server acks.
		/// Confirm state of messages and response.
		/// </summary>
		[Fact]
		public void Given_A_Broadcast_Is_Sent_Return_Multiple_Server_Acks()
		{
			var sendMessageRequest = new SendMessageRequest
			{
				SenderId = "1",
				RecipientType = RecipientType.All,
				RecipientId = "All",
				Content = "Hi everybody"
			};
			var sendMessageResponse = _subjectUnderTest.SendMessage(sendMessageRequest);
			var messageStatuses = sendMessageResponse.Result.MessageStatuses;
			var otherUsers = new List<string> { "0", "2" };

			var expectedMessageStatuses = new RepeatedField<MessageStatus>();
			foreach (var user in otherUsers)
			{
				var expectedServerAck = new MessageStatus
				{
					Content = sendMessageRequest.Content,
					DeliveredTo = DeliveredTo.Server,
					RecipientId = user
				};

				expectedMessageStatuses.Add(expectedServerAck);
			}

			messageStatuses.Should().Equal(expectedMessageStatuses);
			_messages.Should().HaveCount(2);
			foreach (var message in _messages)
			{
				message.SenderId.Should().Be(sendMessageRequest.SenderId);
				message.Content.Should().Be(sendMessageRequest.Content);
				message.DeliveredToRecipient.Should().Be(false);
			}
			_messages[0].RecipientId.Should().Be("0");
			_messages[1].RecipientId.Should().Be("2");
		}

		[Fact]
		public void Given_A_Sender_Is_Not_Specified_Throw_Exception()
		{
			var sendMessageRequest = new SendMessageRequest
			{
				SenderId = "",
				RecipientType = RecipientType.Single,
				RecipientId = "1",
				Content = "some message"
			};

			var result = Assert.ThrowsAsync<RpcException>(async () => await _subjectUnderTest.SendMessage(sendMessageRequest));
			result.Result.Message.Should().Be($"Status(StatusCode=InvalidArgument, Detail=\"You did not specify a sender. Please try again.\")");
		}

		[Theory]
		[InlineData("3")]   // The recipient is '2', but '2' is not a user
		[InlineData("1")]   // The recipient and the sender are both '1'
		public void Given_Invalid_Recipient_Throw_Exception(string recipientId)
		{
			var sendMessageRequest = new SendMessageRequest
			{
				SenderId = "1",
				RecipientType = RecipientType.Single,
				RecipientId = recipientId,
				Content = "hello"
			};

			var result = Assert.ThrowsAsync<RpcException>(async () => await _subjectUnderTest.SendMessage(sendMessageRequest));
			result.Result.Message.Should().Be($"Status(StatusCode=InvalidArgument, Detail=\"You specified an invalid recipient. Please try again.\")");
		}

		[Fact]
		public void Given_User_Sends_Message_To_All_But_There_Are_No_Other_Users_Throw_Exception()
		{
			var users = new List<string> { "1" };
			var subjectUnderTestWithOnlyOneUser = new MessageRequestHandler(users, _messages);
			var sendMessageRequest = new SendMessageRequest
			{
				SenderId = "1",
				RecipientType = RecipientType.All,
				RecipientId = "All",
				Content = "hello errbody"
			};

			var result = Assert.ThrowsAsync<RpcException>(async () => await subjectUnderTestWithOnlyOneUser.SendMessage(sendMessageRequest));
			result.Result.Message.Should().Be($"Status(StatusCode=InvalidArgument, Detail=\"You specified an invalid recipient. Please try again.\")");
		}

		[Fact]
		public void Given_User_Sends_Message_With_Invalid_Recipient_Format_Throw_Exception()
		{
			var sendMessageRequest = new SendMessageRequest
			{
				SenderId = "1",
				RecipientType = RecipientType.All,
				RecipientId = "all",
				Content = "hello errbody"
			};

			var result = Assert.ThrowsAsync<RpcException>(async () => await _subjectUnderTest.SendMessage(sendMessageRequest));
			result.Result.Message.Should().Be($"Status(StatusCode=InvalidArgument, Detail=\"You specified an invalid recipient. Please try again.\")");
		}

		[Fact]
		public void Given_Message_Of_Invalid_Length_Throw_Exception()
		{
			var sendMessageRequest = new SendMessageRequest
			{
				SenderId = "0",
				RecipientType = RecipientType.Single,
				RecipientId = "1",
				Content = "this is 101 characters this is 101 characters this is 101 characters this is 101 characters this is 1"
			};

			var result = Assert.ThrowsAsync<RpcException>(async () => await _subjectUnderTest.SendMessage(sendMessageRequest));
			result.Result.Message.Should().Be($"Status(StatusCode=InvalidArgument, Detail=\"Message must be 1-100 characters. Please try again.\")");
		}
	}
}
