using System.Collections.Generic;

namespace CMS.Models
{
    public class PageElementGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int PageID { get; set; }
        public int NumberOfFields { get; set; }

        // public IList<PageElement> PageElements { get; set; }

    }
}