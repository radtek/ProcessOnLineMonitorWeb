//using System.Collections.Generic;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
//using System.Threading.Tasks;
//using System;

//using System.Web;
//using System.Threading;
//using System.Linq;

//namespace SignalR.MessageHub
//{
//    [HubName("AuthorizeMsgHub")]
//    public class MessageHubSingle : Hub
//    {

//        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>(); //暂时不用

//        public static List<UserInfo> OnlineUsers = new List<UserInfo>(); 

//        public MessageHubSingle()
//        {
//        }

//        public override Task OnConnected()
//        {
//            return base.OnConnected();
//        }

//        public void ConnectSignalR(string userId)
//        {
//            string ErrMsg = "";
//            try
//            {
//                var connnectId = Context.ConnectionId;

//                if (OnlineUsers.Count(x => x.ConnectionId == connnectId) == 0)
//                {
//                    if (OnlineUsers.Any(x => x.UserId == userId))
//                    {
//                        var items = OnlineUsers.Where(x => x.UserId == userId).ToList();
//                        foreach (var item in items)
//                        {
//                            Clients.AllExcept(connnectId).onUserDisconnected(item.ConnectionId, item.UserName);
//                        }
//                        OnlineUsers.RemoveAll(x => x.UserId == userId);
//                    }

//                    //添加在线人员
//                    OnlineUsers.Add(new UserInfo
//                    {
//                        ConnectionId = connnectId,
//                        UserId = userId,
//                        //UserName = userName,
//                        LastLoginTime = DateTime.Now
//                    });
//                }
//                //string clientName = Context.QueryString["clientName"].ToString();
//                // 所有客户端同步在线用户
//                //Clients.All.onConnected(connnectId, userName, OnlineUsers);
//                //Clients.Client(connnectId).onConnected(connnectId);  //

//                Clients.Caller.onConnected(connnectId, "sr1", userId);
//            }
//            catch (Exception ex)
//            {
//                ErrMsg = ex.Message; // + Environment.NewLine + ex.StackTrace;
//                Clients.Caller.onConnected(Context.ConnectionId, "fail," + ErrMsg);
//            }
//        }


//        /// <summary>
//        /// 重新链接
//        /// </summary>
//        /// <returns></returns>
//        public override Task OnReconnected()
//        {

//            return base.OnReconnected();
//        }


//        public override Task OnDisconnected(bool stopCalled)
//        {
//            string ErrMsg = "";
//            try
//            {
//                var user = OnlineUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);

//                // 判断用户是否存在,存在则删除
//                if (user == null) return base.OnDisconnected(stopCalled);

//                Clients.All.onUserDisconnected(user.ConnectionId, user.UserName);   //调用客户端用户离线通知
//                                                                                    // 删除用户
//                OnlineUsers.Remove(user);

//            }
//            catch (Exception ex)
//            {
//                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
//            }
//            return base.OnDisconnected(stopCalled);
//        }

//    }


//    public class ConnectionMapping<T>
//    {
//        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

//        public int Count
//        {
//            get
//            {
//                return _connections.Count;
//            }
//        }

//        public void Add(T key, string connectionId)
//        {
//            lock (_connections)
//            {
//                HashSet<string> connections;
//                if (!_connections.TryGetValue(key, out connections))
//                {
//                    connections = new HashSet<string>();
//                    _connections.Add(key, connections);
//                }

//                lock (connections)
//                {
//                    connections.Add(connectionId);
//                }
//            }
//        }

//        public IEnumerable<string> GetConnections(T key)
//        {
//            HashSet<string> connections;
//            if (_connections.TryGetValue(key, out connections))
//            {
//                return connections;
//            }

//            return Enumerable.Empty<string>();
//        }

//        public void Remove(T key, string connectionId)
//        {
//            lock (_connections)
//            {
//                HashSet<string> connections;
//                if (!_connections.TryGetValue(key, out connections))
//                {
//                    return;
//                }

//                lock (connections)
//                {
//                    connections.Remove(connectionId);

//                    if (connections.Count == 0)
//                    {
//                        _connections.Remove(key);
//                    }
//                }
//            }
//        }
//    }


//    public class UserInfo
//    {
//        /// <summary>
//        /// 连接ID
//        /// </summary>
//        public string ConnectionId { get; set; }

//        public string UserId { get; set; }

//        /// <summary>
//        /// 用户名
//        /// </summary>
//        public string UserName { get; set; }

//        /// <summary>
//        /// 最后登录时间
//        /// </summary>
//        public DateTime LastLoginTime { get; set; }
//    }

//}

///*
//The HubName attribute specifies how the Hub will be referenced in JavaScript code on the client. 
//The default name on the client if you don't use this attribute is a camel-cased version of the class name, which in this case would be stockTickerHub.
//*/
