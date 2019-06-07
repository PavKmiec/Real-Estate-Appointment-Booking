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
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Dependency;

namespace BookingWebsite.Controllers
{
    // Indicating that controller resided within Customers area
    /// <summary>
    /// Customer Home controller
    /// </summary>
    [Area("Customer")]
    public class HomeController : Controller
    {

        // first we need constructor to get dependency injection and get the ApplicationDbContext

        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;

        }
        /// <summary>
        /// Products to list - Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            // in case we want to also display Product Types and Tags on our homepage we Include this
            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.Tags).ToListAsync();
            

            // return View passing productList
            return View(productList);
        }

        /// <summary>
        /// Details Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.Tags).Where(m=>m.Id==id).FirstOrDefaultAsync();

            return View(product);
        }

        /// <summary>
        /// Details POST - adding to cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        [Authorize]
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

        /// <summary>
        /// Removing product from cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
