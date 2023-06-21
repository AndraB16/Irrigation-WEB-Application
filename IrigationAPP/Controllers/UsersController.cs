using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IrigationAPP.Data;
using IrigationAPP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BC = BCrypt.Net.BCrypt;

namespace IrigationAPP.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users model, string password)
        {
            if (ModelState.IsValid)
            {
                model.PasswordHash = BC.HashPassword(password);

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
         public async Task<IActionResult> Login(string username, string password)
         {
             var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

             if (user != null)
             {
                 if (BC.Verify(password, user.PasswordHash))
                 {
                     HttpContext.Session.SetString("User", user.Username);
                     HttpContext.Session.SetString("IsLoggedIn", "true");
                     HttpContext.Session.SetString("Username", user.Username);

                     return RedirectToAction("Index", "Home");
                 }
             }
             ModelState.AddModelError(string.Empty, "Invalid login attempt.");
             return View();
         }

         public IActionResult Logout()
         {
             HttpContext.Session.Remove("IsLoggedIn");

             return RedirectToAction("Index", "Home");
         }

    }
}
