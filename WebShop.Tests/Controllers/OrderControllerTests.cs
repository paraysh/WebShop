//Bearbeiter: Abbas Dayeh
//            Yusuf Can Sönmez(Unterstützung bei der Implementierung)
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using WebShop.Controllers;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Tests.Utilities;

namespace WebShop.Tests.Controllers
{
    [TestClass]
    public class OrderControllerTests
    {
        [TestMethod]
        public void TestApproveRightBudget()
        {
            // Arrange
            var orderId = 77;
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            // Erstellen eines gefälschten Benutzertabellenkontexts
            var contextMock = new Mock<WebShopEntities>();
            var dbSet = new FakeDbSet<tblOrder>();
            contextMock.Setup(dbContext => dbContext.tblOrders).Returns(dbSet);
            dbSet.Add(new tblOrder()
            {
                Id = 77,
                OrderId = "test-order-id-test",
                OrderedBy = 99,
                tblUser = new tblUser()
                {
                    UserName = "Test_User"
                },
                OrderDate = DateTime.Now,
                OrderApproved = "N",
                TotalItems = 1,
                TotalCost = 200,
                tblOrderDetails = new List<tblOrderDetail>()
                {
                    new tblOrderDetail()
                    {
                        Id = 12,
                        OrderId = 77,
                        StockDetailsId = 22,
                        LendingPeriodMonths = 2,
                        LendingStartDt = DateTime.Now,
                        LendingEndDt = DateTime.Now.AddMonths(2),
                        ItemId = 55,
                        tblOrder = new tblOrder(){
                            Id = 77
                        }
                    }
                },
                tblStockDetails = new List<tblStockDetail>()
                {
                    new tblStockDetail()
                    {
                        Id= 22,
                        StockId = 33,
                        SerialNumber = "11111-22222-33333",
                        OrderId= 77,
                        IsDeleted = "N",
                        DeleteReason = null,
                        tblStock = new tblStock()
                        {
                            Id = 33,
                            ItemId = 55,
                            Quantity = 10,
                        }
                    }
                }
            });

            var dbSet1 = new FakeDbSet<tblTeamEmployee>();
            contextMock.Setup(dbContext => dbContext.tblTeamEmployees).Returns(dbSet1);
            dbSet1.Add(new tblTeamEmployee()
            {
                Id = 1,
                TeamEmployeeId = 99,
                Year = DateTime.Now.Year,
                TeamEmployeeBudget = "500,99"
            });

            OrderController _controller = new OrderController(contextMock.Object);

            //Act
            var result = _controller.Approve(orderId);

            //Assert
            Assert.AreEqual("Y", dbSet.Where(x => x.Id == orderId).Single().OrderApproved);
        }

        [TestMethod]
        public void TestApproveWrongBudget()
        {
            // Arrange
            var orderId = 77;
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            // Erstellen eines gefälschten Benutzertabellenkontexts
            var contextMock = new Mock<WebShopEntities>();
            var dbSet = new FakeDbSet<tblOrder>();
            contextMock.Setup(dbContext => dbContext.tblOrders).Returns(dbSet);
            dbSet.Add(new tblOrder()
            {
                Id = 77,
                OrderId = "test-order-id-test",
                OrderedBy = 99,
                tblUser = new tblUser()
                {
                    UserName = "Test_User"
                },
                OrderDate = DateTime.Now,
                OrderApproved = "N",
                TotalItems = 1,
                TotalCost = 200,
                tblOrderDetails = new List<tblOrderDetail>()
                {
                    new tblOrderDetail()
                    {
                        Id = 12,
                        OrderId = 77,
                        StockDetailsId = 22,
                        LendingPeriodMonths = 2,
                        LendingStartDt = DateTime.Now,
                        LendingEndDt = DateTime.Now.AddMonths(2),
                        ItemId = 55,
                        tblOrder = new tblOrder(){
                            Id = 77
                        }
                    }
                },
                tblStockDetails = new List<tblStockDetail>()
                {
                    new tblStockDetail()
                    {
                        Id= 22,
                        StockId = 33,
                        SerialNumber = "11111-22222-33333",
                        OrderId= 77,
                        IsDeleted = "N",
                        DeleteReason = null,
                        tblStock = new tblStock()
                        {
                            Id = 33,
                            ItemId = 55,
                            Quantity = 10,
                        }
                    }
                }
            });

            var dbSet1 = new FakeDbSet<tblTeamEmployee>();
            contextMock.Setup(dbContext => dbContext.tblTeamEmployees).Returns(dbSet1);
            dbSet1.Add(new tblTeamEmployee()
            {
                Id = 1,
                TeamEmployeeId = 99,
                Year = DateTime.Now.Year,
                TeamEmployeeBudget = "100,99"
            });

            OrderController _controller = new OrderController(contextMock.Object);

            //Act
            var result = _controller.Approve(orderId);

            //Assert
            Assert.AreEqual("N", dbSet.Where(x => x.Id == orderId).Single().OrderApproved);
        }

        [TestMethod]
        public void TestReject()
        {
            // Arrange
            var orderId = 77;
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, "50"),
                            new Claim(ClaimTypes.Name, "TestUser"),
                            new Claim(ClaimTypes.Role, "Employee"),
                            new Claim("UserId", "99")
                            };
            Thread.CurrentPrincipal = new TestPrincipal(claims);

            // Erstellen eines gefälschten Benutzertabellenkontexts
            var contextMock = new Mock<WebShopEntities>();
            var dbSet = new FakeDbSet<tblOrder>();
            contextMock.Setup(dbContext => dbContext.tblOrders).Returns(dbSet);
            dbSet.Add(new tblOrder()
            {
                Id = 77,
                OrderId = "test-order-id-test",
                OrderedBy = 99,
                tblUser = new tblUser()
                {
                    UserName = "Test_User"
                },
                OrderDate = DateTime.Now,
                OrderApproved = "N",
                TotalItems = 1,
                TotalCost = 200,
                tblOrderDetails = new List<tblOrderDetail>()
                {
                    new tblOrderDetail()
                    {
                        Id = 12,
                        OrderId = 77,
                        StockDetailsId = 22,
                        LendingPeriodMonths = 2,
                        LendingStartDt = DateTime.Now,
                        LendingEndDt = DateTime.Now.AddMonths(2),
                        ItemId = 55,
                        tblOrder = new tblOrder(){
                            Id = 77
                        }
                    }
                },
                tblStockDetails = new List<tblStockDetail>()
                {
                    new tblStockDetail()
                    {
                        Id= 22,
                        StockId = 33,
                        SerialNumber = "11111-22222-33333",
                        OrderId= 77,
                        IsDeleted = "N",
                        DeleteReason = null,
                        tblStock = new tblStock()
                        {
                            Id = 33,
                            ItemId = 55,
                            Quantity = 10,
                        }
                    }
                }
            });

            var dbSet1 = new FakeDbSet<tblTeamEmployee>();
            contextMock.Setup(dbContext => dbContext.tblTeamEmployees).Returns(dbSet1);
            dbSet1.Add(new tblTeamEmployee()
            {
                Id = 1,
                TeamEmployeeId = 99,
                Year = DateTime.Now.Year,
                TeamEmployeeBudget = "100,99"
            });

            OrderController _controller = new OrderController(contextMock.Object);

            //Act
            var result = _controller.Reject(orderId);

            //Assert
            Assert.AreEqual("R", dbSet.Where(x => x.Id == orderId).Single().OrderApproved);
        }
    }
}