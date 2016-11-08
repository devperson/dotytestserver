using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;
using System.Linq;

namespace ArchivesSocietyAB
{
    public class HubServer : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

       
        public void SM_PostNewMessage(MsgData data)
        {
            string senderName = Context.QueryString["name"];
            var userConnections = _connections.GetConnections(senderName);
            if (userConnections.Any())
                Clients.AllExcept(userConnections.ToArray()).CM_OnNewMessage(data.Data);            
        }

        public void SM_AddAppointment(MsgData data)
        {
            string senderName = Context.QueryString["name"];
            var userConnections = _connections.GetConnections(senderName);
            if (userConnections.Any())
                Clients.AllExcept(userConnections.Last()).CM_OnNewAppointmentAdded(data.Data);
        }

        public void SM_DeletedAppointment(MsgData data)
        {
            string senderName = Context.QueryString["name"];
            var userConnections = _connections.GetConnections(senderName);
            if (userConnections.Any())
                Clients.AllExcept(userConnections.Last()).CM_OnDeleteAppointment(data.Data);
        }

        public void SM_Ping(MsgData data)
        {
            string senderName = Context.QueryString["name"];
            var userConnections = _connections.GetConnections(senderName);
            if (userConnections.Any())
            {
                //Clients.AllExcept(userConnections.Last()).CM_OnNewAppointmentAdded(data.Data);
            }
        }

        //public void SM_DeleteMessage(MsgData data)
        //{
        //    string senderName = Context.QueryString["name"];
        //    var userConnections = _connections.GetConnections(senderName);
        //    if (userConnections.Any())
        //        Clients.AllExcept(userConnections.Last()).CM_OnMessageDeleted(data.Data);
        //}

        //public void SM_NewFeedPosted(MsgData data)
        //{
        //    string senderName = Context.QueryString["name"];
        //    var userConnections = _connections.GetConnections(senderName);
        //    if (userConnections.Any())
        //        Clients.AllExcept(userConnections.Last()).CM_OnNewFeedPost(data.Data);
        //}

        //public void SM_NewFeedCommentPosted(MsgData data)
        //{
        //    string senderName = Context.QueryString["name"];
        //    var userConnections = _connections.GetConnections(senderName);
        //    if (userConnections.Any())
        //        Clients.AllExcept(userConnections.Last()).CM_OnNewFeedCommentPost(data.Data);
        //}

        public override Task OnConnected()
        {
            string name = Context.QueryString["name"];
            Debug.WriteLine(string.Format("{0} Connected", name));
            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.QueryString["name"];
            Debug.WriteLine(string.Format("{0} Disconnected", name));
            if (!string.IsNullOrEmpty(name))
                _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }        
    }
}