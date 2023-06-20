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
        /* [HttpPost]
         public async Task<IActionResult> Register(Users model)
         {
             // Check if the provided username or email already exists
             var existingUser = await _context.Users.FirstOrDefaultAsync(
                 u => u.Username == model.Username || u.Email == model.Email);

             if (existingUser != null)
             {
                 ModelState.AddModelError("", "Username or email already exists.");
                 return View(model);
             }

             // Hash the password
             var passwordHasher = new PasswordHasher<Users>();
             model.PasswordHash = passwordHasher.HashPassword(model, model.PasswordHash);

             // Save the new user to the database
             _context.Users.Add(model);
             await _context.SaveChangesAsync();

             // Redirect the user to the login page
             return RedirectToAction("Login");
         }
         [HttpPost]
         public async Task<IActionResult> Login(string username, string password)
         {
             // Find the user with the provided username
             var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

             if (user == null)
             {
                 ModelState.AddModelError("", "Invalid username or password.");
                 return View();
             }

             // Verify the password
             var passwordHasher = new PasswordHasher<Users>();
             var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

             if (passwordVerificationResult == PasswordVerificationResult.Failed)
             {
                 ModelState.AddModelError("", "Invalid username or password.");
                 return View();
             }

             // Log the user in
             // ...

             // Redirect the user to the home page
             return RedirectToAction("Index", "Home");
         }*/
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users model, string password)
        {
            if (ModelState.IsValid)
            {
                // Hash the password
                model.PasswordHash = BC.HashPassword(password);

                // Save the user to the database
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                // Redirect to the login page after successful registration
                return RedirectToAction("Login");
            }
            // If model state is not valid, return the model to the view to show error messages
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                // Check if the entered password matches the hashed password in the database
                if (BC.Verify(password, user.PasswordHash))
                {
                    // If it matches, log the user in by storing their username in the session
                    HttpContext.Session.SetString("User", user.Username);
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("Username", user.Username);

                    // Redirect to the home page or wherever you want
                    return RedirectToAction("Index", "Home");
                }
            }
            // If login fails, show an error message
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        public IActionResult Logout()
        {
            // clear the session variable
            HttpContext.Session.Remove("IsLoggedIn");

            // redirect to the home page or login page
            return RedirectToAction("Index", "Home");
        }

    }
}
