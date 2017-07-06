using System.Collections.Generic;

namespace CMS.Models
{
    public class Page
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public int SiteID { get; set; }
        public int SortOrder { get; set; }
        public int ParentID { get; set; }
        public IList<PageElement> PageElements { get; set; }

        public IList<Page> ChildPages {get; set; }
        
    }
}