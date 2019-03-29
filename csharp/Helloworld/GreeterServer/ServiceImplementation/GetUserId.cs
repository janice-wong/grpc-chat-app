using System.Threading.Tasks;
using Chatter;
using Grpc.Core;
using GreeterServer.RequestHandlers;

namespace GreeterServer.ServiceImplementation
{
  public partial class ChatterImpl
  {
    public override Task<GetUserIdResponse> GetUserId(GetUserIdRequest request, ServerCallContext context)
    {
      return new UserRequestHandler(_users).GetUserId(request);
    }
  }
}
