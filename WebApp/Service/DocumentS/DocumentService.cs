using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Helper;
using WebApp.Models.Entity;

namespace WebApp.Service.UserS
{
    public class DocumentService : IDocumentService
    {
        private readonly WebAppContext _dbContext;
        public DocumentService(WebAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Document>> GetDocumentsByUserId(int userId)
        {
            return await _dbContext.Documents.Where(x => x.UserID == userId).ToListAsync();
        }

        public async Task Upload(IFormFile file, int userId)
        {
            //Set Key Name
            string documentName = DateTime.Now.Millisecond + file.FileName;
            var savedPath = $"wwwroot/document/Document_UserID_{userId}";
            Directory.CreateDirectory(savedPath);
            //Get url To Save
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), savedPath, documentName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var document = new Models.Entity.Document
            {
                Name = documentName,
                Type = Path.GetExtension(file.FileName),
                UploadDate = DateTime.Now,
                UserID = userId,
                Size = file.Length.ToString()
            };
            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();
        }
    }
}
