//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Collections.Specialized;
using System.Web;

namespace MvcFakes
{
    // Diese Klasse stellt eine gefälschte HttpRequest für Unit-Tests bereit.
    public class FakeHttpRequest : HttpRequestBase
    {
        // Private Felder zur Speicherung der Formularparameter, Query-String-Parameter und Cookies.
        private readonly NameValueCollection _formParams;
        private readonly NameValueCollection _queryStringParams;
        private readonly HttpCookieCollection _cookies;

        // Konstruktor, der die Formularparameter, Query-String-Parameter und Cookies initialisiert.
        public FakeHttpRequest(NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies)
        {
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
        }

        // Überschreibt die Form-Eigenschaft, um die gefälschten Formularparameter zurückzugeben.
        public override NameValueCollection Form
        {
            get
            {
                return _formParams;
            }
        }

        // Überschreibt die QueryString-Eigenschaft, um die gefälschten Query-String-Parameter zurückzugeben.
        public override NameValueCollection QueryString
        {
            get
            {
                return _queryStringParams;
            }
        }

        // Überschreibt die Cookies-Eigenschaft, um die gefälschten Cookies zurückzugeben.
        public override HttpCookieCollection Cookies
        {
            get
            {
                return _cookies;
            }
        }
    }
}