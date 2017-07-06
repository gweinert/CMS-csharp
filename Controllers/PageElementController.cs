using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;
using CMS.Services;
using Google.Cloud.Storage.V1;

namespace CMS.Controllers
{
    public class PageElementController : Controller 
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUploader _imageUploader;
        public PageElementController(ApplicationDbContext context)
        // , ImageUploader imageUploader)
        {
            _context = context;
            // _imageUploader = imageUploader;
        }

        //Used for testing page elements are deleted on page delete SQL
        public async Task<IActionResult> Index()
        {
            var pages = await _context.PageElements.ToArrayAsync();
            
            return View(pages);
        }

        [HttpGet]
        public IActionResult Create(int? id)
        {
            ViewData["PageID"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("Name, Type, PageID")] PageElement pageElement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var parentPage = await _context.Pages
                        .Include(p => p.PageElements)
                        .SingleOrDefaultAsync(p => p.ID == pageElement.PageID);

                    var sortOrderNum = parentPage.PageElements.Count;
                    pageElement.SortOrder = sortOrderNum;
                    
                    _context.Add(pageElement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", "Page", new { id = pageElement.PageID });
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(pageElement);

        }

        [HttpGet]
        public IActionResult CreateImage(int? id)
        {
            ViewData["PageID"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImage(int? id, IFormCollection form)
        {
            StorageClient storageClient = StorageClient.Create();
            string bucketName = "garrettcmstest";

            long size = form.Files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in form.Files)
            {
                if (formFile.Length > 0)
                {

                    var imageAcl = PredefinedObjectAcl.PublicRead;

                    var imageObject = await storageClient.UploadObjectAsync(
                        bucket: bucketName,
                        objectName: formFile.FileName,
                        contentType: formFile.ContentType,
                        source: formFile.OpenReadStream(),
                        options: new UploadObjectOptions { PredefinedAcl = imageAcl }
                    );

                    var googleImagePath = imageObject.MediaLink;

                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var parentPage = await _context.Pages
                                .Include(p => p.PageElements)
                                .SingleOrDefaultAsync(p => p.ID == id);

                            var sortOrderNum = parentPage.PageElements.Count;
                            
                            //new page Element to save in db
                            int pageID = (int)id;
                            Int32.TryParse(form["PageID"], out pageID);
                            
                            PageElement pageEl = new PageElement
                            { 
                                Type = PageElementType.Image,
                                Name = form["Name"],
                                ImagePath = googleImagePath,
                                SortOrder = sortOrderNum,
                                PageID = pageID
                            };
                            
                            _context.Add(pageEl);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "Page", new { id = id });
                        }
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists " +
                            "see your system administrator.");
                    }
                }
            }
            
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            StorageClient storageClient = StorageClient.Create();
            string bucketName = "garrettcmstest";

            if(id == null)
            {
                return NotFound();
            }

            var pageElToDelete = await _context.PageElements
                .SingleOrDefaultAsync( p => p.ID == id);
            
            if(pageElToDelete == null)
            {
                return NotFound();
            }

            try
            {
                // if its an image delete from google cloud
                if(pageElToDelete.ImagePath != null)
                {
                    await storageClient.DeleteObjectAsync(bucketName, pageElToDelete.FileName);
                }
                _context.PageElements.Remove(pageElToDelete);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Page", new { id = pageElToDelete.PageID});
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

        }
    }
}