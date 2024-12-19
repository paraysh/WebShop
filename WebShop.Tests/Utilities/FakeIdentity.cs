//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Security.Principal;

namespace MvcFakes
{
    // Diese Klasse stellt eine gefälschte Identität für Unit-Tests bereit.
    public class FakeIdentity : IIdentity
    {
        // Privates Feld zur Speicherung des Benutzernamens.
        private readonly string _name;

        // Konstruktor, der den Benutzernamen initialisiert.
        public FakeIdentity(string userName)
        {
            _name = userName;
        }

        // Eigenschaft, die den Authentifizierungstyp zurückgibt (nicht implementiert).
        public string AuthenticationType
        {
            get { throw new System.NotImplementedException(); }
        }

        // Eigenschaft, die angibt, ob der Benutzer authentifiziert ist.
        public bool IsAuthenticated
        {
            get { return !String.IsNullOrEmpty(_name); }
        }

        // Eigenschaft, die den Benutzernamen zurückgibt.
        public string Name
        {
            get { return _name; }
        }
    }
}