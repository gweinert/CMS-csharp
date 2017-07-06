using CMS.Data;
// using CMS.Models;

namespace CMS.Services
{
    public class PageService
    {
        private readonly ApplicationDbContext _context;
        public PageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void UpdateEntityAndChildren()
        {

        }
    }
}