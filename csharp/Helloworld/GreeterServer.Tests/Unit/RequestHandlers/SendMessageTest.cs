using System.Collections.Generic;
using GreeterServer.RequestHandlers;
using Xunit;
using Chatter;
using Google.Protobuf.Collections;
using FluentAssertions;

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

		// Given a single message is sent, confirm state of _messages and response
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

		// Given a broadcast is sent, confirm state of _messages and response
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
	}
}
