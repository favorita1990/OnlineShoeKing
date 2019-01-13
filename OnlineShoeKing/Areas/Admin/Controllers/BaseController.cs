

namespace OnlineShoeKing.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Context;

    public class BaseController : Controller
    {
        public DBContext db { get; set; }

        public BaseController()
            : this(new DBContext())
        {}

        public BaseController(DBContext db)
        { this.db = db; }
    }
}