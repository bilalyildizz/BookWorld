using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookWorld.Data;
using BookWorld.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookWorld.Controllers
{
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BasketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Basket
        [Authorize]
        public  async Task<IActionResult>GetAllProducts()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order =  _context.Order.SingleOrDefault(o => o.MusteriId == userId && o.OrderSituation == false);
            
            if(order != null)
            {
               var  basketProducts = _context.Basket.Include(b => b.Book).Where(b => b.OrderId == order.Id);

            return View(await basketProducts.ToListAsync());
            }

            return View(new List<Basket>());
            
          
        }

        public async Task<IActionResult> DeleteBasketItem(int id)
        {

            var basket = await _context.Basket.FindAsync(id);
            _context.Basket.Remove(basket);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetAllProducts");
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Basket.Include(b => b.Book).Include(b => b.Order);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Basket/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .Include(b => b.Book)
                .Include(b => b.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (basket == null)
            {
                return NotFound();
            }

            return View(basket);
        }

        // GET: Basket/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id");
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id");
            return View();
        }

        // POST: Basket/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,BookId,Number")] Basket basket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(basket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", basket.BookId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", basket.OrderId);
            return View(basket);
        }

        // GET: Basket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket.FindAsync(id);
            if (basket == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", basket.BookId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", basket.OrderId);
            return View(basket);
        }

        // POST: Basket/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,BookId,Number")] Basket basket)
        {
            if (id != basket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasketExists(basket.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", basket.BookId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", basket.OrderId);
            return View(basket);
        }

        // GET: Basket/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .Include(b => b.Book)
                .Include(b => b.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (basket == null)
            {
                return NotFound();
            }

            return View(basket);
        }

        // POST: Basket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var basket = await _context.Basket.FindAsync(id);
            _context.Basket.Remove(basket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BasketExists(int id)
        {
            return _context.Basket.Any(e => e.Id == id);
        }
    }
}
