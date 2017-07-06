using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CMS.Models;
using System;
using Microsoft.EntityFrameworkCore;
using CMS.Data;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection form)
        {
            StorageClient storageClient = StorageClient.Create();
            string bucketName = "garrettcmstestdev";

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
                            int pageID = 0;
                            int pageElementID = 0;
                            Int32.TryParse(form["PageID"], out pageID);
                            Int32.TryParse(form["PageElementID"], out pageElementID);
                            

                            //need to find the image page element and update it in db

                            var pageElementToEdit = await _context.PageElements
                                .SingleOrDefaultAsync( p => p.ID == pageElementID);

                            if(pageElementToEdit == null)
                            {
                                return Json(new{success = 0});
                            }
                            
                            pageElementToEdit.ImagePath = googleImagePath;
                            pageElementToEdit.FileName = formFile.FileName;

                            
                            _context.Update(pageElementToEdit);
                            await _context.SaveChangesAsync();
                            return Json(new{success = 1, imagePath = googleImagePath});
                            
                            // return RedirectToAction("Edit", "Page", new { id = id });
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
            return Json(new{ success = 0 });
        }

    }
}
