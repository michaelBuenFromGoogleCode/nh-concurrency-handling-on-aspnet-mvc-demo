using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Ienablemuch
{

    class Test
    {
        public static void Main()
        {
            Type t = typeof(Test);
            string s = t.Assembly.FullName.ToString();
            Console.WriteLine("The fully qualified assembly name " +
                "containing the specified class is {0}.", s);

            Console.ReadLine();
        }
    }

    public static class ValidationExtension
    {

        public static object ToJsonValidation(this ModelStateDictionary modelState)
        {
            var v = from m in modelState.AsEnumerable()
                    from e in m.Value.Errors
                    select new { m.Key, e.ErrorMessage };

            return new
            {
                IsValid = modelState.IsValid,
                PropertyErrors = v.Where(x => !string.IsNullOrEmpty(x.Key)),
                ModelErrors = v.Where(x => string.IsNullOrEmpty(x.Key))
            };
        }


        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ValidationSummary();
        }


        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors)
        {
            if (!htmlHelper.ViewData.ModelState.IsValid || !excludePropertyErrors)
                return htmlHelper.ValidationSummary(excludePropertyErrors);
            else
            {
                return htmlHelper.BuildValidationSummary(null, null);
            }
        }

        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper, string message)
        {
            return htmlHelper.ValidationSummary(message);
        }

        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors, string message)
        {
            if (!htmlHelper.ViewData.ModelState.IsValid || !excludePropertyErrors)
                return htmlHelper.ValidationSummary(excludePropertyErrors, message);
            else
                return htmlHelper.BuildValidationSummary(message, (IDictionary<string, object>)null);

        }


        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper, string message, object htmlAttributes)
        {
            return htmlHelper.ValidationSummary(message, htmlAttributes);
        }


        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors, string message, object htmlAttributes)
        {
            if (!htmlHelper.ViewData.ModelState.IsValid || !excludePropertyErrors)
                return htmlHelper.ValidationSummary(excludePropertyErrors, message, htmlAttributes);
            else
                return htmlHelper.BuildValidationSummary(message, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper, string message, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ValidationSummary(message, htmlAttributes);
        }

        public static MvcHtmlString JsAccessibleValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors, string message, IDictionary<string, object> htmlAttributes)
        {
            if (!htmlHelper.ViewData.ModelState.IsValid || !excludePropertyErrors)
                return htmlHelper.ValidationSummary(excludePropertyErrors, message, htmlAttributes);
            else
                return htmlHelper.BuildValidationSummary(message, htmlAttributes);
        }


        private static MvcHtmlString BuildValidationSummary(this HtmlHelper htmlHelper, string message, IDictionary<string, object> htmlAttributes)
        {
            // excludePropertyErrors is always true here

            string messageSpan;
            if (!String.IsNullOrEmpty(message))
            {
                TagBuilder spanTag = new TagBuilder("span");
                spanTag.SetInnerText(message);
                messageSpan = spanTag.ToString(TagRenderMode.Normal) + Environment.NewLine;
            }
            else
            {
                messageSpan = null;
            }

            TagBuilder divBuilder = new TagBuilder("div");

            divBuilder.AddCssClass(HtmlHelper.ValidationSummaryValidCssClassName);
            divBuilder.InnerHtml = messageSpan + "<ul></ul>";


            // I use false, so jQuery won't be confused putting property validation on summary
            divBuilder.MergeAttribute("data-valmsg-summary", "false");

            divBuilder.MergeAttributes(htmlAttributes);


            return divBuilder.ToMvcHtmlString(TagRenderMode.Normal);
        }

        // copied from ASP.NET MVC. so many internals in ASP.NET MVC :-)
        private static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
        {
            System.Diagnostics.Debug.Assert(tagBuilder != null);
            return new MvcHtmlString(tagBuilder.ToString(renderMode));
        }

    }
}
