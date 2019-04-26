using System.Collections.Generic;
using GreeterServer.RequestHandlers;
using Xunit;
using Chatter;
using FluentAssertions;

namespace GreeterServer.Tests.Unit.RequestHandlers
{
	public class GetFirstUnreadMessageTest
	{
		private readonly MessageRequestHandler _subjectUnderTest;
		private readonly List<string> _users;
		private readonly List<Message> _messages;
		private readonly Message _messageDeliveredToRecipient;
		private readonly Message _messageDeliveredToServer;

		public GetFirstUnreadMessageTest()
		{
			// TODO: Maybe these can go in some MessageTestBase
			_users = new List<string> { "0", "1", "2" };
			_messages = new List<Message>();
			_messageDeliveredToRecipient = new Message("0", "1", "0 to 1: Delivered to 1");
			_messageDeliveredToRecipient.MarkAsReceived();
			_messageDeliveredToServer = new Message("0", "1", "0 to 1: Delivered to the server");

			_messages.Add(_messageDeliveredToRecipient);
			_messages.Add(_messageDeliveredToServer);

			_subjectUnderTest = new MessageRequestHandler(_users, _messages);
		}

		/// <summary>
		/// Given there is an unread message for a specific user, return the message.
		/// Confirm state of messages and response.
		/// </summary>
		[Fact]
		public void Given_There_Is_An_Unread_Message_Return_The_Message()
		{
			var messageResponse = _subjectUnderTest.GetFirstUnreadMessage(new GetMessageRequest { RecipientId = "1" });
			var message = messageResponse.Result;

			var expectedMessage = new GetMessageResponse { SenderId = "0", Content = "0 to 1: Delivered to the server" };
			var expectedMessageDeliveredToRecipient = new Message("0", "1", "0 to 1: Delivered to 1", _messageDeliveredToRecipient.Id);
			expectedMessageDeliveredToRecipient.MarkAsReceived();
			var anotherExpectedMessageDeliveredToRecipient = new Message("0", "1", "0 to 1: Delivered to the server", _messageDeliveredToServer.Id);
			anotherExpectedMessageDeliveredToRecipient.MarkAsReceived();

			var expectedMessages = new List<Message>
			{
				expectedMessageDeliveredToRecipient,
				anotherExpectedMessageDeliveredToRecipient
			};

			message.Should().Be(expectedMessage);
			_messages.Should().BeEquivalentTo(expectedMessages);
		}

		/// <summary>
		/// Given there are no unread messages for a specific user, return no messages.
		/// Confirm state of messages and response.
		/// </summary>
		[Fact]
		public void Given_There_Are_No_Unread_Messages_Return_No_Messages()
		{
			_messageDeliveredToServer.MarkAsReceived();

			var messageResponse = _subjectUnderTest.GetFirstUnreadMessage(new GetMessageRequest { RecipientId = "1" });
			var message = messageResponse.Result;

			var expectedMessage = new GetMessageResponse();
			var expectedMessageDeliveredToRecipient = new Message("0", "1", "0 to 1: Delivered to 1", _messageDeliveredToRecipient.Id);
			expectedMessageDeliveredToRecipient.MarkAsReceived();
			var anotherExpectedMessageDeliveredToRecipient = new Message("0", "1", "0 to 1: Delivered to the server", _messageDeliveredToServer.Id);
			anotherExpectedMessageDeliveredToRecipient.MarkAsReceived();

			var expectedMessages = new List<Message>
			{
				expectedMessageDeliveredToRecipient,
				anotherExpectedMessageDeliveredToRecipient
			};

			message.Should().Be(expectedMessage);
			_messages.Should().BeEquivalentTo(expectedMessages);
		}
	}
}