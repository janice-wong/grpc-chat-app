using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chatter;

namespace GreeterServer.RequestHandlers
{
	public class UserRequestHandler
	{
		private List<string> _users;

		public UserRequestHandler(List<string> users)
		{
			_users = users;
		}

		public Task<GetUserIdResponse> GetUserId(GetUserIdRequest request)
		{
			// TODO: Remove user ID from list of users once user's connection is gone.

			// Return same user ID if user calls this method more than once
			var userExists = _users.Contains(request.UserId);
			if (userExists)
			{
				return Task.FromResult(new GetUserIdResponse { UserId = request.UserId });
			}

			_users.Add(Convert.ToString(_users.Count));
			return Task.FromResult(new GetUserIdResponse { UserId = _users[_users.Count - 1] });
		}
	}
}
