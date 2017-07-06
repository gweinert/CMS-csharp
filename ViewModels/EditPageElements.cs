using System.Collections.Generic;
using CMS.Models;

namespace CMS.ViewModels
{
    public class EditPageElements
    {
        public Page Page { get; set; }
        public List<PageElement> PageElements { get; set; }

        public List<PageElement> GroupPageElements { get; set; }

    }
}