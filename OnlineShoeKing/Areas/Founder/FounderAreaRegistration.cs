using System.Web.Mvc;

namespace OnlineShoeKing.Areas.Founder
{
    public class FounderAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Founder";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Founder_default",
                "Founder/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}