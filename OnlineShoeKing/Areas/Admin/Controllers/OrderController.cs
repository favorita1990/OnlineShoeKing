
namespace OnlineShoeKing.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;
    using Context;
    using Models;
    using Models.ViewModels;
    using Repository;


    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        public DBContext db = new DBContext();
        private readonly OrderRepository orderRepository = new OrderRepository();
        private readonly ProductRepository productRepository = new ProductRepository();
        private readonly OrderDetailRepository orderDetailsRepository = new OrderDetailRepository();

        [HttpPost]
        public ActionResult GetAllOrders([DataSourceRequest]DataSourceRequest request)
        {

            IEnumerable<OrderViewModel> allOrders = new List<OrderViewModel>();

            allOrders = orderRepository.GetAllOrdersGrid();

            return Json(allOrders.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetAllDeletedOrders([DataSourceRequest]DataSourceRequest request)
        {

            IEnumerable<OrderViewModel> allOrders = new List<OrderViewModel>();

            allOrders = orderRepository.GetAllDeletedOrdersGrid();

            return Json(allOrders.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetOrderDetailsByOrderId([DataSourceRequest]DataSourceRequest request, int orderId)
        {
            IEnumerable<OrderDetailViewModel> orderDetails = new List<OrderDetailViewModel>();

            orderDetails = orderDetailsRepository.GetOrderDetailsGridByOrderId(orderId);

            return Json(orderDetails.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult AcceptOrderById([DataSourceRequest]DataSourceRequest request, int orderId)
        {

            var answer = orderRepository.AcceptOrderById(orderId);

            return Json(answer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CancelOrderById([DataSourceRequest]DataSourceRequest request, int orderId)
        {

            var answer = orderRepository.CanceledOrderById(orderId);

            return Json(answer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ResendOrderById([DataSourceRequest]DataSourceRequest request, int orderId, string message)
        {

            var answer = orderRepository.ResendOrderById(orderId, message);

            return Json(answer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetProductsByOrderId([DataSourceRequest]DataSourceRequest request, int orderId)
        {
            List<ProductViewModel> products = new List<ProductViewModel>();

            products = productRepository.GetProductsByOrderId(orderId);

            return Json(products.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult DeleteOrderById([DataSourceRequest]DataSourceRequest request, int orderId)
        {

            orderRepository.DeleteOrderById(orderId);

            return Json("success".ToDataSourceResult(request));
        }

        // GET: Founder/Order
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("IndexPartial")]
        public PartialViewResult _Index()
        {
            IEnumerable<OrderViewModel> allOrders = new List<OrderViewModel>();

            allOrders = orderRepository.GetAllOrdersGrid();

            return PartialView("_Index", allOrders);
        }

        [HttpPost]
        [ActionName("OrderDeletedGrid")]
        public PartialViewResult _OrderDeletedGrid()
        {

            IEnumerable<OrderViewModel> allOrders = new List<OrderViewModel>();

            allOrders = orderRepository.GetAllDeletedOrdersGrid();

            return PartialView("_OrderDeletedGrid", allOrders);
        }
    }
}