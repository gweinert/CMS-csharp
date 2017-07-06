using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CMS.Models;
using CMS.ViewModels;
using CMS.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CMS.Controllers
{
    public class PageElementGroupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PageElementGroupController(
                                        ApplicationDbContext context,
                                        UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        //used for testing need to see all pageELement Groups
        public async Task<IActionResult> Index(int? id)
        {
            var pageElGroups = await _context.PageElementGroups.ToArrayAsync();
            
            return View(pageElGroups);
        }

        [HttpGet]
        public IActionResult Create(int? id)
        {
            ViewData["PageID"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int? id, IFormCollection form)
        {
            int pageID = (int)id;
            
            Dictionary<string, PageElementType> pageElements = new Dictionary<string, PageElementType> {
                { "text", PageElementType.Text },
                { "blurb", PageElementType.Blurb },
                { "image", PageElementType.Image },
                { "link", PageElementType.Link }
            };
            List<PageElement> newPageEls = new List<PageElement>();

            //used for finding initial number of fields
            int elQuantity = 0;
            foreach(var el in pageElements)
            {
                if(form[el.Key].Count > 0)
                {
                    //build the amount selected
                    string elQuantityKey = el.Key + "-quant";
                    int tempQuantity = 0;
                    Int32.TryParse(form[elQuantityKey], out tempQuantity);
                    elQuantity += tempQuantity;
                }
            }

            PageElementGroup newPageElementGroup = new PageElementGroup
            {
                Name = "Group",
                PageID = pageID,
                NumberOfFields = elQuantity
            };

            _context.Add(newPageElementGroup);
            await _context.SaveChangesAsync();            
            
            var pageElIndex = 0;
            foreach(var el in pageElements)
            {
                if(form[el.Key].Count > 0)
                {
                    //build the amount selected
                    string elQuantityKey = el.Key + "-quant";
                    int localElQuantity = 0;
                    Int32.TryParse(form[elQuantityKey], out localElQuantity);
                    

                    for(int i = 0; i < localElQuantity; i++)
                    {
                        PageElement pageEl = new PageElement{
                            Type = el.Value,
                            GroupID = newPageElementGroup.ID,
                            PageID = pageID,
                            SortOrder = pageElIndex / newPageElementGroup.NumberOfFields
                        };
                        newPageEls.Add(pageEl);
                        _context.Add(pageEl);
                    }
                    
                    if(form[elQuantityKey] != "") //this is a pageGroupElement
                    {
                        pageElIndex++;
                    }
                }
                
            }

            // _context.Add(newPageEls);

            _context.SaveChanges();            
            
            // return View();
            return RedirectToAction("Edit", new { id = newPageElementGroup.ID });

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            int groupID = (int)id;

            var groupPageEls = _context.PageElements.Where(pe => pe.GroupID == groupID).OrderBy(pe => pe.SortOrder).ToArray();
            var group = _context.PageElementGroups.SingleOrDefault(g => g.ID == groupID);
            var pageElGroup = new PageElGroup{
                PageElementGroup = group,
                GroupedPageElements = groupPageEls
            };

            if(groupPageEls.Length == 0){
                return View(pageElGroup);
            }
            var pageID = groupPageEls[0].PageID;
            ViewData["PageID"] = pageID;

            // return View(groupPageEls);
            return View(pageElGroup);
            
        }

        [HttpPost]
        public IActionResult Edit(int? id, IFormCollection form)
        {
            //Need to loop through all form values and decide what to do with each value
            //values will be new page Elements
            var f = form;
            var pageElGroup = _context.PageElementGroups.SingleOrDefault(g => g.ID == id);
            pageElGroup.Name = form["Name"];

            foreach(var key in form.Keys)
            {
                var value = form[key];
                string[] keyValues = key.Split('-');
                if(keyValues.Length == 2) // a page el
                {
                    int keyID = Convert.ToInt32(keyValues[0]);
                    string keyFieldType = keyValues[1];

                    var pageEl = _context.PageElements.SingleOrDefault(pe => pe.ID == keyID);

                    if(keyFieldType == "name"){
                        pageEl.Name = value;
                    }
                    
                    if(pageEl.Type == PageElementType.Text)
                    {
                        pageEl.Body = value;
                    }else if(pageEl.Type == PageElementType.Blurb)
                    {
                        pageEl.Body = value;
                    }else if(pageEl.Type == PageElementType.Link)
                    {
                        if(keyFieldType == "linkTitle"){
                            pageEl.LinkTitle = value;
                        } else{
                            pageEl.Path = value;
                        }
                    }

                    _context.Update(pageEl);
                }
            }
            _context.Update(pageElGroup);
                

            _context.SaveChanges();
            
            int groupID = (int)id;
            
            var groupPageEls = _context.PageElements.Where(pe => pe.GroupID == groupID).OrderBy(pe => pe.SortOrder).ToArray();
            var group = _context.PageElementGroups.SingleOrDefault(g => g.ID == groupID);
            
            var pageGroup = new PageElGroup{
                PageElementGroup = group,
                GroupedPageElements = groupPageEls
            };
            
            var pageID = groupPageEls[0].PageID;
            ViewData["PageID"] = pageID;

            return View(pageGroup);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
           if(id == null)
            {
                return NotFound();
            }

            var pageElGroupToDelete = await _context.PageElementGroups
                .SingleOrDefaultAsync( p => p.ID == id);
            
            var pageElsToDelete = _context.PageElements.Where(pe => pe.GroupID == id);
            
            if(pageElGroupToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _context.PageElementGroups.Remove(pageElGroupToDelete);
                foreach( var pageEl in pageElsToDelete)
                {
                    _context.PageElements.Remove(pageEl);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddSlide(int? id)
        {
            var groupID = (int)id;

            try{
                var pageElGroup = await _context.PageElementGroups
                    .SingleOrDefaultAsync(pg => pg.ID == groupID);
                
                var pageElementsToDuplicate = _context.PageElements
                    .Where(pe => pe.GroupID == groupID)
                    .Take(pageElGroup.NumberOfFields);

                var numOfPageEls = _context.PageElements
                    .Where(pe => pe.GroupID == groupID).Count();
                

                foreach(var originalPageEl in pageElementsToDuplicate)
                {
                    PageElement newPageEl = new PageElement{
                        Type = originalPageEl.Type,
                        GroupID = originalPageEl.GroupID,
                        PageID = originalPageEl.PageID,
                        Name = originalPageEl.Name,
                        SortOrder =  ((numOfPageEls + pageElGroup.NumberOfFields) / pageElGroup.NumberOfFields) - 1
                    };
                    _context.Add(newPageEl);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Edit", new { id = groupID, saveChangesError = true });
            }
            
            
            return RedirectToAction("Edit", new { id = groupID});
        }

        [HttpGet]
        public IActionResult DeleteSlide(int? groupID, int? slideIndex)
        {
            if(groupID == null || slideIndex == null)
            {
                return NotFound();
            }
            
            // string currentUserID = _userManager.GetUserId(User);  

            try
            {
                var pageElsToDel = _context.PageElements
                                    .Where(pe => pe.GroupID == groupID && pe.SortOrder == slideIndex);

                var nextSlidesToUpdateSlideIndex = _context.PageElements
                    .Where(pe => pe.GroupID == groupID && pe.SortOrder > slideIndex);

                if(pageElsToDel == null)
                {
                    return NotFound();
                }

                // update all the preceding slides sort order since slide was deleted.
                if(nextSlidesToUpdateSlideIndex != null)
                {
                    // _context.PageElements.Update
                    foreach( var pageEl in nextSlidesToUpdateSlideIndex)
                    {
                        pageEl.SortOrder -= 1;
                    }

                    _context.PageElements.UpdateRange(nextSlidesToUpdateSlideIndex);
                }

                _context.PageElements.RemoveRange(pageElsToDel);

                _context.SaveChanges();
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Edit", new { id = groupID, saveChangesError = true });
            }

            return RedirectToAction("Edit", new { id = groupID, saveChangesError = true });
            
        }
    }
}