using System.Threading.Tasks;
using Grpc.Core;
using Chatter;

namespace GreeterServer.ServiceImplementation
{
  public partial class ChatterImpl
  {
    public override Task<GetMessageResponse> GetFirstUnreadMessage(GetMessageRequest request, ServerCallContext context)
    {
      foreach (var msg in _messages)
      {
        if (msg.RecipientId == request.RecipientId && !msg.DeliveredToRecipient)
        {
          var receivedMessage = msg;
          msg.DeliveredToRecipient = true;

          return Task.FromResult(new GetMessageResponse { SenderId = msg.SenderId, Content = msg.Content });
        }
      }

      return Task.FromResult(new GetMessageResponse());
    }
  }
}
