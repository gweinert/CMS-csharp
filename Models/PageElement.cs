
namespace CMS.Models
{
    
    public class PageElement : IPageElement
    {
        public int ID { get; set; }
        public virtual PageElementType Type { get; set; }
        public virtual string Name { get; set; }
        public string Body { get; set; }
        public string LinkTitle { get; set; }
        public string Path { get; set; }
        public string ImagePath { get; set; }
        public string FileName { get; set; }
        public int SortOrder { get; set; }
        public int PageID { get; set; }
        public int? GroupID { get; set; }

        public PageElement()
        {

        }
        public PageElement(string name, int pageID)
        {
            Name = name;
            PageID = pageID;
        }
    }
}