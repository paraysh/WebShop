//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

namespace MvcFakes
{
    // Diese Klasse stellt einen gefälschten HttpContext für Unit-Tests bereit.
    public class FakeHttpContext : HttpContextBase
    {
        // Private Felder zur Speicherung der gefälschten Daten.
        private readonly FakePrincipal _principal;
        private readonly NameValueCollection _formParams;
        private readonly NameValueCollection _queryStringParams;
        private readonly HttpCookieCollection _cookies;
        private readonly SessionStateItemCollection _sessionItems;

        // Konstruktor, der die gefälschten Daten initialisiert.
        public FakeHttpContext(FakePrincipal principal, NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies, SessionStateItemCollection sessionItems)
        {
            _principal = principal;
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
            _sessionItems = sessionItems;
        }

        // Überschreibt die Request-Eigenschaft, um eine gefälschte HttpRequest zurückzugeben.
        public override HttpRequestBase Request
        {
            get
            {
                return new FakeHttpRequest(_formParams, _queryStringParams, _cookies);
            }
        }

        // Überschreibt die User-Eigenschaft, um den gefälschten Principal zurückzugeben.
        public override IPrincipal User
        {
            get
            {
                return _principal;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        // Überschreibt die Session-Eigenschaft, um eine gefälschte HttpSessionState zurückzugeben.
        public override HttpSessionStateBase Session
        {
            get
            {
                return new FakeHttpSessionState(_sessionItems);
            }
        }
    }
}