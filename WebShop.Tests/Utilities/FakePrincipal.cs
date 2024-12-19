//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Linq;
using System.Security.Principal;

namespace MvcFakes
{
    // Diese Klasse stellt einen gefälschten Principal für Unit-Tests bereit.
    public class FakePrincipal : IPrincipal
    {
        // Private Felder zur Speicherung der Identität und Rollen.
        private readonly IIdentity _identity;
        private readonly string[] _roles;

        // Konstruktor, der die Identität und Rollen initialisiert.
        public FakePrincipal(IIdentity identity, string[] roles)
        {
            _identity = identity;
            _roles = roles;
        }

        // Eigenschaft, die die Identität zurückgibt.
        public IIdentity Identity
        {
            get { return _identity; }
        }

        // Methode, die überprüft, ob der Principal in einer bestimmten Rolle ist.
        public bool IsInRole(string role)
        {
            if (_roles == null)
                return false;
            return _roles.Contains(role);
        }
    }
}