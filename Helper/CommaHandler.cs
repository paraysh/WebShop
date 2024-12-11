using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Helper
{
    public static class CommaHandler
    {
        public static string AddComma(string input)
        {
            if (input.IndexOf(",") > 0)
            {
                if (input.Split(',')[1].Length == 1)
                {
                    return input + "0";
                }
                return input;
            }
            else
            {
                return input + ",00";
            }
        }
    }
}