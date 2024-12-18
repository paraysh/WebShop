using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebShop.Helper;

namespace WebShop.Tests.Controllers
{
    [TestClass]
    public class CommaHandlerTest
    {
        [TestMethod]
        public void Test_AddComma()
        {
            // Arrange
            var a = "17";
            var b = "17,1";
            var c = "18,00";
            var d = "20,22";
            var e = "0";

            // Act
            var aResult = CommaHandler.AddComma(a);
            var bResult = CommaHandler.AddComma(b);
            var cResult = CommaHandler.AddComma(c);
            var dResult = CommaHandler.AddComma(d);
            var eResult = CommaHandler.AddComma(e);

            //Assert
            Assert.AreEqual("17,00" ,aResult);
            Assert.AreEqual("17,10", bResult);
            Assert.AreEqual("18,00", cResult);
            Assert.AreEqual("20,22", dResult);
            Assert.AreEqual("0,00", eResult);
        }
    }
}
