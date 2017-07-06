using System.Collections.Generic;

namespace CMS.Models
{
    public class Site
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public string UserID { get; set; }

        public ICollection<Page> Pages { get; set; }
    }
}