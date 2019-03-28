using System.Collections.Generic;
using Chatter;
using GreeterServer.RequestHandlers;

namespace GreeterServer.ServiceImplementation
{
  public partial class ChatterImpl : Greeter.GreeterBase
  {
    private readonly List<string> _users;
    private readonly List<Message> _messages;

    public ChatterImpl()
    {
      _users = new List<string>();
      _messages = new List<Message>();
    }
  }
}
