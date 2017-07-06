using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CMS.Initialisations;
using Server.Models;
using Microsoft.AspNetCore.Cors;

namespace CMS.Controllers
{
     [Route("api/[controller]")]
     [EnableCors("AllowSpecificOrigin")]

     public class ContactController : Controller
     {
        private readonly EmailSettings _emailSettings;

        public ContactController(IOptions<EmailSettings> emailOptions){
            _emailSettings = emailOptions.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(IFormCollection form)
        {
            if (form == null){
			        return Json( new {success=0} );
			}

            var toName = "Garrett Portfolio Page Contact";
            var toEmail = _emailSettings.ToEmail;
            var fromName = form["name"];
            // var fromEmail = form.["email"];
            var fromNameSpaceRemoved = Regex.Replace(fromName, @"\s+", "_");
            var fromEmail = fromNameSpaceRemoved + "_" + _emailSettings.Login; 
            
            var subject = form["subject"];
            var body = "From: " + form["email"] +
             "\n \n" + form["message"];
            
            var success = Email.SendMessageSmtp(
                                                _emailSettings.Login, 
                                                _emailSettings.Pass, 
                                                toName, 
                                                toEmail, 
                                                fromName, 
                                                fromEmail, 
                                                subject, 
                                                body
                                                );

            if(success) {
                return Json( new {success=1} );
            } else {
                return Json( new {success=0}) ;
            }
        
        }
     }
}