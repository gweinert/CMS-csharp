using System.Collections.Generic;
using System.Linq;
using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class SiteApiController : Controller 
    {
        private readonly ApplicationDbContext _context;

        public SiteApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult Index(int? id)
        {
            if(id == null)
            {
                return Json(new{success = 0});
            }


            // var content = _context.Pages
                // .Include( page => page.PageElements)
                // .Where(page => page.SiteID == id)

            var pages = from p in  _context.Pages.OrderBy(p => p.SortOrder).Where(page => page.SiteID == id)
                select new { 
                    ID = p.ID,
                    Title = p.Title,
                    SiteID = p.SiteID,
                    Link = p.Link,
                    PageElements = p.PageElements
                                    .GroupBy(pe => pe.GroupID)
                                    .Select(g => new{ groupID = g.Key, elements = g } )
                };

            // Dictionary<string, string> elements = new Dictionary<string, string>();
            // var pages = from p in  _context.Pages.OrderBy(p => p.SortOrder)
            //     select new { 
            //         ID = p.ID,
            //         Title = p.Title,
            //         SiteID = p.SiteID,
            //         Link = p.Link,
            //         PageElements = p.PageElements
            //                         .GroupBy(pe => pe.GroupID)
            //                         .Select(g => new{ groupID = g.Key, elements = g.Select(el => new { name = el.Name, value = el.Body != null ? el.Body : el.ImagePath }) } )
            //     };


            var groups = _context.PageElementGroups
                .Where(g => pages.Any(page => page.ID == g.PageID));

            var content = new { pages = pages, groups = groups};


            if(content == null)
            {
                return Json(new{success=0});
            }
            
            return Json(new{success = 1, content = content});
        }
    }
}