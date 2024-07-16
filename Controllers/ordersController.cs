using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
	public class ordersController : Controller
	{
		private readonly WebApplication5Context _context;

		public ordersController(WebApplication5Context context)
		{
			_context = context;
		}

		// GET: orders
		public async Task<IActionResult> Index()
		{
			return _context.orders != null ?
						View(await _context.orders.ToListAsync()) :
						Problem("Entity set 'WebApplication5Context.orders'  is null.");
		}

		// GET: orders/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.orders == null)
			{
				return NotFound();
			}

			var orders = await _context.orders
				.FirstOrDefaultAsync(m => m.Id == id);
			var username=_context.usersaccounts.FirstOrDefault(m=>m.Id==orders.userid).name;
			var bookName = _context.book.FirstOrDefault(m => m.Id == orders.bookId).title;
			orders.CustomerName= username;
			orders.BookTitel=bookName;
			if (orders == null)
			{
				return NotFound();
			}

			return View(orders);
		}

		// GET: orders/Create
		public async Task<IActionResult> Create(int id)
		{
			HttpContext.Session.SetInt32("bookId", id);

			var book = await _context.book.FindAsync(id);

			return View(book);
		}


		// POST: orders/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(orders order)
		{

			var id = HttpContext.Session.GetInt32("bookId");

			//order.userid = HttpContext.Session.GetInt32("userid") ;
			order.orderdate = DateTime.Today;
		    var book=_context.book.FirstOrDefault(m => m.Id == id);
            int qun = book.bookquantity-order.quantity;
			book.bookquantity = qun;
			_context.Update(book);
			_context.SaveChanges();

            _context.Add(order);
		    _context.SaveChangesAsync();
			return RedirectToAction(nameof(myorders));


		}


		public IActionResult myorders()
		{

			int? id = HttpContext.Session.GetInt32("userid");
            var order = _context.orders
                .Where(m => m.Id == id);
            List<int> userid =new List<int>();
			List<int> bookid =new List<int>();
			

            List<orders>  orders = new List<orders>();
           
			
           
            return View(orders);

        }
		

		public async Task<IActionResult> customerOrders(int? id)
		{


			var orItems = await _context.orders.FromSqlRaw("select *  from orders where  userid = '" + id + "'  ").ToListAsync();
			return View(orItems);

		}

		public async Task<IActionResult> customerreport()
		{
			var orItems = await _context.report.FromSqlRaw("select usersaccounts.id as Id, name as customername, sum (quantity * Price)  as total from book, orders,usersaccounts  where usersaccounts.id = orders.userid  and bookid= book.Id group by name,usersaccounts.id ").ToListAsync();
			return View(orItems);

		}


		// GET: orders/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.orders == null)
			{
				return NotFound();
			}

			var orders = await _context.orders.FindAsync(id);
			if (orders == null)
			{
				return NotFound();
			}
			return View(orders);
		}

		// POST: orders/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,bookId,userid,quantity,orderdate")] orders orders)
		{
			if (id != orders.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(orders);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ordersExists(orders.Id))
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
			return View(orders);
		}

		// GET: orders/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.orders == null)
			{
				return NotFound();
			}

			var orders = await _context.orders
				.FirstOrDefaultAsync(m => m.Id == id);
			if (orders == null)
			{
				return NotFound();
			}

			return View(orders);
		}

		// POST: orders/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.orders == null)
			{
				return Problem("Entity set 'WebApplication5Context.orders'  is null.");
			}
			var orders = await _context.orders.FindAsync(id);
			if (orders != null)
			{
				_context.orders.Remove(orders);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ordersExists(int id)
		{
			return (_context.orders?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		[HttpPost]
        public IActionResult AddToCart(int bookId, int quantity)
        {
            // Get the cart from the session.
            Cart cart = HttpContext.Session.GetObject<Cart>("cart");

            // If the cart doesn't exist, create a new one.
            if (cart == null)
            {
                cart = new Cart();
                HttpContext.Session.SetObject("cart", cart);
            }

            // Find the book in the database.
            book book = _context.book.FirstOrDefault(b => b.Id == bookId);

            if (book != null)
            {
                // Add the item to the cart.
                CartItem cartItem = new CartItem
                {
                    Book = book,
                    Quantity = quantity
                };

                cart.Items.Add(cartItem);

                // Update the total amount of the cart.
                cart.Total += cartItem.Book.price * cartItem.Quantity;

                // Update the cart in the session.
                HttpContext.Session.SetObject("cart", cart);
            }

            // Redirect to the cart view.
            return RedirectToAction("Cart");
        }


        public IActionResult Cart()
		{
			// Get the cart from the session.
			Cart cart = HttpContext.Session.GetObject<Cart>("cart");

			// If the cart doesn't exist, create a new one.
			if (cart == null)
			{
				cart = new Cart();
				HttpContext.Session.SetObject("cart", cart);
			}

			// Return the cart view.
			return View(cart);
		}
	}
}
