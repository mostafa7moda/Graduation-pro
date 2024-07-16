using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class usersaccountsController : Controller
    {
        private readonly WebApplication5Context _context;

        public usersaccountsController(WebApplication5Context context)
        {
            _context = context;
        }

        // GET: usersaccounts
        public async Task<IActionResult> Index()
        {
            var roule = HttpContext.Session.GetString("Role");
            if (roule == "Admin")
            {

                return View(await _context.usersaccounts.Where(m => m.role == "Admin").ToListAsync()) ;
                             
            }
            else
            {
                return Content("Access denied");
            }


        }

        // GET: usersaccounts/Details/5


        // GET: usersaccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult login()
        {
            return View();
        }

        [HttpPost, ActionName("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(string na, string pa)
        {
            var user = _context.usersaccounts.Where(x => x.name == na).FirstOrDefault();
            if (user != null)
            {
                if (user.pass == pa)
                {
                    HttpContext.Session.SetString("Name", na);
                    HttpContext.Session.SetString("Role", user.role);
                    HttpContext.Session.SetInt32("userid", user.Id);
                    if (user.role == "Admin")
                    {
                        return RedirectToAction( "Create", "books");

                    }
                    else
                    {
                        return RedirectToAction("catalogue", "books");

                    }

                }
            }
           
                return RedirectToAction("Login");
            
           

        }
        

        // POST: usersaccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,pass,email")] usersaccounts usersaccounts)
        {
            usersaccounts.role = "customer";


                _context.Add(usersaccounts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(login));
           
        }

        // GET: usersaccounts/Edit/5
        public async Task<IActionResult> Edit()
        {
            int? id = HttpContext.Session.GetInt32("userid");


            var usersaccounts = await _context.usersaccounts.FindAsync(id);

            return View(usersaccounts);
        }

        // POST: usersaccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,pass,role,email")] usersaccounts usersaccounts)
        {

                    _context.Update(usersaccounts);
                    await _context.SaveChangesAsync();
               
                return RedirectToAction(nameof(login));

        }



        private IActionResult usersaccountsExists(int id)
        {
            var rool = HttpContext.Session.GetString("Name");
            if (rool == "Admin")
            {
                return RedirectToAction("Index");
            } else
                return Content("Access Denied");
        }
        [HttpGet]
        public IActionResult AddAdmin()
        {
            var roule=HttpContext.Session.GetString("Role");
            if (roule == "Admin")
            {
                return View();
            }
            else 
            {
                return Content("Access denied");
            }

        }
        [HttpPost]
        public IActionResult AddAdmin(usersaccounts usersaccounts)
        {
            _context.Add(usersaccounts);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult AdminList()
        {
            var Admins = _context.usersaccounts.Where(m => m.role == "Admin").ToList();
            return View(Admins);    
        }




        // GET: user/Delete/5

        [HttpGet]
        public IActionResult Delete(int? id)
        {

            var roule = HttpContext.Session.GetString("Role");
            if (roule == "Admin")
            {
                if (id == null || _context.usersaccounts == null)
                {
                    return NotFound();
                }

                var user =  _context.usersaccounts
                    .FirstOrDefaultAsync(m => m.Id == id);
             
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);


            }
            else
            {
                return Content("Access Denied");
            }








        }

        [HttpPost, ActionName("Delete")]
        
        public IActionResult Delete(usersaccounts user)
        {
            
          
            
          
            if (user != null)
            {
                _context.usersaccounts.Remove(user);
            }

             _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("Role");
            return RedirectToAction("login", "usersaccounts");
        }


       







    }



















}
