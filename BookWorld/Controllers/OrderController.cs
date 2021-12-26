using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookWorld.Data;
using BookWorld.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BookWorld.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAllOrders()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = _context.Order.Include(o => o.ApplicationUser).Where(o=>o.OrderSituation==true && o.MusteriId==userId);
            return View(await result.ToListAsync());
        }

        public async Task<IActionResult> FinishOrder()
        {
            int totalAmount = 0;
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _context.Order.Include(o => o.ApplicationUser).SingleOrDefaultAsync(o => o.OrderSituation == false && o.MusteriId == userId);

            var basketPorducts = await  _context.Basket.Include(b=>b.Book).Where(b => b.OrderId == result.Id).ToListAsync();
            foreach (var item in basketPorducts)
            {
                totalAmount += item.Book.Price;
            }
            
            result.OrderSituation = true;
            result.OrderDate = DateTime.Now;
            result.TotalAmount = totalAmount;
            if(result != null)
            {
                _context.Update(result);
                await _context.SaveChangesAsync();

            }

 
            var basket = new List<Basket>(); 
                return View("/Views/Basket/GetAllProducts.cshtml",basket);
           
        }



        public async Task<IActionResult> OrderDetail(int id)
        {
            var result = _context.Basket.Include(o => o.Book).Where(o=>o.OrderId==id);
            return View(await result.ToListAsync());
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Order.Include(o => o.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["MusteriId"] = new SelectList(_context.ApplicationUser, "Id", "Id");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MusteriId,OrderDate,TotalAmount,OrderSituation")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MusteriId"] = new SelectList(_context.ApplicationUser, "Id", "Id", order.MusteriId);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["MusteriId"] = new SelectList(_context.ApplicationUser, "Id", "Id", order.MusteriId);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MusteriId,OrderDate,TotalAmount,OrderSituation")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["MusteriId"] = new SelectList(_context.ApplicationUser, "Id", "Id", order.MusteriId);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
