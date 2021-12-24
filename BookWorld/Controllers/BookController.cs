using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookWorld.Data;
using BookWorld.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace BookWorld.Controllers
{
   
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Book.Include(b => b.Author).Include(b => b.Publisher).Include(b => b.Subcategory).Include(b => b.Translator);
            return View(await applicationDbContext.ToListAsync());
        }

        
        public async Task<IActionResult> BookDetail(int? id)
        {
         

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Subcategory)
                .Include(b => b.Translator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize]
        public async Task<IActionResult> AddBookToBasket(UserBasketDto userBasketDto)
        {
            
            var orderResult = _context.Order
                .Include(b => b.ApplicationUser)
                .SingleOrDefault(b => b.MusteriId == userBasketDto.UserId && b.OrderSituation == false);

            if (orderResult == null)
            {
                Order order = new Order
                {
                    MusteriId = userBasketDto.UserId,
                };

                _context.Add(order);
               await  _context.SaveChangesAsync();

                Basket basket = new Basket
                {
                    Number = userBasketDto.Number,
                    BookId = userBasketDto.BookId,
                    OrderId = order.Id
                };
                _context.Add(basket);
               await  _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            else
            {
                Basket basket = new Basket
                {
                    Number=userBasketDto.Number,
                    BookId=userBasketDto.BookId,
                    OrderId =orderResult.Id
                };

                _context.Add(basket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
        }

        [HttpPost]
        public  IActionResult Search(string Ara)
        {
            var result =  _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Subcategory)
                .Include(b => b.Translator).Where(b => b.Name.Contains(Ara) == true);
                 return View("~/Views/Home/Index.cshtml",  result);

        }







        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Subcategory)
                .Include(b => b.Translator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name");
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Name");
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategory, "Id", "Name");
            ViewData["TranslatorId"] = new SelectList(_context.Translator, "Id", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PageNumber,Image,Price,AuthorId,SubcategoryId,PublisherId,ReleaseDate,TranslatorId")] Book book)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream =new FileStream(Path.Combine(uploads, fileName + extension),FileMode.Create))
                {

                    files[0].CopyTo(fileStream);

                }
                book.Image = @"\images\" + fileName + extension;


                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name", book.AuthorId);
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Name", book.PublisherId);
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategory, "Id", "Name", book.SubcategoryId);
            ViewData["TranslatorId"] = new SelectList(_context.Translator, "Id", "Name", book.TranslatorId);
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Id", book.AuthorId);
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherId);
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategory, "Id", "Id", book.SubcategoryId);
            ViewData["TranslatorId"] = new SelectList(_context.Translator, "Id", "Id", book.TranslatorId);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PageNumber,Image,Price,AuthorId,SubcategoryId,PublisherId,ReleaseDate,TranslatorId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Id", book.AuthorId);
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherId);
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategory, "Id", "Id", book.SubcategoryId);
            ViewData["TranslatorId"] = new SelectList(_context.Translator, "Id", "Id", book.TranslatorId);
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Subcategory)
                .Include(b => b.Translator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
