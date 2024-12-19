//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Tests.Utilities
{
    // Diese Klasse stellt einen TestPrincipal für Unit-Tests bereit, der von ClaimsPrincipal erbt.
    public class TestPrincipal : ClaimsPrincipal
    {
        // Konstruktor, der eine Reihe von Claims akzeptiert und eine neue TestIdentity erstellt.
        public TestPrincipal(params Claim[] claims) : base(new TestIdentity(claims))
        {
        }
    }

    // Diese Klasse stellt eine TestIdentity für Unit-Tests bereit, die von ClaimsIdentity erbt.
    public class TestIdentity : ClaimsIdentity
    {
        // Konstruktor, der eine Reihe von Claims akzeptiert.
        public TestIdentity(params Claim[] claims) : base(claims)
        {
        }
    }
}