using System;
using System.Text;
using System.Web.Mvc;

namespace Web.Models
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinksForAdminContent(this HtmlHelper html,
            PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                for (int i = 1; i <= pageInfo.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    tag.MergeAttribute("onclick", "UpdateAdminContent(this.value, " + i + ")");
                    tag.InnerHtml = i.ToString();
                    
                    if (i == pageInfo.PageNumber)
                    {
                        tag.AddCssClass("selected");
                        tag.AddCssClass("btn-primary");
                    }
                    tag.AddCssClass("btn btn-default");
                    result.Append(tag.ToString());
                }
            }
            catch { }
            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString PageLinksForManagerContent(this HtmlHelper html,
            PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                for (int i = 1; i <= pageInfo.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    tag.MergeAttribute("onclick", "UpdateManagerContent(" + i + ")");
                    tag.InnerHtml = i.ToString();
                    
                    if (i == pageInfo.PageNumber)
                    {
                        tag.AddCssClass("selected");
                        tag.AddCssClass("btn-primary");
                    }
                    tag.AddCssClass("btn btn-default");
                    result.Append(tag.ToString());
                }
            }
            catch { }
            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString PageLinksForUserContent(this HtmlHelper html,
            PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();

            try
            {
                for (int i = 1; i <= pageInfo.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    tag.MergeAttribute("onclick", "UpdateUserContent(" + i + ")");
                    tag.InnerHtml = i.ToString();
                    
                    if (i == pageInfo.PageNumber)
                    {
                        tag.AddCssClass("selected");
                        tag.AddCssClass("btn-primary");
                    }
                    tag.AddCssClass("btn btn-default");
                    result.Append(tag.ToString());
                }
            }
            catch { }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}