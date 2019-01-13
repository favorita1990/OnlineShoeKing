

namespace OnlineShoeKing.Repository
{
    using Models.ViewModels;

    public class BreadCrumbRepository
    {

        public BreadCrumbVm AddBreadCrumb(string controller, string controllerName, string controllerPartial, string action, string actionName, string actionPartial, string idName)
        {
            var breadCrumb = new BreadCrumbVm()
            {
                Controller = controller,
                ControllerName = controllerName,
                ControllerPartial = controllerPartial,
                Action = action,
                ActionName = actionName,
                ActionPartial = actionPartial,
                IdName = idName
            };

            return breadCrumb;
        }

        public BreadCrumbVm AddBreadCrumb(string controller, string controllerName, string controllerPartial, string action, string actionName, 
            string actionPartial, string actionId, string idName)
        {
            var breadCrumb = new BreadCrumbVm()
            {
                Controller = controller,
                ControllerName = controllerName,
                ControllerPartial = controllerPartial,
                Action = action,
                ActionName = actionName,
                ActionPartial = actionPartial,
                ActionId = actionId,
                IdName = idName
            };

            return breadCrumb;
        }
    }
}