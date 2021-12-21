using BookWorld.Data;
using BookWorld.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookWorld.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index ()
        {
            var bookList = _context.Book.Include(b => b.Author).Include(b => b.Publisher).Include(b => b.Subcategory).Include(b => b.Translator);
            return View(await bookList.ToListAsync());
        }

        [HttpPost]
        public IActionResult Index(string Ara)
        {
            var result = _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Subcategory)
                .Include(b => b.Translator).Where(b => b.Name.Contains("clue") == true).ToList();
            return View(result);



        }


        public IActionResult Privacy()
        {
            if (User.IsInRole("admin"))
                return View();
            else
                return View("home");
        }

        [HttpPost]
        public IActionResult CultureManagement(string culture,string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return LocalRedirect(returnUrl);
        }
     
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Deneme(int id)
        {
            return View("home");
        }
    }
}
