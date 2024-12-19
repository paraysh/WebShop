//Bearbeiter: Alper Daglioglu
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebShop.Controllers;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Tests.Utilities;

namespace WebShop.Tests.Controllers
{
    // Diese Klasse enthält Unit-Tests für den StockController.
    [TestClass]
    public class StockControllerTests
    {
        // Diese Methode testet die Add-Methode des StockControllers.
        [TestMethod]
        public void TestAdd()
        {
            // Arrange: Initialisierung der Testdaten und Mock-Objekte.
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            AddStockModel addStockModel = new AddStockModel()
            {
                Id = 10, // Artikel-ID
                LstSerialNumbers = new List<string>() { "111", "222", "333" },
                Quantity = 3,
            };

            var contextMock = new Mock<WebShopEntities>();
            var dbSet = new FakeDbSet<tblStock>();
            contextMock.Setup(dbContext => dbContext.tblStocks).Returns(dbSet);

            var dbSet2 = new FakeDbSet<tblStockDetail>();
            contextMock.Setup(dbContext => dbContext.tblStockDetails).Returns(dbSet2);

            StockController _controller = new StockController(contextMock.Object);

            // Act: Aufruf der Add-Methode des Controllers.
            var result = _controller.Add(addStockModel);

            // Assert: Überprüfung der Ergebnisse.
            Assert.AreEqual(1, dbSet.Count()); // 1 Zeile in tblStocks
            Assert.AreEqual(3, dbSet2.Count()); // 3 Zeilen in tblStockDetails
        }

        // Diese Methode testet die Remove-Methode des StockControllers.
        [TestMethod]
        public void TestRemove()
        {
            // Arrange: Initialisierung der Testdaten und Mock-Objekte.
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            AddStockModel addStockModel = new AddStockModel()
            {
                Id = 10, // Artikel-ID
                SelectedSerialNo = "30", // "111" Seriennummer
                Quantity = 3,
                DeleteReason = "Test delete"
            };

            var contextMock = new Mock<WebShopEntities>();
            var dbSet = new FakeDbSet<tblItem>();
            contextMock.Setup(dbContext => dbContext.tblItems).Returns(dbSet);
            dbSet.Add(new tblItem()
            {
                Id = 10,
                Name = "TestHardware",
                Description = "TestHardwarebeschreibung",
                Type = 10,
                Cost = "20",
                IsActive = "Y"
            });

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

            StockController _controller = new StockController(contextMock.Object);

            // Act: Aufruf der Remove-Methode des Controllers.
            var result = _controller.Remove(addStockModel);

            // Assert: Überprüfung der Ergebnisse.
            Assert.AreEqual(1, dbSet.Count()); // 1 Zeile in tblItems
            Assert.AreEqual(1, dbSet1.Count()); // 1 Zeile in tblStocks
            Assert.AreEqual(3, dbSet2.Count()); // 3 Zeilen in tblStockDetails
            Assert.AreEqual("Y", dbSet2.Where(x => x.SerialNumber == "111").First().IsDeleted); // Überprüfung, ob die Seriennummer "111" gelöscht wurde
        }
    }
}