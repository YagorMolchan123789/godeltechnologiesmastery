using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GTE.Mastery.ShoeStore.Web.Helpers
{
    [HtmlTargetElement("page-link")]
    public class PaginationTagHelper : TagHelper
    {
        IUrlHelperFactory urlHelperFactory;
        public PaginationTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        [HtmlAttributeName("page-model")]
        public PageViewModel? PageModel { get; set; }

        [HtmlAttributeName("page-action")]
        public string PageAction { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (PageModel == null)
            {
                throw new Exception("PageModel is not set");
            }

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            TagBuilder currentItem = CreateTag(urlHelper, PageModel.PageNumber);

            if (PageModel.HasPreviousPage)
            {
                TagBuilder prevItem = CreateTag(urlHelper, PageModel.PageNumber - 1);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            tag.InnerHtml.AppendHtml(currentItem);

            if (PageModel.HasNextPage)
            {
                TagBuilder nextItem = CreateTag(urlHelper, PageModel.PageNumber + 1);
                tag.InnerHtml.AppendHtml(nextItem);
            }

            output.Content.AppendHtml(tag);
        }

        private TagBuilder CreateTag(IUrlHelper urlHelper, int pageNumber = 1)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == PageModel?.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(PageAction, new { page = pageNumber });
            }
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }


    }
}
