using System.Threading.Tasks;
using Grpc.Core;
using Chatter;
using GreeterServer.RequestHandlers;

namespace GreeterServer.ServiceImplementation
{
	public partial class ChatterImpl
	{
		public override Task<GetMessageStatusResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
		{
			return new MessageRequestHandler(_users, _messages).SendMessage(request);
		}
	}
}
