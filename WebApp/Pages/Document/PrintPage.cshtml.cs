using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Service.UserS;

namespace WebApp.Pages.Document
{
    public class PrintPageModel : BaseRazorModel
    {
        private readonly IUserService _userService;
        public PrintPageModel(IUserService userService)
        {
            _userService = userService;
        }
        public IList<Models.Entity.Document> Documents { get; set; }
        public Models.Entity.User UserInfomation { get; set; }
        public async Task OnGet()
        {
            UserInfomation = await _userService.GetUserById(UserID);
            Documents = UserInfomation.Documents.ToList();
        }
    }
}
