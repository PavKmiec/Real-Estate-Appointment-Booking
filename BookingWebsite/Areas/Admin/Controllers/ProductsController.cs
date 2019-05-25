﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Models;
using BookingWebsite.Models.ViewModel;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        // we need to access database
        private readonly ApplicationDbContext _db;

        // 
        private readonly IHostingEnvironment _hostingEnvironment;



        // whenever we are post-ing or retrieving this will automatically bind this ProductsViewModel
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }


        // constructor - retrieving db using dependency injection 
        public ProductsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            // initialise ProductsViewModel
            ProductsVM = new ProductsViewModel()
            {
                // assign product types from db
                ProductTypes = _db.ProductTypes.ToList(),
                // tags
                Tags = _db.Tags.ToList(),
                Products = new Products()
            };

        }

        
        public async Task<IActionResult> Index()
        {

            // return list of products
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.Tags);
            return View(await products.ToListAsync());
        }



        // Get : Product Create

        public IActionResult Create()
        {

            // passing ProductsVM (viewModel)
            return View(ProductsVM);
        }


        // POST : Product Create

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()  // note that because we have bind-ed ProductsViewModel we do not have to pass it here
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);

            }


            _db.Products.Add(ProductsVM.Products);
            await _db.SaveChangesAsync();

            // Image being saved
            string webRootPath = _hostingEnvironment.WebRootPath;

            // files will have files uploaded from the View
            var files = HttpContext.Request.Form.Files;




            // retrieve from database 
            var productsFromDb = _db.Products.Find(ProductsVM.Products.Id);


            // check for uploaded files
            if (files.Count != 0) 
            {
                // Image has been uploaded
                // find upload path
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                // find file extenstion
                var extenstion = Path.GetExtension(files[0].FileName);

                // using filesteram we will copy uploaded file to the server and rename it to product Id
                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extenstion), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                    
                }

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extenstion;


            }
            else
            {
                // when user does not upload an image use default image
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath+ @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        // GET : Edit
        //  passing id parameter of the product that user wants to edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            //Populate product ;  SingleOrDefaultAsync - return just one record
            ProductsVM.Products = await _db.Products.Include(m => m.Tags).Include(m => m.ProductTypes)
                .SingleOrDefaultAsync(m => m.Id == id);


            // if product has not been retrieved return NotFound
            if (ProductsVM.Products==null)
            {
                return NotFound();

            }

            // return ViewModel
            return View(ProductsVM);

        }


        // POST : Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id) // Because we have already binded Products View Model we do not have to pass it as a parameter
        {
            if (ModelState.IsValid)
            {
                // we need webRootPath in case we need to change the image
                string webRootPath = _hostingEnvironment.WebRootPath;
                // then we need to get files that are uploaded , if there was any
                var files = HttpContext.Request.Form.Files;

                // if new image is uploaded, first we need to delete the old one there fore we need its name 

                var productsFromDb = _db.Products.Where(m => m.Id == ProductsVM.Products.Id).FirstOrDefault();

                // if user uploaded an image
                if (files.Count>0 && files[0] != null)
                {
                    // this will retrieve the folded where image is uploaded or where image already exists
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);

                    // we need to find extenstion of the images
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productsFromDb.Image);

                    // delete old file 
                    if (System.IO.File.Exists(Path.Combine(uploads, ProductsVM.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.Id + extension_old));
                        
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);

                    }

                    ProductsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension_new;
                }


                //update productsFromDb and save to database
                if (ProductsVM.Products.Image !=null)
                {
                    productsFromDb.Image = ProductsVM.Products.Image;
                }

                // update rest of the properties 
                productsFromDb.Name = ProductsVM.Products.Name;
                productsFromDb.Price = ProductsVM.Products.Price;
                productsFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productsFromDb.TagsId = ProductsVM.Products.TagsId;
                productsFromDb.FurnishDetail = ProductsVM.Products.FurnishDetail;
                productsFromDb.Available = ProductsVM.Products.Available;
                // save
                await _db.SaveChangesAsync();

                // redirect
                return RedirectToAction(nameof(Index));

            }

            return View(ProductsVM);
        }

        // GET : Details
        //  passing id parameter of the product that user wants to View
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            //Populate product ;  SingleOrDefaultAsync - return just one record
            ProductsVM.Products = await _db.Products.Include(m => m.Tags).Include(m => m.ProductTypes)
                .SingleOrDefaultAsync(m => m.Id == id);


            // if product has not been retrieved return NotFound
            if (ProductsVM.Products == null)
            {
                return NotFound();

            }

            // return ViewModel
            return View(ProductsVM);

        }



        // GET : Delete
        //  passing id parameter of the product that user wants to delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            //Populate product ;  SingleOrDefaultAsync - return just one record
            ProductsVM.Products = await _db.Products.Include(m => m.Tags).Include(m => m.ProductTypes)
                .SingleOrDefaultAsync(m => m.Id == id);


            // if product has not been retrieved return NotFound
            if (ProductsVM.Products == null)
            {
                return NotFound();

            }

            // return ViewModel
            return View(ProductsVM);

        }

        // POST : Delete

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Products products = await _db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            var uploads = Path.Combine(webRootPath, SD.ImageFolder);
            var extention = Path.GetExtension(products.Image);

            if (System.IO.File.Exists(Path.Combine(uploads, products.Id + extention)))
            {
                System.IO.File.Delete(Path.Combine(uploads, products.Id + extention));



            }
            // remove entry from database
            _db.Products.Remove(products);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


    }
}