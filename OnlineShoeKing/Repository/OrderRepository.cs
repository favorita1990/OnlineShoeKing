

namespace OnlineShoeKing.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Context;
    using Models;
    using Models.ViewModels;

    public class OrderRepository
    {
        public DBContext db = new DBContext();
        public UserRepository userRepository = new UserRepository();

        public void create(Order order)
        {
            List<int> order1 = db.Orders.Select(e => e.Id).ToList();
            order.Id = order1.Count;
            db.Orders.Add(order);
            db.SaveChanges();
        }
        
        public OrderViewModel Map(Order order)
        {
            UserModel admin = userRepository.GetUserById(order.AdminId);
            UserModel user = userRepository.GetUserById(order.UserId);
            string adminName = "";
            try
            {
                adminName = $"{admin.FirstName} {admin.LastName}";
            }
            catch
            {
                // ignored
            }

            string username = "";
            try
            {
                username = $"{user.FirstName} {user.LastName}";
            }
            catch
            {
                // ignored
            }

            var orderStatus = db.OrderStatuses.FirstOrDefault(f => f.Id == order.OrderStatusId).Name;

            OrderViewModel newOrder = new OrderViewModel()
            {
                OrderId = order.Id,
                Address = order.Address,
                AdminId = order.AdminId,
                AdminName = adminName,
                UserId = order.UserId,
                UserName = username,
                City = order.City,
                Created = order.Created.Value.ToString("MM.dd.yyyy - HH:mm"),
                Updated = order.Updated.Value.ToString("MM.dd.yyyy - HH:mm"),
                PhoneNumber = order.PhoneNumber,
                OrderStatus = orderStatus,
                PostOffice = order.PostOffice,
                Total = order.Total,
                Deleted = order.Deleted
            };

            return newOrder;
        }

        public List<OrderViewModel> GetAllOrdersGrid()
        {
            List<OrderViewModel> allOrders = new List<OrderViewModel>();

            foreach (var order in db.Orders.Where(w => w.Deleted == false).ToList())
            {
                allOrders.Add(Map(order));
            }

            return allOrders;
        }

        public List<OrderViewModel> GetAllDeletedOrdersGrid()
        {
            List<OrderViewModel> allOrders = new List<OrderViewModel>();

            foreach (var order in db.Orders.Where(w => w.Deleted).ToList())
            {
                allOrders.Add(Map(order));
            }

            return allOrders;
        }

        public void DeleteOrderById(int orderId)
        {
            string currentUserName = HttpContext.Current.User.Identity.Name;

            var order = db.Orders.FirstOrDefault(w => w.Id == orderId);
            order.Deleted = true;
            order.AdminId = userRepository.UserId(currentUserName);

            foreach (var detail in order.OrderDetails.ToList())
            {
                var size = db.SizeOfProducts.Find(detail.SizeId);
                size.Quantity++;
            }

            foreach (var message in order.OrderMessages.ToList())
            {
                message.Deleted = true;
            }

            db.SaveChanges();
        }

        public string AcceptOrderById(int orderId)
        {
            string currentUserName = HttpContext.Current.User.Identity.Name;

            var order = db.Orders.FirstOrDefault(w => w.Id == orderId);
            
            if(order.Deleted == false)
            {
                var admin = userRepository.GetUserByEmail(currentUserName);
                order.AdminId = admin.Id;
                order.OrderStatusId = db.OrderStatuses.FirstOrDefault(w => w.Name == "accepted").Id;

                OrderMessage orderMessage = new OrderMessage()
                {
                    Added = DateTime.Now,
                    Deleted = false,
                    OrderId = order.Id,
                    ReadOrUnread = false,
                    Message = string.Format(Resources.Language.MessageOrderWasAcceptedAt, 
                    order.Id.ToString(), DateTime.Now.ToString("MM.dd.yyyy - HH:mm"), order.Total.ToString(), admin.FirstName, admin.LastName)
                };

                db.OrderMessages.Add(orderMessage);
                db.SaveChanges();

                return Resources.Language.MessageOrderWasAccepted;
            }
            else
            {
                return Resources.Language.MessageOrderBeenDeleted;
            }
        }

        public string CanceledOrderById(int orderId)
        {
            var currentUserName = HttpContext.Current.User.Identity.Name;

            var order = db.Orders.FirstOrDefault(w => w.Id == orderId);

            if (order.Deleted == false)
            {
                var admin = userRepository.GetUserByEmail(currentUserName);
                order.AdminId = admin.Id;
                order.OrderStatusId = db.OrderStatuses.FirstOrDefault(w => w.Name == "canceled").Id;

                OrderMessage orderMessage = new OrderMessage()
                {
                    Added = DateTime.Now,
                    Deleted = false,
                    OrderId = order.Id,
                    ReadOrUnread = false,
                    Message = string.Format(Resources.Language.MessageOrderWasCanceledAt,
                    order.Id.ToString(), DateTime.Now.ToString("MM.dd.yyyy - HH:mm"), admin.FirstName, admin.LastName)
                };

                db.OrderMessages.Add(orderMessage);
                db.SaveChanges();

                return Resources.Language.MessageOrderWasCanceled;
            }
            else
            {
                return Resources.Language.MessageOrderBeenDeleted;
            }
        }

        public string ResendOrderById(int orderId, string message)
        {
            var currentUserName = HttpContext.Current.User.Identity.Name;

            var order = db.Orders.FirstOrDefault(w => w.Id == orderId);

            if (order.Deleted == false)
            {
                var admin = userRepository.GetUserByEmail(currentUserName);
                order.AdminId = admin.Id;
                order.OrderStatusId = db.OrderStatuses.FirstOrDefault(w => w.Name == "resend").Id;

                OrderMessage orderMessage = new OrderMessage()
                {
                    Added = DateTime.Now,
                    Deleted = false,
                    OrderId = order.Id,
                    ReadOrUnread = false,
                    Message = string.Format(Resources.Language.MessageOrderNo,
                    order.Id.ToString(), DateTime.Now.ToString("MM.dd.yyyy - HH:mm"), message)
                };

                db.OrderMessages.Add(orderMessage);
                db.SaveChanges();

                return Resources.Language.MessageOrderMessageWasSent;
            }
            else
            {
                return Resources.Language.MessageOrderBeenDeleted;
            }
        }
    }
}