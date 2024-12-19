//Bearbeiter: Yusuf Can Sönmez 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Helper
{
    /// <summary>
    /// Die CommaHandler-Klasse bietet Methoden zum Hinzufügen von Kommata zu Zeichenfolgen.
    /// </summary>
    public static class CommaHandler
    {
        /// <summary>
        /// Fügt ein Komma zu einer gegebenen Zeichenfolge hinzu, falls erforderlich.
        /// </summary>
        /// <param name="input">Die Eingabezeichenfolge, zu der ein Komma hinzugefügt werden soll.</param>
        /// <returns>Die modifizierte Zeichenfolge mit einem Komma.</returns>
        public static string AddComma(string input)
        {
            // Überprüft, ob die Eingabe bereits ein Komma enthält
            if (input.IndexOf(",") > 0)
            {
                // Überprüft, ob nach dem Komma nur eine Ziffer steht
                if (input.Split(',')[1].Length == 1)
                {
                    // Fügt eine Null hinzu, um zwei Dezimalstellen zu haben
                    return input + "0";
                }
                return input;
            }
            else
            {
                // Fügt ",00" hinzu, wenn kein Komma vorhanden ist
                return input + ",00";
            }
        }
    }
}