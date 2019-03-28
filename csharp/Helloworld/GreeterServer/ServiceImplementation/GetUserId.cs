using System.Threading.Tasks;
using Chatter;
using Grpc.Core;

namespace GreeterServer.ServiceImplementation
{
	public partial class ChatterImpl
	{
		public override Task<GetUserIdResponse> GetUserId(GetUserIdRequest request, ServerCallContext context)
		{
			return new RequestHandlers.UserRequestHandler(_users).GetUserId(request);
		}
	}
}
