using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;

            //assert
            Product[] prod = result.Products.ToArray();
            Assert.IsTrue(prod.Length == 2);
            Assert.AreEqual(prod[0].Name, "P4");
            Assert.AreEqual(prod[1].Name, "P5");

        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //arrange
            HtmlHelper htmlHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                ItemsPerPage = 3,
                TotalItems = 28
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //act
            MvcHtmlString result = htmlHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
       + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(2).Model;


            //Assert
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage,2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);

        }
    }
}
