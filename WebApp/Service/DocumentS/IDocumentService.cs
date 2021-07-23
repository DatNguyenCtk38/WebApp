using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models.Entity;

namespace WebApp.Service.UserS
{
    public interface IDocumentService
    {
        public Task<List<Document>> GetDocumentsByUserId(int userId);
        public Task Upload(IFormFile file, int userId);
    }
}
