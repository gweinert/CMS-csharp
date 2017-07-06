namespace CMS.Models
{
    public class Text : PageElement
    {
        public override PageElementType Type { get; set; } = PageElementType.Text;

        public Text(): base() {} 

        public Text(string name, int pageID) : base(name, pageID) {}

    }
}