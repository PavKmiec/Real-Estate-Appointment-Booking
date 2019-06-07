using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Models;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingWebsite.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for Product Tags
    /// </summary>
    [Authorize(Roles = SD.SuperAdminEndUser + "," + SD.AdminEndUser)]
    [Area("Admin")]
    public class TagsController : Controller
    {


        private readonly ApplicationDbContext _db;

        public TagsController(ApplicationDbContext db)
        {
            _db = db;

        }

        /// <summary>
        /// Index method action, list of tags 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            // passing Tags List to View from database
            return View(_db.Tags.ToList());
        }




        /// <summary>
        /// GET Create
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        //POST Create action method


        // build-in asp.net functionality
        // With each request of httpPost AntiForgeryToken is added and passes along with the request
        // Once it reaches the server it is checked if its valid (not be altered)
        /// <summary>
        /// POST Create
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Tags tags)
        {
            //  Modelstate.IsValid - validation check on the server side if model is valid.
            //   we can have meany attributes assigned to properties in out models
            // asp.net will check against those attributes 
            if (ModelState.IsValid)
            {
                // if it is valid we will retrieved information to the database
                _db.Add(tags);
                // once addad we save changes to database
                await _db.SaveChangesAsync();
                // return to index with product types ; using "nameof" helps prevent spelling errors
                return RedirectToAction(nameof(Index));
            }

            return View(tags);
        }



        // GET Edit Action Method - retrieving Id which user wants to edit
        /// <summary>
        /// GET Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            // if id is null return nor found
            if (id == null)
            {
                return NotFound();

            }
            // if Id is not null, retrieve tags  from database
            var tags = await _db.Tags.FindAsync(id);
            // check if tag is null, return not found
            if (tags == null)
            {
                return NotFound();

            }
            // if tags is not null return vieW passing tags to the edit view
            return View(tags);
        }



        // build-in asp.net functionality
        // With each request of httpPost AntiForgeryToken is added and passes along with the request
        // Once it reaches the server it is checked if its valid (not be altered)
        /// <summary>
        /// POST Edit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, Tags tags)
        {


            // check if passed id is equal to tags id
            if (id != tags.Id)
            {
                return NotFound();
            }


            //  Modelstate.IsValid - validation check on the server side if model is valid.
            //   we can have meany attributes assigned to properties in out models
            // asp.net will check against those attributes 
            if (ModelState.IsValid)
            {
                // update
                _db.Update(tags);
                //save changes
                await _db.SaveChangesAsync();
                // return to index with product types ; using "nameof" helps prevent spelling errors
                return RedirectToAction(nameof(Index));
            }

            return View(tags);
        }



        /// <summary>
        /// GET Details Action method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            // if id is null return nor found
            if (id == null)
            {
                return NotFound();

            }
            // if Id is not null, retrieve tags from database
            var tags = await _db.Tags.FindAsync(id);
            // check if tag is null, return not found
            if (tags == null)
            {
                return NotFound();

            }

            return View(tags);

        }



        /// <summary>
        /// GET Delete Action Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            // if id is null return nor found
            if (id == null)
            {
                return NotFound();

            }
            // if Id is not null, retrieve tag from database
            var tags = await _db.Tags.FindAsync(id);
            // check if tag is null, return not found
            if (tags == null)
            {
                return NotFound();

            }
            // if Tag is not null return vieW passing tags to the delete view
            return View(tags);
        }

        //POST Delete action method


        // build-in asp.net functionality
        // With each request of httpPost AntiForgeryToken is added and passes along with the request
        // Once it reaches the server it is checked if its valid (not be altered)
        /// <summary>
        /// POST Delate action method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // all we need to delete is an Id
        {

            // getting id from database
            var productTypes = await _db.ProductTypes.FindAsync(id);
            //once we hav an id we just need to remove
            _db.ProductTypes.Remove(productTypes);
            //save changes
            await _db.SaveChangesAsync();
            // return to index with product types ; using "nameof" helps prevent spelling errors
            return RedirectToAction(nameof(Index));
        }


    }
}