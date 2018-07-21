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
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

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
                ItemsPerPage = 10,
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
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;


            //Assert
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);

        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category="cat1"},
                new Product {ProductID = 2, Name = "P2", Category="cat2"},
                new Product {ProductID = 3, Name = "P3", Category="cat1"},
                new Product {ProductID = 4, Name = "P4", Category="cat3"},
                new Product {ProductID = 5, Name = "P5", Category="cat2"}
            });
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //act
            var result = ((ProductsListViewModel)controller.List("cat2").Model).Products.ToArray();

            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "cat2");
            Assert.IsTrue(result[1].Name == "P5" && result[1].Category == "cat2");

        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category="Apples"},
                new Product {ProductID = 2, Name = "P2", Category="Plums"},
                new Product {ProductID = 3, Name = "P3", Category="Plums"},
                new Product {ProductID = 4, Name = "P4", Category="Apples"},
                new Product {ProductID = 5, Name = "P5", Category="Oranges"}
            });

            NavController target = new NavController(mock.Object);

            //act
            var results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");

        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category="Apples"},
                new Product {ProductID = 2, Name = "P2", Category="Plums"},
                new Product {ProductID = 3, Name = "P3", Category="Plums"},
                new Product {ProductID = 4, Name = "P4", Category="Apples"},
                new Product {ProductID = 5, Name = "P5", Category="Oranges"}
            });

            NavController target = new NavController(mock.Object);

            //act
            var results = target.Menu("Plums").ViewBag.SelectedCategory;

            //assert
            Assert.AreEqual("Plums",results);
        }


        [TestMethod]
        public void Generate_Cat_Specific_Prod_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category="Apples"},
                new Product {ProductID = 2, Name = "P2", Category="Plums"},
                new Product {ProductID = 3, Name = "P3", Category="Plums"},
                new Product {ProductID = 4, Name = "P4", Category="Apples"},
                new Product {ProductID = 5, Name = "P5", Category="Oranges"}
            });

            ProductController target = new ProductController(mock.Object);
            target.pageSize = 3;

            //act
            var results = ((ProductsListViewModel)target.List("Apples").Model).PagingInfo.TotalItems;
            var results1 = ((ProductsListViewModel)target.List("Plums").Model).PagingInfo.TotalItems;
            var results2 = ((ProductsListViewModel)target.List("Oranges").Model).PagingInfo.TotalItems;
            var results4 = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            //assert
            Assert.AreEqual(results, 2);
            Assert.AreEqual(results1, 2);
            Assert.AreEqual(results2, 1);
            Assert.AreEqual(results4, 5);
        }
    }
}
