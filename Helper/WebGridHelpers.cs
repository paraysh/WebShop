using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using WebShop.Models;

namespace WebShop.Helper
{
    /// <summary>
    /// Die WebGridHelpers-Klasse enthält Hilfsfunktionen zur Unterstützung der WebGrid-Ansicht im WebShop.
    /// Sie bietet eine Funktion zum Erstellen eines Filters für eine WebGrid-Ansicht.
    /// </summary>
    public static class WebGridHelpers
    {
        /// <summary>
        /// Erstellt einen Filter für eine WebGrid-Ansicht basierend auf einer bestimmten Eigenschaft.
        /// </summary>
        /// <typeparam name="T">Der Typ der Daten, die gefiltert werden.</typeparam>
        /// <param name="helper">Das HtmlHelper-Objekt, das die Erweiterungsmethode aufruft.</param>
        /// <param name="users">Die Liste der OrderModel-Objekte, die gefiltert werden sollen.</param>
        /// <param name="property">Die Eigenschaft, nach der gefiltert werden soll.</param>
        /// <param name="headingText">Der Überschriftstext für die Filterspalte.</param>
        /// <returns>Ein HtmlString, der das Partial-View für den Filter rendert.</returns>
        public static HtmlString WebGridFilter<T>(this HtmlHelper helper,
        IEnumerable<OrderModel> users, Func<OrderModel, string> property,
            string headingText) where T : class
        {
            // Erstellt ein Modell für den WebGrid-Filter
            var model = new WebGridFilterModel
            {
                // Gruppiert die Benutzer nach der angegebenen Eigenschaft und wählt das erste Element jeder Gruppe aus
                OrderBy = users.GroupBy(property).Select(g => g.First()),
                Property = property,
                HeadingText = headingText
            };
            // Rendert das Partial-View für den Filter und gibt es als HtmlString zurück
            return helper.Partial("_webGridFilter", model);
        }
    }
}