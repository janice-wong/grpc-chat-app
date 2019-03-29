using System.Threading.Tasks;
using Grpc.Core;
using Chatter;
using GreeterServer.RequestHandlers;

namespace GreeterServer.ServiceImplementation
{
  public partial class ChatterImpl
  {
    public override Task<GetMessageResponse> GetFirstUnreadMessage(GetMessageRequest request, ServerCallContext context)
    {
      return new MessageRequestHandler(_users,_messages).GetFirstUnreadMessage(request);
    }
  }
}
