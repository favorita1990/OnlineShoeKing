

namespace OnlineShoeKing.Hubs
{
    using Context;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Helpers;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    using OnlineShoeKing.Models;

    [HubName("notifications")]
    public class NotificationsHub : Hub
    {
        public static Dictionary<string, Pair<string, string, string, string, List<Tuple<string, string, string, bool, string, string>>>> Users =
        new Dictionary<string, Pair<string, string, string, string, List<Tuple<string, string, string, bool, string, string>>>>();

        public static List<Tuple<string, string, string, bool, string, string>> customers =
        new List<Tuple<string, string, string, bool, string, string>>();

        private readonly DBContext db = new DBContext();

        public class Pair<T1, T2, T3, T4, T5>
        {
            private T2 userName;
            private T1 id;
            private T3 fullName;
            private T4 picture;
            private T5 list;

            public Pair(T1 id, T2 userName, T3 fullName, T4 picture, T5 list)
            {
                this.userName = userName;
                this.id = id;
                this.fullName = fullName;
                this.picture = picture;
                this.list = list;
            }

            public T1 Id
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.id = value;
                }
            }
            public T2 UserName
            {
                get
                {
                    return this.userName;
                }
                set
                {
                    this.userName = value;
                }
            }
            public T3 FullName
            {
                get
                {
                    return this.fullName;
                }
                set
                {
                    this.fullName = value;
                }
            }
            public T4 Picture
            {
                get
                {
                    return this.picture;
                }
                set
                {
                    this.picture = value;
                }
            }
            public T5 ListUsers
            {
                get
                {
                    return this.list;
                }
                set
                {
                    this.list = value;
                }
            }
        }

        public void Connect(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName)) return;
            var user = this.db.Users.FirstOrDefault(w => w.Email == userName);
            if (user == null) return;
            {
                var userPass = Crypto.VerifyHashedPassword(user.PasswordHash, password);
                if (userPass)
                {

                    var userOnlineNew = customers.FirstOrDefault(w => w.Item3 == userName);
                    if (userOnlineNew == null)
                    {
                        var userTemp = this.db.Users.FirstOrDefault(f => f.Email == userName);
                        userTemp.OnlineOrOffline = true;
                        userTemp.ConnectionId = this.Context.ConnectionId;
                        this.db.SaveChanges();
                        var fullname = $"{userTemp.FirstName} {userTemp.LastName}";

                        userTemp.ImageUrl = userTemp.ImageUrl != null ? Resources.Pictures.FolderUsersImages + userTemp.ImageUrl :
                                                     (userTemp.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                        customers.Add(new Tuple<string, string, string, bool, string, string>
                            (userTemp.Id, userTemp.Gender, userTemp.Email, userTemp.OnlineOrOffline, fullname, userTemp.ImageUrl));
                    }

                    try
                    {
                        customers = customers.Distinct().ToList();
                    }
                    catch
                    {
                        // ignored
                    }

                    //var onlineUserTemp = db.Users.OrderBy(o => o.CreationDate).Where(w => w.OnlineOrOffline == true).ToList();
                    //foreach (var userOnlineTemp in customers)
                    //{
                    //    string fullname = string.Format("{0} {1}", userOnlineTemp.FirstName, userOnlineTemp.LastName);
                    //    customers.Add(new Tuple<string, string, string, bool, string, string>
                    //    (userOnlineTemp.Id, userOnlineTemp.Gender, userOnlineTemp.Email, userOnlineTemp.OnlineOrOffline, fullname, userOnlineTemp.ImageUrl));
                    //}
                }
            }
        }

        public void Disconnect()
        {
            var userName = Context.User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                db.Users.FirstOrDefault(w => w.Email == userName).OnlineOrOffline = false;
                db.SaveChanges();

                customers = customers.Where(w => w.Item3 != userName).ToList();

                foreach (var customer in customers.ToList())
                {
                    var usersOnline = customers.Where(w => w.Item3 != customer.Item3);
                    var onlineUsersReadOrUnread = new List<Tuple<string, string, string, bool, string, string>>();
                    var userId = customer.Item1;
                    foreach (var toUser in usersOnline)
                    {
                        Message msg = null;
                        msg = this.db.Messages?.ToList().FirstOrDefault(w => ((w.FromUserId == toUser.Item1 && w.ToUserId == userId)
                                                                        && (w.ReadOrUnread == false)));

                        if (msg != null)
                        {
                            onlineUsersReadOrUnread.Add(
                                new Tuple<string, string, string, bool, string, string>
                                    (toUser.Item1, toUser.Item2, toUser.Item3, false, toUser.Item5, toUser.Item6));
                        }
                        else
                        {
                            onlineUsersReadOrUnread.Add(
                                new Tuple<string, string, string, bool, string, string>
                                    (toUser.Item1, toUser.Item2, toUser.Item3, true, toUser.Item5, toUser.Item6));
                        }
                    }

                    onlineUsersReadOrUnread = onlineUsersReadOrUnread.OrderBy(o => o.Item4).ToList();
                    this.Clients.User(customer.Item3).onConnected(onlineUsersReadOrUnread);
                }

                var customerCount = customers.Count > 0 ? customers.Count - 1 : 0;
                Clients.All.updateUsersOnlineCount(customerCount);
            }
        }

        public override Task OnConnected()
        {

            var userName = Context.User.Identity.Name;

            if (string.IsNullOrEmpty(userName)) return base.OnConnected();
            var userOnlineNew = customers.FirstOrDefault(w => w.Item3 == userName);
            if(userOnlineNew == null)
            {
                var userTemp = this.db.Users.FirstOrDefault(f => f.Email == userName);
                userTemp.OnlineOrOffline = true;
                this.db.SaveChanges();
                var fullname = $"{userTemp.FirstName} {userTemp.LastName}";

                userTemp.ImageUrl = userTemp.ImageUrl != null ? Resources.Pictures.FolderUsersImages + userTemp.ImageUrl :
                                                     (userTemp.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                customers.Add(new Tuple<string, string, string, bool, string, string>
                    (userTemp.Id, userTemp.Gender, userTemp.Email, userTemp.OnlineOrOffline, fullname, userTemp.ImageUrl));
            }

            try
            {
                if (customers.Count > 0)
                {
                    customers = customers.Distinct().ToList();
                }
            }
            catch
            {
                // ignored
            }

            foreach (var customer in customers.ToList())
            {
                var currentUserName = customer.Item3;
                var usersOnline = customers.Where(w => w.Item3 != currentUserName)?.ToList();
                var onlineUsersReadOrUnread = new List<Tuple<string, string, string, bool, string, string>>();
                var userId = customer.Item1;
                foreach (var toUser in usersOnline)
                {
                    Message msg = null;
                    msg = this.db.Messages?.ToList().FirstOrDefault(w => ((w.FromUserId == toUser.Item1 && w.ToUserId == userId)
                                                                && (w.ReadOrUnread == false)));

                    if (msg != null)
                    {
                        onlineUsersReadOrUnread.Add(
                            new Tuple<string, string, string, bool, string, string>
                                (toUser.Item1, toUser.Item2, toUser.Item3, false, toUser.Item5, toUser.Item6));
                    }
                    else
                    {
                        onlineUsersReadOrUnread.Add(
                            new Tuple<string, string, string, bool, string, string>
                                (toUser.Item1, toUser.Item2, toUser.Item3, true, toUser.Item5, toUser.Item6));
                    }
                }

                onlineUsersReadOrUnread = onlineUsersReadOrUnread.OrderBy(o => o.Item4).ToList();
                this.Clients.User(customer.Item3).onConnected(onlineUsersReadOrUnread);
            }

            var customerCount = customers.Count > 0 ? customers.Count - 1 : 0;
            this.Clients.All.updateUsersOnlineCount(customerCount);


            //Clients.All.onConnectedCustomers(customers);

            //Clients.All.updateUnreadMsgUsersCount(Users);

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            var userName = Context.User.Identity.Name;

            if (string.IsNullOrEmpty(userName)) return base.OnReconnected();
            var userOnlineNew = customers.FirstOrDefault(w => w.Item3 == userName);
            if (userOnlineNew == null)
            {
                var userTemp = this.db.Users.FirstOrDefault(f => f.Email == userName);
                userTemp.OnlineOrOffline = true;
                this.db.SaveChanges();
                var fullname = $"{userTemp.FirstName} {userTemp.LastName}";

                userTemp.ImageUrl = userTemp.ImageUrl != null ? Resources.Pictures.FolderUsersImages + userTemp.ImageUrl :
                                                     (userTemp.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                customers.Add(new Tuple<string, string, string, bool, string, string>
                    (userTemp.Id, userTemp.Gender, userTemp.Email, userTemp.OnlineOrOffline, fullname, userTemp.ImageUrl));
            }

            try
            {
                if (customers.Count > 0)
                {
                    customers = customers.Distinct().ToList();
                }
            }
            catch
            {
                // ignored
            }

            foreach (var customer in customers.ToList())
            {
                var currentUserName = customer.Item3;
                var usersOnline = customers?.Where(w => w.Item3 != currentUserName).ToList();
                var onlineUsersReadOrUnread = new List<Tuple<string, string, string, bool, string, string>>();
                var userId = customer.Item1;
                foreach (var userOnline in usersOnline)
                {
                    Message msg = null;
                    msg = this.db.Messages?.ToList().FirstOrDefault(w => (w.FromUserId == userOnline.Item1 && w.ToUserId == userId)
                                                                         && (w.ReadOrUnread == false));

                    if (msg != null)
                    {
                        onlineUsersReadOrUnread.Add(
                            new Tuple<string, string, string, bool, string, string>
                                (userOnline.Item1, userOnline.Item2, userOnline.Item3, false, userOnline.Item5, userOnline.Item6));
                    }
                    else
                    {
                        onlineUsersReadOrUnread.Add(
                            new Tuple<string, string, string, bool, string, string>
                                (userOnline.Item1, userOnline.Item2, userOnline.Item3, true, userOnline.Item5, userOnline.Item6));
                    }
                }

                   

                onlineUsersReadOrUnread = onlineUsersReadOrUnread.OrderBy(o => o.Item4).ToList();
                this.Clients.User(customer.Item3).onConnected(onlineUsersReadOrUnread);
            }

            var customerCount = customers.Count > 0 ? customers.Count - 1 : 0;
            this.Clients.All.updateUsersOnlineCount(customerCount);

            //Clients.All.updateUnreadMsgUsersCount(Users);

            return base.OnReconnected();
        }

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    var userName = Context.User.Identity.Name;

        //    if (string.IsNullOrEmpty(userName)) return base.OnDisconnected(stopCalled);
        //    this.db.Users.FirstOrDefault(w => w.Email == userName).OnlineOrOffline = false;
        //    this.db.SaveChanges();

        //    customers = customers.Where(w => w.Item3 != userName).ToList();


        //    foreach (var customer in customers.ToList())
        //    {
        //        var usersOnline = customers?.Where(w => w.Item3 != customer.Item3);
        //        var onlineUsersReadOrUnread = new List<Tuple<string, string, string, bool, string, string>>();
        //        var userId = customer.Item1;
        //        foreach (var toUser in usersOnline)
        //        {
        //            Message msg = null;
        //            msg = this.db.Messages?.ToList().FirstOrDefault(w => (w.FromUserId == toUser.Item1 && w.ToUserId == userId)
        //                                                                 && (w.ReadOrUnread == false));

        //            if (msg != null)
        //            {
        //                onlineUsersReadOrUnread.Add(
        //                    new Tuple<string, string, string, bool, string, string>
        //                        (toUser.Item1, toUser.Item2, toUser.Item3, false, toUser.Item5, toUser.Item6));
        //            }
        //            else
        //            {
        //                onlineUsersReadOrUnread.Add(
        //                    new Tuple<string, string, string, bool, string, string>
        //                        (toUser.Item1, toUser.Item2, toUser.Item3, true, toUser.Item5, toUser.Item6));
        //            }
        //        }

        //        onlineUsersReadOrUnread = onlineUsersReadOrUnread.OrderBy(o => o.Item4).ToList();
        //        this.Clients.User(customer.Item3).onConnected(onlineUsersReadOrUnread);
        //    }

        //    var customerCount = customers.Count > 0 ? customers.Count - 1 : 0;
        //    this.Clients.All.updateUsersOnlineCount(customerCount);

        //    //Clients.All.messagesOfUser(customers.Count);

        //    return base.OnDisconnected(stopCalled);
        //}

        public void ClearOnlineUsers()
        {
            customers = new List<Tuple<string, string, string, bool, string, string>>();

            foreach (var onlineUser in db.Users.Where(w => w.OnlineOrOffline))
            {
                onlineUser.OnlineOrOffline = false;
            }

            this.db.SaveChanges();

            this.Clients.All.clearOnlineUsers();

        }

        public void CheckOnlineUsers()
        {
            var userName = Context.User.Identity.Name;

            if (userName != "")
            {
                var userOnlineNew = customers.FirstOrDefault(w => w.Item3 == userName);
                if (userOnlineNew == null)
                {
                    var userTemp = this.db.Users.FirstOrDefault(f => f.Email == userName);
                    userTemp.OnlineOrOffline = true;
                    this.db.SaveChanges();
                    var fullname = $"{userTemp.FirstName} {userTemp.LastName}";

                    userTemp.ImageUrl = userTemp.ImageUrl != null ? Resources.Pictures.FolderUsersImages + userTemp.ImageUrl :
                                                     (userTemp.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                    customers.Add(new Tuple<string, string, string, bool, string, string>
                        (userTemp.Id, userTemp.Gender, userTemp.Email, userTemp.OnlineOrOffline, fullname, userTemp.ImageUrl));
                }

                if (customers.Count > 0)
                {
                    customers = customers.Distinct().ToList();
                }

                foreach (var customer in customers.ToList())
                {
                    var currentUserName = customer.Item3;
                    var usersOnline = customers?.Where(w => w.Item3 != currentUserName).ToList();
                    var onlineUsersReadOrUnread = new List<Tuple<string, string, string, bool, string, string>>();
                    var userId = customer.Item1;
                    foreach (var userOnline in usersOnline)
                    {
                        Message msg = null;
                        msg = this.db.Messages?.ToList().FirstOrDefault(w => ((w.FromUserId == userOnline.Item1 && w.ToUserId == userId)
                                                                              && (w.ReadOrUnread == false)));

                        if (msg != null)
                        {
                            onlineUsersReadOrUnread.Add(
                                new Tuple<string, string, string, bool, string, string>
                                    (userOnline.Item1, userOnline.Item2, userOnline.Item3, false, userOnline.Item5, userOnline.Item6));
                        }
                        else
                        {
                            onlineUsersReadOrUnread.Add(
                                new Tuple<string, string, string, bool, string, string>
                                    (userOnline.Item1, userOnline.Item2, userOnline.Item3, true, userOnline.Item5, userOnline.Item6));
                        }
                    }



                    onlineUsersReadOrUnread = onlineUsersReadOrUnread.OrderBy(o => o.Item4).ToList();
                    this.Clients.User(customer.Item3).onConnected(onlineUsersReadOrUnread);
                }

                var customerCount = customers.Count > 0 ? customers.Count - 1 : 0;
                this.Clients.All.updateUsersOnlineCount(customerCount);
            }
        }

        public void SendNotification(string message)
        {
            this.Clients.All.receiveNotification(Context.User.Identity.Name, message);
        }

        public void UpdateOrderMessages()
        {
            var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orderMessagesCount = db.Orders.Where(w => w.OrderStatusId == orderStatusId && !w.Deleted).ToList().Count;
            this.Clients.All.getUnreadOrderMessages(orderMessagesCount);
        }

        public void SendUserChat(string toUserId)
        {
            var userName = Context.User.Identity.Name;
            var fromUser = db.Users.FirstOrDefault(w => w.Email == userName);
            var toUser = db.Users.FirstOrDefault(f => f.Id == toUserId);

            if ((fromUser != null) && (toUser != null))
            {
                fromUser.ChattingWithUserId = toUser.Id;
                db.SaveChanges();
            }

            if (toUser == null) return;
            {
                var messages = this.db.Messages?.Where(w => (w.FromUserId == fromUser.Id && w.ToUserId == toUser.Id) ||
                                                       (w.FromUserId == toUser.Id && w.ToUserId == fromUser.Id));

                UserViewModel fromUserModel = new UserViewModel()
                                                  {
                                                      Id = fromUser.Id,
                                                      ConnectionId = fromUser.ConnectionId,
                                                      Email = fromUser.Email,
                                                      FirstName = fromUser.FirstName,
                                                      LastName = fromUser.LastName,
                                                      ImageUrl = toUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + toUser.ImageUrl :
                                                     (toUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman)
                };

                UserViewModel toUserModel = new UserViewModel()
                                                {
                                                    Id = toUser.Id,
                                                    ConnectionId = toUser.ConnectionId,
                                                    Email = toUser.Email,
                                                    FirstName = toUser.FirstName,
                                                    LastName = toUser.LastName,
                                                    ImageUrl = toUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + toUser.ImageUrl :
                                                     (toUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman)
                                                };


                var listMessages = new List<Tuple<string, string, string, bool>>();
                if (messages != null)
                {

                    foreach (var item in messages)
                    {
                        listMessages.Add(new Tuple<string, string, string, bool>
                            (item.Text, item.FromUserId, item.ToUserId, item.ReadOrUnread));
                    }
                }

                this.Clients.Caller.getUserChat(fromUserModel, toUserModel, listMessages);
            }
        }

        public void SendCustomerChat(string toUserEmail)
        {
            var userName = Context.User.Identity.Name;
            var fromUser = db.Users.FirstOrDefault(w => w.Email == userName);
            var toUser = db.Users.FirstOrDefault(f => f.Email == toUserEmail);

            if ((fromUser != null) && (toUser != null))
            {
                fromUser.ChattingWithUserId = toUser.Id;
                db.SaveChanges();
            }

            if (toUser == null) return;
            {
                var messages = this.db.Messages?.Where(w => (w.FromUserId == fromUser.Id && w.ToUserId == toUser.Id) ||
                                                       (w.FromUserId == toUser.Id && w.ToUserId == fromUser.Id));

                UserViewModel fromUserModel = new UserViewModel()
                {
                    Id = fromUser.Id,
                    ConnectionId = fromUser.ConnectionId,
                    Email = fromUser.Email,
                    FirstName = fromUser.FirstName,
                    LastName = fromUser.LastName,
                    ImageUrl = toUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + toUser.ImageUrl :
                                                     (toUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman)
                };

                UserViewModel toUserModel = new UserViewModel()
                {
                    Id = toUser.Id,
                    ConnectionId = toUser.ConnectionId,
                    Email = toUser.Email,
                    FirstName = toUser.FirstName,
                    LastName = toUser.LastName,
                    ImageUrl = toUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + toUser.ImageUrl :
                                                     (toUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman)
                };


                var listMessages = new List<Tuple<string, string, string, bool>>();
                if (messages != null)
                {

                    foreach (var item in messages)
                    {
                        listMessages.Add(new Tuple<string, string, string, bool>
                            (item.Text, item.FromUserId, item.ToUserId, item.ReadOrUnread));
                    }
                }

                this.Clients.Caller.getUserChat(fromUserModel, toUserModel, listMessages);
            }
        }

        public void SendUserOrderRead(string messageId, bool deleteOrNot)
        {
            var userName = Context.User.Identity.Name;
            var orderMessageId = int.Parse(messageId);
            var messageUpdate = db.OrderMessages.FirstOrDefault(w => w.Id == orderMessageId);
            var user = db.Users.FirstOrDefault(w => w.Email == userName);

            if (deleteOrNot)
            {
                messageUpdate.Deleted = true;
            }
            else
            {
                messageUpdate.ReadOrUnread = true;
            }

            db.SaveChanges();

            var messages = db.Messages.Where(w => w.ToUserId == user.Id).ToList();
            var messagesCount = 0;

            var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orders = db.Orders.Where(w => w.UserId == user.Id && w.OrderStatusId != orderStatusId).ToList();

            var orderMessagesCount = 0;
            foreach (var order in orders)
            {
                orderMessagesCount += order.OrderMessages.Count(f => (f.ReadOrUnread == false) && f.Deleted == false);
            }

            foreach (var everyUser in db.Users?.ToList())
            {
                var message = messages.LastOrDefault(w => w.FromUserId == everyUser.Id);

                if (message != null && message.DeletedMessageFirst != user.Id && message.DeletedMessageSecond != user.Id)
                {
                    if (message.ReadOrUnread == false)
                    {
                        messagesCount++;
                    }
                }
            }

            messagesCount += orderMessagesCount;

            this.Clients.User(user.Email).getUnreadUserMessages(messagesCount);
        }

        public void SendChatMessage(string toUserId, string message)
        {
            var userName = Context.User.Identity.Name;
            var toUser = db.Users.FirstOrDefault(f => f.Id == toUserId);
            var fromUser = db.Users.FirstOrDefault(f => f.Email == userName);

            if (toUser != null)
            {
                Message msg = new Message
                                  {
                                      Text = message,
                                      DateSentMessage = DateTime.Now,
                                      FromUserId = fromUser.Id,
                                      ToUserId = toUser.Id,
                                      ReadOrUnread = false
                                  };
                db.Messages.Add(msg);
                db.SaveChanges();
            }

            var messages = db.Messages.Where(w => w.ToUserId == toUser.Id).ToList();
            var messagesCount = 0;

            foreach (var everyUser in db.Users?.ToList())
            {
                var messageInside = messages.LastOrDefault(w => w.FromUserId == everyUser.Id);

                if (messageInside != null && messageInside.DeletedMessageFirst != fromUser.Id && messageInside.DeletedMessageSecond != fromUser.Id)
                {
                    if (messageInside.ReadOrUnread == false)
                    {
                        messagesCount++;
                    }
                }
            }

            var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orders = db.Orders.Where(w => w.UserId == toUser.Id && w.OrderStatusId != orderStatusId).ToList();

            var orderMessagesCount = 0;
            foreach (var order in orders)
            {
                orderMessagesCount += order.OrderMessages.Count(f => (f.ReadOrUnread == false) && (f.Deleted == false));
            }

            messagesCount += orderMessagesCount;
            

            this.Clients.User(toUser.UserName).getUnreadUserMessages(messagesCount);
            //Clients.All.updateUnreadMsgUsersCount(Users);

            if (toUser.ChattingWithUserId == fromUser.Id)
            {
                UserViewModel fromUserVM = new UserViewModel()
                { 
                    Email = fromUser.Email,
                    ImageUrl = fromUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + fromUser.ImageUrl :
                                                     (fromUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman)
                };
                this.Clients.User(toUser.UserName).receiveChatMessage(fromUserVM, message);
            }

            var usersOnline = customers?.Where(w => w.Item3 != toUser.Email).ToList();
            var onlineUsersReadOrUnread = new List<Tuple<string, string, string, bool, string, string>>();
            var userId = toUser.Id;
            foreach (var toUserTemp in usersOnline)
            {
                Message msg = null;
                msg = db.Messages?.FirstOrDefault(w => w.FromUserId == toUserTemp.Item1 && w.ToUserId == userId
                                                                                        && (w.ReadOrUnread == false));

                if (msg != null)
                {
                    onlineUsersReadOrUnread.Add(
                    new Tuple<string, string, string, bool, string, string>
                    (toUserTemp.Item1, toUserTemp.Item2, toUserTemp.Item3, false, toUserTemp.Item5, toUserTemp.Item6));
                }
                else
                {
                    onlineUsersReadOrUnread.Add(
                    new Tuple<string, string, string, bool, string, string>
                    (toUserTemp.Item1, toUserTemp.Item2, toUserTemp.Item3, true, toUserTemp.Item5, toUserTemp.Item6));
                }
            }

            onlineUsersReadOrUnread = onlineUsersReadOrUnread.OrderBy(o => o.Item4).ToList();
            Clients.User(toUser.Email).onConnected(onlineUsersReadOrUnread);
        }

        public void CloseChat()
        {
            var userName = Context.User.Identity.Name;
            var user = db.Users?.First(f => f.Email == userName);
            user.ChattingWithUserId = null;
            db.SaveChanges();
        }

        public void SendMessageAsRead(string toUserId)
        {
            var userName = Context.User.Identity.Name;
            var toUser = db.Users.FirstOrDefault(f => f.Id == toUserId);
            var fromUser = db.Users.FirstOrDefault(f => f.Email == userName);
            var msgs = db.Messages.Where(w => w.FromUserId == toUser.Id && w.ToUserId == fromUser.Id);

            var messages = db.Messages?.Where(w => w.ToUserId == fromUser.Id).ToList();
            var messagesCount = 0;

            foreach (var item in msgs)
            {
                item.ReadOrUnread = true;
            }

            db.SaveChanges();

            var usersOnline = customers?.Where(w => w.Item3 != userName);
            var onlineUsersReadOrUnread = new List<Tuple<string, string, string, bool, string, string>>();
            string userId = fromUser.Id;
            foreach (var toUserTemp in usersOnline)
            {
                Message msg = null;
                msg = db.Messages?.FirstOrDefault(w => ((w.FromUserId == toUserTemp.Item1 && w.ToUserId == userId)
                && (w.ReadOrUnread == false)));

                if (msg != null)
                {
                    onlineUsersReadOrUnread.Add(
                    new Tuple<string, string, string, bool, string, string>
                    (toUserTemp.Item1, toUserTemp.Item2, toUserTemp.Item3, false, toUserTemp.Item5, toUserTemp.Item6));
                }
                else
                {
                    onlineUsersReadOrUnread.Add(
                    new Tuple<string, string, string, bool, string, string>
                    (toUserTemp.Item1, toUserTemp.Item2, toUserTemp.Item3, true, toUserTemp.Item5, toUserTemp.Item6));
                }
            }

            onlineUsersReadOrUnread = onlineUsersReadOrUnread.OrderBy(o => o.Item4).ToList();
            Clients.User(userName).onConnected(onlineUsersReadOrUnread);

            foreach (var everyUser in db.Users?.ToList())
            {
                var message = messages.LastOrDefault(w => w.FromUserId == everyUser.Id);

                if (message?.ReadOrUnread == false)
                {
                    messagesCount++;
                }
            }

            var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orders = db.Orders?.Where(w => w.UserId == fromUser.Id && w.OrderStatusId != orderStatusId).ToList();

            var orderMessagesCount = 0;
            foreach (var order in orders)
            {
                orderMessagesCount += order.OrderMessages.Count(f => (f.ReadOrUnread == false) && (f.Deleted == false));
            }

            messagesCount += orderMessagesCount;

            this.Clients.Caller.getUnreadUserMessages(messagesCount);

        }

        public void MessagesOfUser()
        {
            var userName = Context.User.Identity.Name;
            var listMessagesOfUser = new List<Tuple<string, string, string, bool, string, Tuple<bool, bool>, DateTime>>();
            var user = db.Users.FirstOrDefault(w => w.Email == userName);
            var messages = db.Messages?.Where(w => w.ToUserId == user.Id).ToList();
            var messagesTo = db.Messages?.Where(w => w.FromUserId == user.Id).ToList();

            foreach (var everyUser in db.Users.ToList())
            {
                var message = messages?.LastOrDefault(w => w.FromUserId == everyUser.Id);
                var messageTo = messagesTo?.LastOrDefault(w => w.ToUserId == everyUser.Id);


                if ((message != null) && (messageTo != null))
                {
                    if ((message.DeletedMessageFirst != user.Id && message.DeletedMessageSecond != user.Id) ||
                    (messageTo.DeletedMessageFirst != user.Id && messageTo.DeletedMessageSecond != user.Id))
                    {
                        var result = DateTime.Compare(message.DateSentMessage, messageTo.DateSentMessage);
                        if (result > 0)
                        {
                            var gender = everyUser.Gender == "0" ? true : false;
                            var day = message.DateSentMessage.Day < 10 ? "0" +
                            message.DateSentMessage.Day : message.DateSentMessage.Day.ToString();

                            var month = message.DateSentMessage.Month < 10 ? "0" +
                            message.DateSentMessage.Month : message.DateSentMessage.Month.ToString();

                            var year = message.DateSentMessage.Year.ToString();

                            var hour = message.DateSentMessage.Hour < 10 ? "0" +
                            message.DateSentMessage.Hour : message.DateSentMessage.Hour.ToString();

                            var minute = message.DateSentMessage.Minute < 10 ? "0" +
                            message.DateSentMessage.Minute: message.DateSentMessage.Minute.ToString();

                            var fullname = $"{everyUser.FirstName} {everyUser.LastName}";
                            var messageSent = $"{day}.{month}.{year} {hour}:{minute}";


                            everyUser.ImageUrl = everyUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + everyUser.ImageUrl :
                                                     (everyUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                            listMessagesOfUser.Add(new Tuple<string, string, string, bool, string, Tuple<bool, bool>, DateTime>
                            (everyUser.Id, fullname, messageSent, message.ReadOrUnread, everyUser.ImageUrl,
                            new Tuple<bool, bool>(gender, false), message.DateSentMessage));
                        }
                        else
                        {
                            var gender = everyUser.Gender == "0" ? true : false;
                            var day = messageTo.DateSentMessage.Day < 10 ? "0" +
                            messageTo.DateSentMessage.Day : messageTo.DateSentMessage.Day.ToString();

                            var month = messageTo.DateSentMessage.Month < 10 ? "0" +
                            messageTo.DateSentMessage.Month : messageTo.DateSentMessage.Month.ToString();

                            var year = messageTo.DateSentMessage.Year.ToString();

                            var hour = messageTo.DateSentMessage.Hour < 10 ? "0" +
                            messageTo.DateSentMessage.Hour : messageTo.DateSentMessage.Hour.ToString();

                            var minute = messageTo.DateSentMessage.Minute < 10 ? "0" +
                            messageTo.DateSentMessage.Minute : messageTo.DateSentMessage.Minute.ToString();

                            var fullname = $"{everyUser.FirstName} {everyUser.LastName}";
                            var messageSent = $"{day}.{month}.{year} {hour}:{minute}";

                          
                            everyUser.ImageUrl = everyUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + everyUser.ImageUrl :
                                                     (everyUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                                listMessagesOfUser.Add(new Tuple<string, string, string, bool, string, Tuple<bool, bool>, DateTime>
                            (everyUser.Id, fullname, messageSent, true, everyUser.ImageUrl,
                            new Tuple<bool, bool>(gender, false), messageTo.DateSentMessage));
                        }
                    }
                }
                else
                {
                    if (message != null)
                    {
                        if (message.DeletedMessageFirst != user.Id && message.DeletedMessageSecond != user.Id)
                        {
                            var gender = everyUser.Gender == "0" ? true : false;
                            var day = message.DateSentMessage.Day < 10 ? "0" +
                            message.DateSentMessage.Day : message.DateSentMessage.Day.ToString();

                            var month = message.DateSentMessage.Month < 10 ? "0" +
                            message.DateSentMessage.Month : message.DateSentMessage.Month.ToString();

                            var year = message.DateSentMessage.Year.ToString();

                            var hour = message.DateSentMessage.Hour < 10 ? "0" +
                            message.DateSentMessage.Hour : message.DateSentMessage.Hour.ToString();

                            var minute = message.DateSentMessage.Minute < 10 ? "0" +
                            message.DateSentMessage.Minute : message.DateSentMessage.Minute.ToString();

                            var fullname = $"{everyUser.FirstName} {everyUser.LastName}";
                            var messageSent = $"{day}.{month}.{year} {hour}:{minute}";

                        
                            everyUser.ImageUrl = everyUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + everyUser.ImageUrl :
                                                     (everyUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                                listMessagesOfUser.Add(new Tuple<string, string, string, bool, string, Tuple<bool, bool>, DateTime>
                            (everyUser.Id, fullname, messageSent, message.ReadOrUnread, everyUser.ImageUrl,
                            new Tuple<bool, bool>(gender, false), message.DateSentMessage));
                        }
                    }
                    if (messageTo != null)
                    {
                        if (messageTo.DeletedMessageFirst != user.Id && messageTo.DeletedMessageSecond != user.Id)
                        {
                            var gender = everyUser.Gender == "0" ? true : false;
                            var day = messageTo.DateSentMessage.Day < 10 ? "0" +
                            messageTo.DateSentMessage.Day : messageTo.DateSentMessage.Day.ToString();

                            var month = messageTo.DateSentMessage.Month < 10 ? "0" +
                            messageTo.DateSentMessage.Month : messageTo.DateSentMessage.Month.ToString();

                            var year = messageTo.DateSentMessage.Year.ToString();

                            var hour = messageTo.DateSentMessage.Hour < 10 ? "0" +
                            messageTo.DateSentMessage.Hour : messageTo.DateSentMessage.Hour.ToString();

                            var minute = messageTo.DateSentMessage.Minute < 10 ? "0" +
                            messageTo.DateSentMessage.Minute : messageTo.DateSentMessage.Minute.ToString();

                            var fullname = $"{everyUser.FirstName} {everyUser.LastName}";
                            var messageSent = $"{day}.{month}.{year} {hour}:{minute}";

                       
                            everyUser.ImageUrl = everyUser.ImageUrl != null ? Resources.Pictures.FolderUsersImages + everyUser.ImageUrl :
                                                     (everyUser.Gender == "0" ? Resources.Pictures.ProfilePic : Resources.Pictures.ProfileWoman);

                            listMessagesOfUser.Add(new Tuple<string, string, string, bool, string, Tuple<bool, bool>, DateTime>
                            (everyUser.Id, fullname, messageSent, true, everyUser.ImageUrl,
                            new Tuple<bool, bool>(gender, false), messageTo.DateSentMessage));
                        }
                    }
                }
            }

            var orderStatusId = db.OrderStatuses?.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orders = db.Orders?.Where(w => w.UserId == user.Id && w.OrderStatusId != orderStatusId).ToList();

            foreach (var order in orders)
            {
                foreach (var message in order.OrderMessages.Where(w => w.Deleted == false))
                {
                    listMessagesOfUser.Add(new Tuple<string, string, string, bool, string, Tuple<bool, bool>, DateTime>
                            (message.Id.ToString(), message.Message, message.Added.ToString("MM.dd.yyyy - HH:mm"),
                            message.ReadOrUnread, "", new Tuple<bool, bool>(true, true), message.Added));
                }
            }

            listMessagesOfUser = listMessagesOfUser.OrderByDescending(o => o.Item7).ToList();
            this.Clients.Caller.messagesOfUsers(listMessagesOfUser);

        }

        public void DeleteChatMessage(string toUserId)
        {
            var userName = Context.User.Identity.Name;
            var user = db.Users?.Where(w => w.Email == userName).FirstOrDefault();
            var messages = db.Messages?.Where(w => w.ToUserId == user.Id).ToList();
            var messagesTo = db.Messages?.Where(w => w.FromUserId == user.Id).ToList();


            var message = messages?.Where(w => w.FromUserId == toUserId).LastOrDefault();
            var messageTo = messagesTo?.Where(w => w.ToUserId == toUserId).LastOrDefault();

            try
            {
                if (string.IsNullOrEmpty(message.DeletedMessageFirst))
                {
                    message.DeletedMessageFirst = user.Id;
                }

                if (!string.IsNullOrEmpty(message.DeletedMessageFirst) && string.IsNullOrEmpty(message.DeletedMessageSecond))
                {
                    if (message.DeletedMessageFirst != user.Id)
                    {
                        message.DeletedMessageSecond = user.Id;
                    }
                }

                db.SaveChanges();
            }
            catch
            {
                // ignored
            }

            try
            {
                if (string.IsNullOrEmpty(messageTo.DeletedMessageFirst))
                {
                    messageTo.DeletedMessageFirst = user.Id;
                }

                if (!string.IsNullOrEmpty(messageTo.DeletedMessageFirst) && string.IsNullOrEmpty(messageTo.DeletedMessageSecond))
                {
                    if (messageTo.DeletedMessageFirst != user.Id)
                    {
                        messageTo.DeletedMessageSecond = user.Id;
                    }
                }

                db.SaveChanges();
            }
            catch
            {
                // ignored
            }

            var messagesCount = 0;
            var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orders = db.Orders.Where(w => w.UserId == user.Id && w.OrderStatusId != orderStatusId)?.ToList();
            var orderMessagesCount = 0;

            foreach (var order in orders)
            {
                orderMessagesCount += order.OrderMessages.Count(f => (f.ReadOrUnread == false) && (f.Deleted == false));
            }

            foreach (var everyUser in db.Users?.ToList())
            {
                var messageItem = messages?.LastOrDefault(w => w.FromUserId == everyUser.Id);

                if (messageItem != null && messageItem.DeletedMessageFirst != user.Id && messageItem.DeletedMessageSecond != user.Id && messageItem.ReadOrUnread == false)
                {
                    messagesCount++;
                }
            }

            messagesCount += orderMessagesCount;

            this.Clients.User(user.Email).getUnreadUserMessages(messagesCount);
        }

        public void SendUserMessages()
        {
            try
            {
                var userName = Context.User.Identity.Name;

                if(userName != "")
                {
                    var user = db.Users.FirstOrDefault(w => w.Email == userName);
                    var messages = db.Messages.Where(w => w.ToUserId == user.Id).ToList();
                    var messagesCount = 0;

                    var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
                    var orders = db.Orders.Where(w => w.UserId == user.Id && w.OrderStatusId != orderStatusId).ToList();

                    var orderMessagesCount = 0;
                    foreach (var order in orders)
                    {
                        orderMessagesCount += order.OrderMessages.Count(f => (f.ReadOrUnread == false) && f.Deleted == false);
                    }

                    foreach (var everyUser in db.Users?.ToList())
                    {
                        var message = messages.LastOrDefault(w => w.FromUserId == everyUser.Id);

                        if (message == null || message.DeletedMessageFirst == user.Id
                                            || message.DeletedMessageSecond == user.Id) continue;
                        if (message.ReadOrUnread == false)
                        {
                            messagesCount++;
                        }
                    }

                    messagesCount += orderMessagesCount;

                    this.Clients.User(user.Email).getUnreadUserMessages(messagesCount);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void UpdateMessagesCount(int orderId)
        {

            var orderUser = db.Orders?.FirstOrDefault(w => w.Id == orderId);
            var user = db.Users.FirstOrDefault(w => w.Id == orderUser.UserId);
            var messages = db.Messages?.Where(w => w.ToUserId == orderUser.UserId).ToList();
            var messagesCount = 0;

            var orderStatusId = db.OrderStatuses.FirstOrDefault(f => f.Name == "waiting")?.Id;
            var orders = db.Orders?.Where(w => w.UserId == orderUser.UserId && w.OrderStatusId != orderStatusId).ToList();

            var orderMessagesCount = 0;
            foreach (var order in orders)
            {
                orderMessagesCount += order.OrderMessages.Count(f => f.ReadOrUnread == false && (f.Deleted == false));
            }

            foreach (var everyUser in db.Users.ToList())
            {
                var message = messages?.LastOrDefault(w => w.FromUserId == everyUser.Id);

                if (message != null && message.DeletedMessageFirst != user.Id && message.DeletedMessageSecond != user.Id)
                {
                    if (message.ReadOrUnread == false)
                    {
                        messagesCount++;
                    }
                }
            }

            messagesCount += orderMessagesCount;

            this.Clients.User(user.Email).getUnreadUserMessages(messagesCount);
        }
    }
}