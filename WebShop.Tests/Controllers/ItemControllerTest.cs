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
            // Erstellen von zwei ItemModel-Objekten mit Testdaten
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

            // Erstellen einer gefälschten Tabelle
            var dbSet = new FakeDbSet<tblItem>();
            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblItems).Returns(dbSet);

            // Erstellen eines ItemController-Objekts mit dem gefälschten Kontext
            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            // Hinzufügen der Items zur gefälschten Tabelle
            var result1 = _controller.Add(item1Model);
            var result2 = _controller.Add(item2Model);
            var rows = dbSet.Count(); // Überprüfen der Zeilenanzahl

            // Assert
            // Überprüfen, ob zwei Zeilen hinzugefügt wurden
            Assert.AreEqual(2, rows);
            // Überprüfen, ob die hinzugefügten Items in der Tabelle vorhanden sind
            Assert.IsNotNull(dbSet.Where(x => x.Name == "TestHardware" && x.IsActive == "Y"));
            Assert.IsNotNull(dbSet.Where(x => x.Name == "TestLizenzsoftware" && x.IsActive == "N"));
        }

        [TestMethod]
        public void Test_EditItem()
        {
            // Arrange
            // Erstellen von zwei ItemModel-Objekten mit Testdaten
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

            // Erstellen einer gefälschten Tabelle
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

            // Erstellen eines ItemController-Objekts mit dem gefälschten Kontext
            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            // Bearbeiten der Items in der gefälschten Tabelle
            var result1 = _controller.Edit(item1Model);
            var result2 = _controller.Edit(item2Model);
            var rows = dbSet.Count(); // Überprüfen der Zeilenanzahl

            // Assert
            // Überprüfen, ob zwei Zeilen in der Tabelle vorhanden sind
            Assert.AreEqual(2, rows);
            // Überprüfen, ob die Kosten der bearbeiteten Items korrekt aktualisiert wurden
            Assert.AreEqual("10,00", dbSet.Where(x => x.Id == 10).Single().Cost);
            Assert.AreEqual("0,00", dbSet.Where(x => x.Id == 20).Single().Cost);
        }

        [TestMethod]
        public void Test_DeavtivateItem()
        {
            // Arrange
            // Festlegen der Item-ID, die deaktiviert werden soll
            var itemId = 10;

            // Erstellen einer gefälschten Tabelle
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
            // Erstellen eines ItemController-Objekts mit dem gefälschten Kontext
            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            // Deaktivieren des Items in der gefälschten Tabelle
            var result = _controller.Deactivate(itemId);
            var rows = dbSet.Count(); // Überprüfen der Zeilenanzahl

            // Assert
            // Überprüfen, ob die Zeile in der Tabelle vorhanden ist
            Assert.AreEqual(1, rows);
            // Überprüfen, ob das Item korrekt deaktiviert wurde
            Assert.AreEqual("N", dbSet.Where(x => x.Id == itemId).Single().IsActive);
        }

        [TestMethod]
        public void Test_ActivateItem()
        {
            // Arrange
            // Festlegen der Item-ID, die aktiviert werden soll
            var itemId = 20;

            // Erstellen einer gefälschten Tabelle
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

            // Erstellen eines ItemController-Objekts mit dem gefälschten Kontext
            ItemController _controller = new ItemController(contextMock.Object);

            // Act
            // Aktivieren des Items in der gefälschten Tabelle
            var result = _controller.Activate(itemId);
            var rows = dbSet.Count(); // Überprüfen der Zeilenanzahl

            // Assert
            // Überprüfen, ob die Zeile in der Tabelle vorhanden ist
            Assert.AreEqual(1, rows);
            // Überprüfen, ob das Item korrekt aktiviert wurde
            Assert.AreEqual("Y", dbSet.Where(x => x.Id == itemId).Single().IsActive);
        }
    }
}