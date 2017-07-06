using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CMS.Models;
using CMS.ViewModels;
using CMS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    public class SiteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SiteController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
                     
        }

        public IActionResult Index()
        {
             string currentUserID = _userManager.GetUserId(User);  
            IEnumerable<Site> userSites = _context.Sites.Where(s => s.UserID == currentUserID);
            
            return View(userSites);
        }

        public IActionResult Detail(int? id)
        {
            
            if(id == null)
            {
                NotFound();
            }

            string currentUserID = _userManager.GetUserId(User);  
            Site site = _context.Sites.Where(s => s.UserID == currentUserID)
                .Include( s => s.Pages )
                .SingleOrDefault( s => s.ID == id);

            SitePages sitePages = new SitePages{Site = site, PageTrees = new List<PageTree>() };
            
            // foreach( var page in site.Pages)
            // {
            //     if(page.ParentID == 0)
            //     {
            //         var topPage = new PageTree{Page = page};
            //         sitePages.PageTrees.Add(topPage);
            //     } else 
            //     {
            //         var pageInSecondLevel = true;
            //         foreach(var pageTree in sitePages.PageTrees)
            //         {   
            //             pageInSecondLevel = true;
            //             if(pageTree.Page.ID == page.ParentID)
            //             {
            //                 pageInSecondLevel = false;
            //                 if(pageTree.ChildPages == null)
            //                 {
            //                     pageTree.ChildPages = new List<PageTree>();
            //                 }
            //                 PageTree newPageTree = new PageTree{Page = page};
            //                 pageTree.ChildPages.Add(newPageTree);
            //             }
            //         }

            //         if(pageInSecondLevel){
            //             foreach(var pageTree in sitePages.PageTrees){

            //                 if(pageTree.ChildPages != null){

            //                     foreach(var secondLevelPage in pageTree.ChildPages)
            //                     {
            //                         if(page.ParentID == secondLevelPage.Page.ID)
            //                         {
            //                             if(secondLevelPage.ChildPages == null)
            //                             {
            //                                 secondLevelPage.ChildPages = new List<PageTree>();
            //                             }
            //                             PageTree newPageTree = new PageTree{Page = page};
            //                             secondLevelPage.ChildPages.Add(newPageTree);
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }
            // }
        

            // if(sitePages ==  null)
            if(site == null)
            {
                NotFound();
            }

            // return View(sitePages);
            return View(site);
            

        }
    }
}