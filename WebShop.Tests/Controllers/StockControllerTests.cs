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
    [TestClass]
    public class StockControllerTests
    {
        [TestMethod]
        public void TestIndex()
        {
            // Arrange
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            // create fake table
            var dbSet = new FakeDbSet<tblItem>();
            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblItems).Returns(dbSet);
            dbSet.Add(new tblItem()
            {
                Id = 10,
                Name = "TestHardware",
                Description = "TestHardwarebeschreibung",
                Type = 10,
                Cost = "20",
                IsActive = "Y",
                tblStocks = new List<tblStock>() {
                    new tblStock {
                        ItemId = 10,
                        Quantity = 1
                    },
                    new tblStock {
                        ItemId = 10,
                        Quantity = 2
                    }
                }
            });
            dbSet.Add(new tblItem()
            {
                Id = 20,
                Name = "TestLizenzsoftware",
                Description = "TestLizenzsoftwarebeschreibung",
                Type = 30,
                Cost = "17",
                IsActive = "N",
                tblStocks = new List<tblStock>() {
                    new tblStock {
                        ItemId = 20,
                        Quantity = 10
                    },
                    new tblStock {
                        ItemId = 20,
                        Quantity = 14
                    }
                }
            });

            StockController _controller = new StockController(contextMock.Object);
            //Act
            var result = _controller.Index();

            //Assert
            //Assert.AreEqual(2, dbSet.Count());
        }

        [TestMethod]
        public void TestAdd()
        {
            // Arrange
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            AddStockModel addStockModel = new AddStockModel()
            {
                Id= 10, // Item ID
                LstSerialNumbers = new List<string>() { "111","222","333" },
                Quantity = 3,

            };

            var contextMock = new Mock<WebShopEntities>();
            var dbSet = new FakeDbSet<tblStock>();
            contextMock.Setup(dbContext => dbContext.tblStocks).Returns(dbSet);

            var dbSet2 = new FakeDbSet<tblStockDetail>();
            contextMock.Setup(dbContext => dbContext.tblStockDetails).Returns(dbSet2);

            StockController _controller = new StockController(contextMock.Object);



            //Act
            var result = _controller.Add(addStockModel);

            //Assert
            Assert.AreEqual(1, dbSet.Count()); // 1 row in tblStocks
            Assert.AreEqual(3, dbSet2.Count()); // 3 rows in tblStockDetails
        }

        [TestMethod]
        public void TestRemove()
        {
            // Arrange
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            AddStockModel addStockModel = new AddStockModel()
            {
                Id = 10, // Item ID
                SelectedSerialNo = "30", // "111" serial number
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


            //Act
            var result = _controller.Remove(addStockModel);

            //Assert
            Assert.AreEqual(1, dbSet.Count()); // 1 row in tblItems
            Assert.AreEqual(1, dbSet1.Count()); // 1 row in tblStocks
            Assert.AreEqual(3, dbSet2.Count()); // 3 rows in tblStockDetails
            Assert.AreEqual("Y", dbSet2.Where(x => x.SerialNumber == "111").First().IsDeleted); // Chaeck if serial no "111" is deleted
        }
    }
}
