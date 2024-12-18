//Bearbeiter: Alper Daglioglu
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using WebShop.Controllers;
using WebShop.Models.Entity;
using WebShop.Models.Enum;
using WebShop.Models;
using WebShop.Tests.Utilities;
using System.Linq;

namespace WebShop.Tests.Controllers
{
    [TestClass]
    public class ItemControllerTest
    {
        [TestMethod]
        public void Test_AddItem()
        {
            // Arrange
            var item1Model = new ItemModel()
            {
                Name = "TestHardware",
                Description = "TestHardwarebeschreibung",
                Type = 10,
                Cost = "20",
                IsActive = "Y"
            };

            var item2Model = new ItemModel()
            {
                Name = "TestLizenzsoftware",
                Description = "TestLizenzsoftwarebeschreibung",
                Type = 30,
                Cost = "17",
                IsActive = "N"
            };

            // create fake table
            var dbSet = new FakeDbSet<tblItem>();
            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblItems).Returns(dbSet);

            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            var result1 = _controller.Add(item1Model);
            var result2 = _controller.Add(item2Model);
            var rows = dbSet.Count(); // check row count

            //Assert
            Assert.AreEqual(2, rows); // check if 2 rows added
            Assert.IsNotNull(dbSet.Where(x => x.Name == "TestHardware" && x.IsActive == "Y"));
            Assert.IsNotNull(dbSet.Where(x => x.Name == "TestLizenzsoftware" && x.IsActive == "N"));
        }

        [TestMethod]
        public void Test_EditItem()
        {
            // Arrange
            var item1Model = new ItemModel()
            {
                Id = 10,
                Name = "TestHardware",
                Description = "TestHardwarebeschreibung",
                Type = 10,
                Cost = "10",
                IsActive = "Y"
            };

            var item2Model = new ItemModel()
            {
                Id = 20,
                Name = "TestLizenzsoftware",
                Description = "TestLizenzsoftwarebeschreibung",
                Type = 30,
                Cost = "0",
                IsActive = "N"
            };

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
                IsActive = "Y"
            });
            dbSet.Add(new tblItem()  
            {
                Id = 20,
                Name = "TestLizenzsoftware",
                Description = "TestLizenzsoftwarebeschreibung",
                Type = 30,
                Cost = "17",
                IsActive = "N"
            });

            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            var result1 = _controller.Edit(item1Model);
            var result2 = _controller.Edit(item2Model);
            var rows = dbSet.Count(); // check row count

            //Assert
            Assert.AreEqual(2, rows); // check if 2 rows in table
            Assert.AreEqual("10,00", dbSet.Where(x => x.Id == 10).Single().Cost);
            Assert.AreEqual("0,00", dbSet.Where(x => x.Id == 20).Single().Cost);
        }

        [TestMethod]
        public void Test_DeavtivateItem()
        {
            // Arrange
            var itemId = 10;

            // create fake user table
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
                IsActive = "Y"
            });
            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            var result = _controller.Deactivate(itemId);
            var rows = dbSet.Count(); // check row count

            //Assert
            Assert.AreEqual(1, rows); // row should exist in table
            Assert.AreEqual("N", dbSet.Where(x => x.Id == itemId).Single().IsActive); // check if isActive = N
        }

        [TestMethod]
        public void Test_ActivateItem()
        {
            // Arrange
            var itemId = 20;

            // create fake user table
            var dbSet = new FakeDbSet<tblItem>();
            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblItems).Returns(dbSet);
            dbSet.Add(new tblItem()
            {
                Id = 20,
                Name = "TestLizenzsoftware",
                Description = "TestLizenzsoftwarebeschreibung",
                Type = 30,
                Cost = "17",
                IsActive = "N"
            });

            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            var result = _controller.Activate(itemId);
            var rows = dbSet.Count(); // check row count

            //Assert
            Assert.AreEqual(1, rows); // row should exist in table
            Assert.AreEqual("Y", dbSet.Where(x => x.Id == itemId).Single().IsActive); // check if isActive = Y
        }
    }
}
