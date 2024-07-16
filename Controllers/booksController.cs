using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Data;
using WebApplication5.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace WebApplication5.Controllers
{
    public class booksController : Controller
    {
        private readonly WebApplication5Context _context;

        public booksController(WebApplication5Context context)
        {
            _context = context;
        }

        // GET: books
        public IActionResult Index()
        {
            var roule = HttpContext.Session.GetString("Role");
            if (roule == "Admin")
            {
                var books = _context.book.ToList();
                return View(books);

            }
            else
            {
                return Content("Access Denied");
            }


        }

        // GET: books/catalogue
        public async Task<IActionResult> catalogue()
        {
            return View(await _context.book.ToListAsync());
        }

        // GET: books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.book == null)
            {
                return NotFound();
            }

            var book = await _context.book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: books/Create
        public IActionResult Create()
        {
            var roule = HttpContext.Session.GetString("Role");
            if (roule == "Admin")
            {
                return View();

            }
            else
            {
                return Content("Access Denied");
            }


        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormFile file, [Bind("Id,title,info,bookquantity,price,cataid,author,imgfile")] book book)
        {
            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                {  file.CopyTo(filestream); }

                book.imgfile = filename;
            }

            _context.book.Add(book);
             _context.SaveChanges();
            return RedirectToAction(nameof(Index));
           // return View("catalogue", "books");
        }

        // GET: books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var roule = HttpContext.Session.GetString("Role");
            if (roule == "Admin")
            {
                if (id == null || _context.book == null)
                {
                    return NotFound();
                }

                var book = await _context.book.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
                

            }
            else
            {
                return Content("Access Denied");
            }

           
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile file, int id, [Bind("Id,title,info,bookquantity,price,cataid,author,imgfile")] book book)
        {
            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                book.imgfile = filename;
            }
            _context.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            var roule = HttpContext.Session.GetString("Role");
            if (roule == "Admin")
            {
                if (id == null || _context.book == null)
                {
                    return NotFound();
                }

                var book = await _context.book
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (book == null)
                {
                    return NotFound();
                }

                return View(book);
                

            }
            else
            {
                return Content("Access Denied");
            }


           
        }

        // POST: books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.book == null)
            {
                return Problem("Entity set 'WebApplication5Context.book'  is null.");
            }
            var book = await _context.book.FindAsync(id);
            if (book != null)
            {
                _context.book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: books/Search
        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction(nameof(catalogue));
            }

            var books = await _context.book.Where(b => b.title.Contains(search) || b.author.Contains(search)).ToListAsync();
            return View("catalogue", books);
        }

        private bool bookExists(int id)
        {
            return _context.book.Any(e => e.Id == id);
        }
    }
}