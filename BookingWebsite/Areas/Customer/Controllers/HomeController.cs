using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Extensions;
using Microsoft.AspNetCore.Mvc;
using BookingWebsite.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Dependency;

namespace BookingWebsite.Controllers
{
    // Indicating that controller resided within Customers area
    [Area("Customer")]
    public class HomeController : Controller
    {

        // first we need constructor to get dependency injection and get the ApplicationDbContext

        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;

        }

        public async Task<IActionResult> Index()
        {
            // in case we want to also display Product Types and Tags on our homepage we Include this
            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.Tags).ToListAsync();
            

            // return View passing productList
            return View(productList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.Tags).Where(m=>m.Id==id).FirstOrDefaultAsync();

            return View(product);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            // check if anything exist in session
            List<int> lstShoppongCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppongCart == null)
            {
                lstShoppongCart = new List<int>();
            }
            // add retrived item (id) to lstShoppingCart
            lstShoppongCart.Add(id);
            //set session
            HttpContext.Session.Set("ssShoppingCart", lstShoppongCart);

            // redirect
            return RedirectToAction("Index", "Home", new {area = "Customer"});

        }

        public IActionResult Remove(int id)
        {
            // retrieve cart
            List<int> lstShoppongCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppongCart.Count>0)
            {
                if (lstShoppongCart.Contains(id))
                {
                    lstShoppongCart.Remove(id);
                }
            }
            //set session
            HttpContext.Session.Set("ssShoppingCart", lstShoppongCart);
            // return to Index
            return RedirectToAction(nameof(Index));

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
