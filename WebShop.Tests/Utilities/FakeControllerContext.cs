//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace MvcFakes
{
    // Diese Klasse stellt einen gefälschten ControllerContext für Unit-Tests bereit.
    public class FakeControllerContext : ControllerContext
    {
        // Konstruktor, der nur den Controller akzeptiert.
        public FakeControllerContext(IController controller)
            : this(controller, null, null, null, null, null, null)
        {
        }

        // Konstruktor, der den Controller und Cookies akzeptiert.
        public FakeControllerContext(IController controller, HttpCookieCollection cookies)
            : this(controller, null, null, null, null, cookies, null)
        {
        }

        // Konstruktor, der den Controller und Session-Items akzeptiert.
        public FakeControllerContext(IController controller, SessionStateItemCollection sessionItems)
            : this(controller, null, null, null, null, null, sessionItems)
        {
        }

        // Konstruktor, der den Controller und Formularparameter akzeptiert.
        public FakeControllerContext(IController controller, NameValueCollection formParams)
            : this(controller, null, null, formParams, null, null, null)
        {
        }

        // Konstruktor, der den Controller, Formularparameter und Query-String-Parameter akzeptiert.
        public FakeControllerContext(IController controller, NameValueCollection formParams, NameValueCollection queryStringParams)
            : this(controller, null, null, formParams, queryStringParams, null, null)
        {
        }

        // Konstruktor, der den Controller und den Benutzernamen akzeptiert.
        public FakeControllerContext(IController controller, string userName)
            : this(controller, userName, null, null, null, null, null)
        {
        }

        // Konstruktor, der den Controller, den Benutzernamen und Rollen akzeptiert.
        public FakeControllerContext(IController controller, string userName, string[] roles)
            : this(controller, userName, roles, null, null, null, null)
        {
        }

        // Hauptkonstruktor, der alle möglichen Parameter akzeptiert.
        public FakeControllerContext
            (
                IController controller,
                string userName,
                string[] roles,
                NameValueCollection formParams,
                NameValueCollection queryStringParams,
                HttpCookieCollection cookies,
                SessionStateItemCollection sessionItems
            )
            : base(new FakeHttpContext(new FakePrincipal(new FakeIdentity(userName), roles), formParams, queryStringParams, cookies, sessionItems), new RouteData(), (ControllerBase)controller)
        { }
    }
}