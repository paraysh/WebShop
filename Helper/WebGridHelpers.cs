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
    public static class WebGridHelpers
    {
        public static HtmlString WebGridFilter<T>(this HtmlHelper helper,
        IEnumerable<OrderModel> users, Func<OrderModel, string> property,
            string headingText) where T : class
        {
            var model = new WebGridFilterModel
            {
                OrderBy = users.GroupBy(property).Select(g => g.First()),
                Property = property,
                HeadingText = headingText
            };
            return helper.Partial("_webGridFilter", model);
        }
    }
}