

namespace OnlineShoeKing.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using Context;
    using Models;
    using Models.ViewModels;

    public class OrderDetailRepository
    {
        private DBContext db = new DBContext();

        public void create(OrderDetail orderDetail)
        {
            db.OrderDetails.Add(orderDetail);
            db.SaveChanges();
        }

        public OrderDetailViewModel Map(OrderDetail orderDetail)
        {

            OrderDetailViewModel newOrderDetail = new OrderDetailViewModel()
            {
                OrderDetailsId = orderDetail.Id,
                OrderId = orderDetail.OrderId,
                Price = orderDetail.Price,
                ProductId = orderDetail.ProductId,
                Quantity = orderDetail.Quantity,
                Specials = orderDetail.Specials,
                Status = orderDetail.Status
            };

            return newOrderDetail;
        }

        public List<OrderDetailViewModel> GetAllOrderDetailsGrid()
        {
            List<OrderDetailViewModel> allOrders = new List<OrderDetailViewModel>();

            foreach (var orderDetail in db.OrderDetails.ToList())
            {
                allOrders.Add(Map(orderDetail));
            }

            return allOrders;
        }

        public List<OrderDetailViewModel> GetOrderDetailsGridByOrderId(int orderId)
        {
            List<OrderDetailViewModel> allOrders = new List<OrderDetailViewModel>();

            foreach (var orderDetail in db.OrderDetails.Where(w => w.OrderId == orderId).ToList())
            {
                allOrders.Add(Map(orderDetail));
            }

            return allOrders;
        }
    }
}