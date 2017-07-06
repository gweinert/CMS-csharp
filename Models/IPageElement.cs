namespace CMS.Models
{
    public enum PageElementType
    {
        Text,
        Blurb,
        Link,
        Image
    }
    public interface IPageElement
    {
        int ID { get; set; }
        PageElementType Type { get; set; }
        string Name { get; set; }
        string Body { get; set; }
        int PageID { get; set; }
        int? GroupID { get; set; }
    }
}