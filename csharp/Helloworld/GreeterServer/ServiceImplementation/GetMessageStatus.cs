using System.Threading.Tasks;
using Grpc.Core;
using Chatter;

namespace GreeterServer.ServiceImplementation
{
	public partial class ChatterImpl
	{
		public override Task<GetMessageStatusResponse> GetMessageStatus(GetMessageStatusRequest request, ServerCallContext context)
		{
			return new RequestHandlers.MessageRequestHandler(_users, _messages).GetMessageStatus(request);
		}
	}
}
