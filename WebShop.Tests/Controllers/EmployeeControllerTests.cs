//Bearbeiter: Alper Daglioglu
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using WebShop.Controllers;
using WebShop.Helper;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Models.Enum;
using WebShop.Tests.Utilities;

namespace WebShop.Tests.Controllers
{
    [TestClass]
    public class EmployeeControllerTests
    {
        [TestMethod]
        public void Test_AddAdmin()
        {
            // Arrange
            var userModel = new UserModel()
            {
                UserName = "Admin123",
                Password = "Admin123",
                ConfirmPassword = "Admin123",
                FirstName = "Tom",
                LastName = "Müller",
                Email = "tommueller@email.com",
                UserRoleEnum = UserRoleEnum.Admins,
                IsActive = "Y",
            };

            // Erstelle eine gefälschte Benutzertabelle
            var userDbSet = new FakeDbSet<tblUser>();

            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);

            EmployeeController _controller = new EmployeeController(contextMock.Object);

            // Act
            var result = _controller.Add(userModel);
            var rows = userDbSet.Count(); // Überprüfe die Zeilenanzahl

            // Assert
            Assert.AreEqual(1, rows);
        }

        [TestMethod]
        public void Test_AddTeamLeader()
        {
            // Arrange
            var teamLeaderModel = new UserModel()
            {
                UserName = "TestTeamleiter",
                Password = "TestPasswort",
                ConfirmPassword = "TestPasswort",
                FirstName = "Maxime",
                LastName = "Mustermann",
                Email = "maxime.mustermann@email.de",
                UserRoleEnum = UserRoleEnum.TeamLeaders,
                TeamBudget = "999,00",
                IsActive = "Y",
            };

            // Erstelle eine gefälschte Benutzertabelle
            var userDbSet = new FakeDbSet<tblUser>();
            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);

            EmployeeController _controller = new EmployeeController(contextMock.Object);

            // Act
            var result = _controller.Add(teamLeaderModel); // Füge Teamleiter hinzu
            var rows = userDbSet.Count<tblUser>(); // Überprüfe die Zeilenanzahl

            // Assert
            Assert.AreEqual(1, rows);
            Assert.AreEqual("TestTeamleiter", userDbSet.First().UserName);
        }

        [TestMethod]
        public void Test_AddEmployee()
        {
            // Arrange
            var employeeModel = new UserModel()
            {
                UserName = "TestMitarbeiter",
                Password = "123",
                ConfirmPassword = "123",
                FirstName = "Max",
                LastName = "Mustermann",
                Email = "max.mustermann@email.de",
                TeamLeader = 99,
                UserRoleEnum = UserRoleEnum.Employee,
                EmployeeBudget = "600,00",
                IsActive = "Y",
            };

            // Erstelle eine gefälschte Benutzertabelle
            var userDbSet = new FakeDbSet<tblUser>();
            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);
            userDbSet.Add(new tblUser()  // Erstelle Teamleiter-Datensatz
            {
                Id = 99,
                UserName = "TestTeamleiter",
                FirstName = "Maxime",
                LastName = "Mustermann",
                Email = "maxime.mustermann@email.de",
                UserRole = (int)UserRoleEnum.TeamLeaders,
                IsActive = "Y"
            });

            EmployeeController _controller = new EmployeeController(contextMock.Object);

            // Act
            var result = _controller.Add(employeeModel); // Füge Mitarbeiter hinzu
            var rows = userDbSet.Count<tblUser>(); // Überprüfe die Zeilenanzahl

            // Assert
            Assert.AreEqual(2, rows);
            Assert.AreEqual(1, userDbSet.Where(x => x.UserName == "TestTeamleiter").Count());
        }

        [TestMethod]
        public void Test_EditUser()
        {
            // Arrange
            var userModel = new UserModel()
            {
                Id = 99,
                UserName = "Admin123",
                Password = "Admin123",
                ConfirmPassword = "Admin123",
                FirstName = "Kevin",
                LastName = "Schmidt",
                Email = "kevinschmidt@email.com",
                UserRoleEnum = UserRoleEnum.Admins,
                IsActive = "Y",
            };

            // Erstelle eine gefälschte Benutzertabelle
            var userDbSet = new FakeDbSet<tblUser>();

            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);
            userDbSet.Add(new tblUser()  // Füge Mitarbeiter in die Tabelle ein, um ihn zu bearbeiten
            {
                Id = 99,
                UserName = "TestTeamleiter",
                FirstName = "Maxime",
                LastName = "Mustermann",
                Email = "maxime.mustermann@email.de",
                UserRole = (int)UserRoleEnum.TeamLeaders,
                IsActive = "Y"
            });

            EmployeeController _controller = new EmployeeController(contextMock.Object);

            // Act
            var result = _controller.Edit(userModel);
            var rows = userDbSet.Count<tblUser>(); // Überprüfe die Zeilenanzahl

            // Assert
            Assert.AreEqual(1, rows);
            Assert.AreEqual(1, userDbSet.Where(x => x.UserName == "Admin123").Count());
        }

        [TestMethod]
        public void Test_DeavtivateUser()
        {
            // Arrange
            var userId = 99;

            // Erstelle eine gefälschte Benutzertabelle
            var userDbSet = new FakeDbSet<tblUser>();

            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);
            userDbSet.Add(new tblUser()  // Füge Mitarbeiter in die Tabelle ein, um ihn zu bearbeiten
            {
                Id = 99,
                UserName = "TestTeamleiter",
                FirstName = "Maxime",
                LastName = "Mustermann",
                Email = "maxime.mustermann@email.de",
                UserRole = (int)UserRoleEnum.TeamLeaders,
                IsActive = "Y" // Benutzer ist in der Tabelle aktiv
            });

            EmployeeController _controller = new EmployeeController(contextMock.Object);

            // Act
            var result = _controller.Deactivate(userId);
            var rows = userDbSet.Count<tblUser>(); // Überprüfe die Zeilenanzahl

            // Assert
            Assert.AreEqual(1, rows); // Zeile sollte in der Tabelle existieren
            Assert.AreEqual("N", userDbSet.Where(x => x.Id == 99).Single().IsActive); // Überprüfe, ob isActive = N ist
        }
    }
}