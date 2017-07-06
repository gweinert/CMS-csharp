using System.Collections.Generic;
using CMS.Models;

namespace CMS.ViewModels
{
    public class SitePages
    {
        public Site Site { get; set; }

        public List<PageTree> PageTrees { get; set; }

        // public List<Dictionary<int, List<Page>>> Pages { get; set; }

    }

    public class PageTree
    {
        // public Dictionary<Page, Page[]> PageSubpage { get; set; }
        public Page Page { get; set; }
        public List<PageTree> ChildPages { get; set; }
    }
}