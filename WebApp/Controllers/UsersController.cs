using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Helper;
using WebApp.Models;
using WebApp.Models.Entity;
using WebApp.Service.UserS;

namespace WebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly WebAppContext _dbContext;
        private readonly IUserService _userService;
        public int UserID { get { return int.Parse(HttpContext.User.Claims.FirstOrDefault().Value); } }
        public UsersController(WebAppContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        // GET: Users/Create
        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Document/Index");
            }
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var isExistEmailUser = await _userService.CheckExistEmailUser(userViewModel.Email);
                if (isExistEmailUser)
                {
                    ViewBag.ErrorMessage = "The Email is existed";
                    return View(userViewModel);
                }
                var user = new User
                {
                    RegisterDate = DateTime.Now,
                    Birthdate = userViewModel.Birthdate,
                    Address = userViewModel.Address,
                    Email = userViewModel.Email,
                    FullName = userViewModel.FullName,
                    Password = EncryptHelper.EncryptPassword(userViewModel.Password),
                    Phone = userViewModel.Phone,
                    PersonalPhoto = FileHelper.SaveFile(userViewModel.PersonalPhoto)
                };

                await _userService.CreateNewUser(user);
                await StoreCookie(user);
                return Redirect("/Document/Index");
            }
            return View(userViewModel);
        }
        private async Task StoreCookie(User user)
        {
            var claims = new List<Claim>
                    {
                        new Claim("UserID", user.ID.ToString()),
                    };
            var claimsIdentity = new ClaimsIdentity(
               claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public IActionResult SignIn()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Document/Index");
            }
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.FindEmailUserByEmailAndPassword(userViewModel.Email, userViewModel.Password);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "The Email or Password in incorrect";
                    return View(userViewModel);
                }
                await StoreCookie(user);
                return Redirect("/Document/Index");
            }
            return View(userViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn");
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userService.GetUserById(UserID);
            var userViewModel = new UserViewModel
            {
                Address = user.Address,
                Birthdate = user.Birthdate,
                Email = user.Email,
                FullName = user.FullName,
                BytePersonalPhoto = user.PersonalPhoto,
                Phone = user.Phone
            };
            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Phone = userViewModel.Phone,
                        FullName = userViewModel.FullName,
                        Email = userViewModel.Email,
                        Birthdate = userViewModel.Birthdate,
                        Address = userViewModel.Address,
                        ID = UserID
                    };
                  
                    await _userService.UpdateUser(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Document/Index");
            }
            return View(userViewModel);
        }
        private bool UserExists(int id)
        {
            return _dbContext.Users.Any(e => e.ID == id);
        }
    }
}
