using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CMS.Models;
using CMS.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CMS.Controllers
{
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public PageController(
                                ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create(int? id, int? pageID)
        {
            ViewData["ParentID"] = pageID != null ? pageID : 0;
            ViewData["SiteID"] = id;
            return View();
        }

        [HttpPost]
        public IActionResult Create(
            [Bind("Title, Link, SiteID, ParentID")] Page page) //BIND so SiteID && ID arent the same
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var parentPage = _context.Pages.SingleOrDefault(p => p.ID == page.ParentID);
                    if(parentPage != null){
                        if(parentPage.ChildPages == null)
                        {
                            parentPage.ChildPages = new List<Page>();
                        }
                        parentPage.ChildPages.Add(page);
                        _context.Update(parentPage);

                    }
                    _context.Add(page);
                    _context.SaveChanges();
                    return RedirectToAction("Edit", new { id = page.ID } );
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(page);
        }

        // [HttpGet]
        // public IActionResult CreateSubPage(int? id)
        // {

        //     return RedirectToAction("Edit", new { id = id });            
        // }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string currentUserID = _userManager.GetUserId(User);  
            var userSites = _context.Sites.Where(s => s.UserID == currentUserID);
            
            var pageToUpdate = await _context.Pages
                .Where(p => userSites.Any( s => s.ID == p.SiteID))
                .Include(p => p.PageElements )
                .SingleOrDefaultAsync(page => page.ID == id  );

            if (pageToUpdate == null)
            {
                return NotFound();
            }

            pageToUpdate.PageElements = pageToUpdate.PageElements.OrderBy(pe => pe.SortOrder).ToList();

            var editPage = new EditPageElements{
                Page = pageToUpdate,
                PageElements = pageToUpdate.PageElements.Where(pe => pe.GroupID == null).ToList(),
                GroupPageElements = pageToUpdate.PageElements.Where(pe => pe.GroupID != null).ToList()
            };

            return View(editPage);
        }

        [HttpPost, ActionName("Edit")]
        // public async Task<IActionResult> EditPage(int? id, Page page)
        public async Task<IActionResult> EditPage(int? id, EditPageElements editPageElements)
        
        {
            var page = editPageElements.Page;
            if(id == null)
            {
                return NotFound();
            }

             if (ModelState.IsValid)
            {
                try
                {   

                    var pageToUpdate = _context.Pages
                        .Include(p => p.PageElements)
                        .SingleOrDefault( p => p.ID == id);
                    
                    pageToUpdate.Link = page.Link;
                    pageToUpdate.Title = page.Title;
                    pageToUpdate.SortOrder = page.SortOrder;

                    //only editing page direct elements, not grouped Elements
                    var pageElementsToEdit = pageToUpdate.PageElements.Where(p => p.GroupID == null);
                    // var editedPageElements = page.PageElements.Where(p => p.GroupID == null);

                    _context.PageElements.RemoveRange(pageElementsToEdit);
                    await _context.SaveChangesAsync();

                    if(editPageElements.PageElements != null){

                        for(int i = 0; i < editPageElements.PageElements.Count; i++)
                        {
                            pageToUpdate.PageElements.Add(editPageElements.PageElements[i]);
                        }
                    
                    }
                    
                    _context.Update(pageToUpdate);

                    var editPage = new EditPageElements {
                        Page = pageToUpdate,
                        PageElements = editPageElements.PageElements.Where(pe => pe.GroupID == null).ToList(),
                        GroupPageElements = pageToUpdate.PageElements.Where(pe => pe.GroupID != null).ToList()
                    };

                    await _context.SaveChangesAsync();

                    return View(editPage);
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }



            return RedirectToAction("Edit", new { id = id });
            
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            var pageToDelete = await _context.Pages
                .SingleOrDefaultAsync( p => p.ID == id);
            
            if(pageToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _context.Pages.Remove(pageToDelete);
                await _context.SaveChangesAsync();
                return RedirectToAction("Detail", "Site", new { id = pageToDelete.SiteID});
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

        }
    }
}