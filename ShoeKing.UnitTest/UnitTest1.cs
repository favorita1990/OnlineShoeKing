using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnlineShoeKing.Controllers;

namespace ShoeKing.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void HomeIndex()
        {
            HomeController controller = new HomeController();

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HomeAbout()
        {
            HomeController controller = new HomeController();

            ViewResult result = controller.About() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CustomersIndex()
        {
            CustomersController controller = new CustomersController();

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CustomersProfileIndex()
        {
            CustomersController controller = new CustomersController();

            ViewResult result = controller.ProfileIndex("founder1990@abv.bg") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProductsCategory()
        {
            ProductsController controller = new ProductsController();

            ViewResult result = controller.Category(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProductsProduct()
        {
            ProductsController controller = new ProductsController();

            ViewResult result = controller.Product(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProfileIndex()
        {
            ProfileController controller = new ProfileController();

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProfilePhotos()
        {
            ProfileController controller = new ProfileController();

            ViewResult result = controller.Photos() as ViewResult;

            Assert.IsNotNull(result);
        }

    }
}
