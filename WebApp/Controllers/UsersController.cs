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

namespace WebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly WebAppContext _dbContext;
        public UsersController(WebAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Users
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult SignUp()
        {
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
                var user = new User
                {
                    RegisterDate = DateTime.Now,
                    Birthdate = userViewModel.Birthdate,
                    Address = userViewModel.Address,
                    Email = userViewModel.Email,
                    FullName = userViewModel.FullName,
                    Password = EncryptHelper.EncryptPassword(userViewModel.Password),
                    Phone = userViewModel.Phone,
                    PersonalPhoto = FileHelper.EncryptPassword(userViewModel.PersonalPhoto)
                };

                _dbContext.Add(user);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userViewModel);
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
                var user = _dbContext.Users.FirstOrDefault(x => x.Email == userViewModel.Email && x.Password == EncryptHelper.EncryptPassword(userViewModel.Password));
                if (user == null)
                {
                    ViewBag.ErrorMessage = "The Email or Password in incorrect";
                    return View(userViewModel);
                }

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


                return RedirectToAction(nameof(Index));
            }
            return View(userViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn");
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Birthdate,Email,Password,Phone,Address,PersonalPhoto,RegisterDate")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _dbContext.Users.Any(e => e.ID == id);
        }
    }
}
