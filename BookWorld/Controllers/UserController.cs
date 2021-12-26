using BookWorld.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BookWorld.Data;

namespace BookWorld.Controllers
{
    
        public class UserController : Controller
        {
            private readonly ApplicationDbContext _context;

            private readonly IHttpContextAccessor _httpContextAccessor;

            public UserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;
            }
            public IActionResult Index()
            {
                return View();
            }
            [Authorize]
            public IActionResult UserDetail()
            {

           //var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
           var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;


                var user = _context.ApplicationUser.SingleOrDefault(u => u.Id == userId);
                return View(user);

            }
        }

    }
