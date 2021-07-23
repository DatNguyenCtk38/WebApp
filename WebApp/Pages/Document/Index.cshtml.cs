using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models.Entity;

namespace WebApp.Pages.Document
{
    public class IndexModel : BaseRazorModel
    {
        private readonly WebAppContext _context;

        public IndexModel(WebAppContext context)
        {
            _context = context;
        }

        public IList<Models.Entity.Document> Documents { get;set; }
        public IFormFile UploadFile { get; set; }
        public async Task OnGetAsync()
        {
            Documents = await _context.Documents.Where(x => x.UserID == UserID).ToListAsync();
        }
        
        public async Task<ActionResult> OnPostAsync(IFormFile uploadFile)
        {
            if(uploadFile != null)
            {
                //Set Key Name
                string documentName = DateTime.Now.Millisecond + uploadFile.FileName;
                var savedPath = $"wwwroot/document/Document_UserID_{UserID}";
                Directory.CreateDirectory(savedPath);
                //Get url To Save
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), savedPath, documentName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                   await uploadFile.CopyToAsync(stream);
                }
                var document = new Models.Entity.Document
                {
                    Name = documentName,
                    Type = Path.GetExtension(uploadFile.FileName),
                    UploadDate = DateTime.Now,
                    UserID = UserID,
                    Size = uploadFile.Length.ToString()
                };
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
            }
            return Redirect("/Document/Index");
        }
    }
}
