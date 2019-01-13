
namespace OnlineShoeKing
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //emoticons
            bundles.Add(new StyleBundle("~/Content/emoticonsCSS").Include(
                      "~/Content/support/skype/emoticons.css"));

            bundles.Add(new ScriptBundle("~/bundles/emoticonsHead").Include(
                     "~/Scripts/emoticons.js"));

            bundles.Add(new ScriptBundle("~/bundles/emoticonsFooter").Include(
                     "~/Scripts/emoticonsChat.js"));


            bundles.Add(new ScriptBundle("~/js/autocomplete").Include("~/Scripts/jquery-ui-1.12.1.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/kendoStyles").Include(
                "~/Content/kendo/kendo.common.min.css",
                "~/Content/kendo/kendo.default.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryKendo").Include(
                "~/Scripts/kendo/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo/kendo.web.min.js",
                "~/Scripts/jquery-ui-1.12.1.js",
                // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
                "~/Scripts/kendo/kendo.aspnetmvc.min.js"));  //after jquery

            bundles.Add(new ScriptBundle("~/bundles/jqueryunob").Include(
                "~/Scripts/jquery.unobtrusive*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //Just copy -->
            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                "~/Scripts/jquery.signalR-*"));
            //@Scripts.Render("~/bundles/signalr")
            //<script src = "/signalr/hubs"></ script>

            bundles.IgnoreList.Clear();
        }
    }
}
