using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Pages
{
    public class BaseRazorModel : PageModel
    {
        public int UserID { get { return int.Parse(HttpContext.User.Claims.FirstOrDefault().Value); } }
    }
}
