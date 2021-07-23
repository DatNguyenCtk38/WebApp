using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Pages.Document
{
    public class PrintPageModel : BaseRazorModel
    {
        private readonly WebAppContext _context;
        public PrintPageModel(WebAppContext context)
        {
            _context = context;
        }
        public IList<Models.Entity.Document> Documents { get; set; }
        public Models.Entity.User UserInfomation { get; set; }
        public async Task OnGet()
        {
            UserInfomation = await _context.Users.Include(x=> x.Documents).FirstOrDefaultAsync(x => x.ID == UserID);
            Documents = UserInfomation.Documents.ToList();
        }
    }
}
