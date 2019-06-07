using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BookingWebsite.TagHelpers
{

    /// <summary>
    /// we need to define what will be target element for this
    /// - in other words, this page tag helpers will be assigned inside div tag in View, and add atribute of page-model
    /// </summary>
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {

        // usind dependency injection
        // we need url helper factory - to build URLs
        private IUrlHelperFactory urlHelperFactory;


        //constr
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;

        }
        //TODO description


        [ViewContext] // View context provides access to things like httpContext, http request / response...
        [HtmlAttributeNotBound] 
        public ViewContext ViewContext { get; set; }


        // attributes that we need for pagination
        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        /// <summary>
        /// function for the processing 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // it will fetch the urs inside the helper
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i=1; i<=PageModel.totalPage;i++)
            {
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.urlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;

                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i==PageModel.CurrentPage? PageClassSelected : PageClassNormal );
                }

                // numbering
                tag.InnerHtml.Append(i.ToString());

                result.InnerHtml.AppendHtml(tag);

            }

            // append everything in the div tag - we are using TagHelperOutput
            // and modifying by adding html that we have created
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
