//Bearbeiter: Abbas Dayeh
//            Yusuf Can Sönmez(PlaceOrder Test)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcFakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using WebShop.Controllers;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Models.Enum;
using WebShop.Tests.Utilities;

namespace WebShop.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTests
    {
        // Mock-Objekt für die WebShopEntities-Datenbank
        Mock<WebShopEntities> contextMock = new Mock<WebShopEntities>();

        public ProductControllerTests()
        {
            // Arrange
            // Erstellen von Claims für den Testbenutzer
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            // Erstellen einer gefälschten Tabelle für tblItem
            var dbSet = new FakeDbSet<tblItem>();
            contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblItems).Returns(dbSet);
            dbSet.Add(new tblItem()
            {
                Id = 10,
                Name = "TestHardware",
                Description = "TestHardwarebeschreibung",
                Type = 10,
                Cost = "20",
                IsActive = "Y",
                tblItemTypeMaster = new tblItemTypeMaster() { Id = (int)ItemTypeEnum.Hardware }
            });
            dbSet.Add(new tblItem()
            {
                Id = 20,
                Name = "TestLizenzsoftware",
                Description = "TestLizenzsoftwarebeschreibung",
                Type = 30,
                Cost = "17",
                IsActive = "Y",
                tblItemTypeMaster = new tblItemTypeMaster() { Id = (int)ItemTypeEnum.LicensedSoftware }
            });

            // Erstellen einer gefälschten Tabelle für tblStock
            var dbSet1 = new FakeDbSet<tblStock>();
            contextMock.Setup(dbContext => dbContext.tblStocks).Returns(dbSet1);
            dbSet1.Add(new tblStock()
            {
                Id = 20,
                ItemId = 10,
                Quantity = 3,
                CreatedBy = "TestUser",
                CreatedDate = DateTime.Now
            });
            dbSet1.Add(new tblStock()
            {
                Id = 90,
                ItemId = 20,
                Quantity = 0,
                CreatedBy = "TestUser",
                CreatedDate = DateTime.Now
            });

            // Erstellen einer gefälschten Tabelle für tblStockDetail
            var dbSet2 = new FakeDbSet<tblStockDetail>();
            contextMock.Setup(dbContext => dbContext.tblStockDetails).Returns(dbSet2);
            dbSet2.Add(new tblStockDetail()
            {
                Id = 30,
                StockId = 20,
                SerialNumber = "111",
                IsDeleted = "N",
                DeleteReason = null
            });
            dbSet2.Add(new tblStockDetail()
            {
                Id = 40,
                StockId = 20,
                SerialNumber = "222",
                IsDeleted = "N",
                DeleteReason = null
            });
            dbSet2.Add(new tblStockDetail()
            {
                Id = 50,
                StockId = 20,
                SerialNumber = "333",
                IsDeleted = "N",
                DeleteReason = null
            });
        }

        [TestMethod]
        public void TestIndexWithSearch()
        {
            //Arrange
            // Bereits im Konstruktor erledigt

            //Act
            ProductController _controller = new ProductController(contextMock.Object);
            var result = _controller.Index("Hard");

            //assert
            Assert.AreEqual(1, ((List<ProductModel>)((System.Web.Mvc.ViewResultBase)result).Model).Count);

        }

        [TestMethod]
        public void TestIndexWithoutSearch()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);
            var result = _controller.Index("");

            //assert
            Assert.AreEqual(2, ((List<ProductModel>)((System.Web.Mvc.ViewResultBase)result).Model).Count);
        }

        [TestMethod]
        public void TestIndexFiltering()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);
            var result = _controller.IndexFilter("Hardware");

            //assert
            Assert.AreEqual(1, ((List<ProductModel>)((System.Web.Mvc.ViewResultBase)result).Model).Count);
        }

        [TestMethod]
        public void TestAddToCart()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);

            // Erstellen eines gefälschten Controller-Kontexts für die Sitzung
            var sessionItems = new SessionStateItemCollection();
            _controller.ControllerContext = new FakeControllerContext(_controller, sessionItems);

            var result = _controller.AddToCart(10); // Artikel in den Warenkorb legen

            //assert
            Assert.AreEqual(1, _controller.Session["CartCounter"]);
        }

        [TestMethod]
        public void TestEditCart()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);

            // Erstellen eines gefälschten Controller-Kontexts für die Sitzung
            var sessionItems = new SessionStateItemCollection();
            _controller.ControllerContext = new FakeControllerContext(_controller, sessionItems);

            _controller.AddToCart(10); // Artikel hinzufügen, um Daten in der Sitzung zu erhalten
            var lstShoppingCartModel = (List<ShoppingCartModel>)_controller.Session["CartItem"];
            var initialStartDt = lstShoppingCartModel.Where(x => x.Id == 10).Single().LendingStartDt;
            var initialEndDt = lstShoppingCartModel.Where(x => x.Id == 10).Single().LendingEndDt;

            var result = _controller.EditCart(10, 3); // Leihdauer auf 3 Monate ändern
            var lstShoppingCartModelAfterEdit = (List<ShoppingCartModel>)_controller.Session["CartItem"];
            var initialStartDtAfterEdit = lstShoppingCartModelAfterEdit.Where(x => x.Id == 10).Single().LendingStartDt;
            var initialEndDtAfterEdit = lstShoppingCartModelAfterEdit.Where(x => x.Id == 10).Single().LendingEndDt;

            //assert
            Assert.AreEqual(1, _controller.Session["CartCounter"]);
            Assert.AreEqual(initialStartDt.ToShortDateString(), initialStartDtAfterEdit.ToShortDateString());
            Assert.AreEqual(initialEndDtAfterEdit.ToShortDateString(), initialStartDt.AddMonths(3).ToShortDateString());

        }

        [TestMethod]
        public void TestRemoveItem()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);

            // Erstellen eines gefälschten Controller-Kontexts für die Sitzung
            var sessionItems = new SessionStateItemCollection();
            _controller.ControllerContext = new FakeControllerContext(_controller, sessionItems);

            _controller.AddToCart(10); // Artikel in den Warenkorb legen

            var result = _controller.RemoveItem(10); // Artikel mit der ID 10 entfernen

            //assert
            Assert.AreEqual(0, _controller.Session["CartCounter"]);
        }

        [TestMethod]
        public void TestDetailsById()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);

            var result = _controller.DetailsById(10); // Details des Artikels mit der ID 10 abrufen

            //assert
            Assert.AreEqual(10, ((tblItem)((ViewResultBase)result).Model).Id);
        }

        [TestMethod]
        public void TestPlaceOrder()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);

            // Erstellen eines gefälschten Controller-Kontexts für die Sitzung
            var sessionItems = new SessionStateItemCollection();
            _controller.ControllerContext = new FakeControllerContext(_controller, sessionItems);

            _controller.AddToCart(10); // Artikel in den Warenkorb legen

            var result = _controller.PlaceOrder(); // Bestellung aufgeben

            //assert
            Assert.AreEqual(0, _controller.Session["CartCounter"]);
        }

        [TestMethod]
        public void TestClearCart()
        {
            //Act
            ProductController _controller = new ProductController(contextMock.Object);

            // Erstellen eines gefälschten Controller-Kontexts für die Sitzung
            var sessionItems = new SessionStateItemCollection();
            _controller.ControllerContext = new FakeControllerContext(_controller, sessionItems);

            _controller.AddToCart(10); // Artikel in den Warenkorb legen

            var result = _controller.ClearCart(); // Warenkorb leeren

            //assert
            Assert.IsNull(_controller.Session["CartCounter"]);
            Assert.IsNull(_controller.Session["CartItem"]);
        }

    }
}