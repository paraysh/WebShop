using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    /// <summary>
    /// Repräsentiert ein Nachrichtenmodell im WebShop.
    /// Diese Klasse enthält Eigenschaften, die für die Anzeige von Nachrichten an den Benutzer erforderlich sind.
    /// </summary>
    public class MessageVM
    {
        /// <summary>
        /// Die CSS-Klassenname, der für die Formatierung der Nachricht verwendet wird.
        /// </summary>
        public string CssClassName { get; set; }

        /// <summary>
        /// Der Titel der Nachricht.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Der Inhalt der Nachricht.
        /// </summary>
        public string Message { get; set; }
    }
}