//Bearbeiter: Alper Daglioglu
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebShop.Models;
using System.Linq;
using Moq;
using WebShop.Models.Entity;
using WebShop.Tests.Utilities;
using WebShop.Helper;
using WebShop.Models.Enum;
using System.Web;
using System.Web.Routing;
using WebShop.Controllers;

namespace WebShop.Tests.Controllers
{
    [TestClass()]
    public class AccountControllerTests
    {
        

        [TestMethod()]
        public void LoginTest()
        {
            //Arrange
            AccountController _controller = new AccountController();

            //Act
            var result = _controller.Login("") as ActionResult;

            ////Assert
            Assert.AreEqual("", ((ViewResultBase)result).ViewName);
        }

        [TestMethod()]
        public void Login_TestLoginModel_UserName()
        {
            // Arrange
            AccountController _controller = new AccountController();
            var loginModel = new LoginModel() { UserName = null, Password = "admin123" };

            // Validate model state start
            var validationContext = new ValidationContext(loginModel, null, null);
            var validationResults = new List<ValidationResult>();

            //set validateAllProperties to true to validate all properties; if false, only required attributes are validated.
            Validator.TryValidateObject(loginModel, validationContext, validationResults, false);
            foreach (var validationResult in validationResults)
            {
                _controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
            // Validate model state end


            // Act
            var result =  _controller.Login(loginModel);

            // Assert
            Assert.IsTrue(((ViewResultBase)result).ViewData.ModelState["UserName"].Errors.Count > 0);
        }

        [TestMethod()]
        public void Login_TestLoginModel_Password()
        {
            // Arrange
            AccountController _controller = new AccountController();
            var loginModel = new LoginModel() { UserName = "Admin", Password = null };

            // Validate model state start
            var validationContext = new ValidationContext(loginModel, null, null);
            var validationResults = new List<ValidationResult>();

            //set validateAllProperties to true to validate all properties; if false, only required attributes are validated.
            Validator.TryValidateObject(loginModel, validationContext, validationResults, false);
            foreach (var validationResult in validationResults)
            {
                _controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
            // Validate model state end


            // Act
            var result = _controller.Login(loginModel);

            // Assert
            Assert.IsTrue(((ViewResultBase)result).ViewData.ModelState["Password"].Errors.Count > 0);
        }

        [TestMethod()]
        public void Login_TestLoginModel_UserNameWrong()
        {
            // Arrange
            var loginModel = new LoginModel() { UserName = "Admin123", Password = "Admin123" };
            PasswordHash hash = new PasswordHash(loginModel.Password);

            // create fake table with 2 rows
            var userDbSet = new FakeDbSet<tblUser>();
            userDbSet.Add(new tblUser() { UserName = "Admin12", HashPassword = hash.ToArray() });
            userDbSet.Add(new tblUser() { UserName = "user2", HashPassword = null });

            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);

            AccountController _controller = new AccountController(contextMock.Object);

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            Assert.AreEqual(((ViewResultBase)result).ViewData.ModelState[""].Errors[0].ErrorMessage,
                "Benutzername oder Passwort ungültig.");
        }

        [TestMethod()]
        public void Login_TestLoginModel_PasswordWrong()
        {
            // Arrange
            var loginModel = new LoginModel() { UserName = "Admin123", Password = "Admin123" };
            var wrongPassword = new PasswordHash("Admin12");
           

            // create fake table with 2 rows
            var userDbSet = new FakeDbSet<tblUser>();
            userDbSet.Add(new tblUser() { UserName = "Admin123", HashPassword = wrongPassword.ToArray() });
            userDbSet.Add(new tblUser() { UserName = "user2", HashPassword = null });

            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);

            AccountController _controller = new AccountController(contextMock.Object);

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            Assert.AreEqual(((ViewResultBase)result).ViewData.ModelState[""].Errors[0].ErrorMessage,
                "Benutzername oder Passwort ungültig.");
        }

        [TestMethod()]
        public void Login_TestLoginModel_CorrectPasswordUserName()
        {
            // Arrange
            var loginModel = new LoginModel() { UserName = "Admin123", Password = "Admin123" };
            var correctPassword = new PasswordHash("Admin123");

            // create fake table with 2 rows
            var userDbSet = new FakeDbSet<tblUser>();
            userDbSet.Add(new tblUser() { UserName = "Admin123", HashPassword = correctPassword.ToArray()
                , IsActive = "Y", UserRole = (int)UserRoleEnum.Admins, 
                tblUserRolesMaster = new tblUserRolesMaster() { UserRole = "Administrator" } });
            userDbSet.Add(new tblUser() { UserName = "user2", HashPassword = null });

            var contextMock = new Mock<WebShopEntities>();
            contextMock.Setup(dbContext => dbContext.tblUsers).Returns(userDbSet);

            AccountController _controller = new AccountController(contextMock.Object);

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            Assert.AreEqual(loginModel.UserName, ((ViewResultBase)result).ViewBag.CurrentUser.UserName);
        }

        [TestMethod()]
        public void TestSignOut()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.Expect(r => r.HttpMethod).Returns("GET");

            var httpContext = new Mock<HttpContextBase>();
            var session = new Mock<HttpSessionStateBase>();

            httpContext.Expect(c => c.Request).Returns(request.Object);
            httpContext.Expect(c => c.Session).Returns(session.Object);

            session.Expect(c => c.Add("CartCounter", 1));
            session.Expect(c => c.Add("CartItem", "hello world"));

            AccountController _controller = new AccountController();
            _controller.ControllerContext = new ControllerContext(new RequestContext(httpContext.Object, new RouteData()), _controller);

            // session.VerifyAll(); // function is trying to add the desired item to the session in the constructor
            //Act
            var result = _controller.SignOut() as ViewResult;

            // Assert
            Assert.IsNull(_controller.Session["CartCounter"]);
            Assert.IsNull(_controller.Session["CartItem"]);
        }
    }
}