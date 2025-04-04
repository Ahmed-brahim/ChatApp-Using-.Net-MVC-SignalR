using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace SignalR_Project.Hubs
{
    public class ChatHub:Hub
    {
		
        public void SendMessage(string name, string message)
        {
			//this method to make a group and add a connection to it
			Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
			//this method to remove a connection from a group
			Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
			//this method to send a message to all connection
            Clients.All.SendAsync("ReceiveMessage", name, message);
        }
		//this method is called when a new connection is established
		public override Task OnConnectedAsync()  
		{
			return base.OnConnectedAsync();
		}
		//this method is called when a connection is disconnected
        public override Task OnDisconnectedAsync(Exception exception)
		{
			return base.OnDisconnectedAsync(exception);
		}
	}
}
