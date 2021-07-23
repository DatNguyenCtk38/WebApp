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
using WebApp.Service.UserS;

namespace WebApp.Pages.Document
{
    public class IndexModel : BaseRazorModel
    {
        private readonly IDocumentService _documentService;

        public IndexModel(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public IList<Models.Entity.Document> Documents { get;set; }
        public IFormFile UploadFile { get; set; }
        public async Task OnGetAsync()
        {
            Documents = await _documentService.GetDocumentsByUserId(UserID);
        }
        
        public async Task<ActionResult> OnPostAsync(IFormFile uploadFile)
        {
            if(uploadFile != null)
            {
                await _documentService.Upload(uploadFile, UserID);
            }
            return Redirect("/Document/Index");
        }
    }
}
