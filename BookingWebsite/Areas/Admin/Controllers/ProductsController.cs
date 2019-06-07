using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Extensions;
using BookingWebsite.Models;
using BookingWebsite.Models.ViewModel;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BookingWebsite.Controllers
{
    /// <summary>
    /// Products controller
    /// </summary>
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser + "," + SD.SellerEndUser + "," + SD.Employee)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        // we need to access database
        private readonly ApplicationDbContext _db;

        // we need hosting environment for uploading images
        private readonly IHostingEnvironment _hostingEnvironment;



        // whenever we are post-ing or retrieving this will automatically bind this ProductsViewModel
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }


        /// <summary>
        /// Constructor - retrieving db using dependency injection
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
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

        /// <summary>
        /// GET Index action - products to list
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()  
        {


            // if user is in role "Seller" redirect to create method
            if (User.IsInRole(SD.SellerEndUser))
            {
                return RedirectToAction("Create");

            }
            // return list of products
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.Tags);
            return View(await products.ToListAsync());
        }



        // Get : Product Create
        /// <summary>
        /// Get create action
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {

            // passing ProductsVM (viewModel)
            return View(ProductsVM);
        }


        // POST : Product Create
        /// <summary>
        /// POST create action for product
        /// </summary>
        /// <returns></returns>
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()  // note that because we have bind-ed ProductsViewModel we do not have to pass it here
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);

            }

            // add
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

            // because we don't have an online payment implemented yet, this is a temporary solution, to display a message to a seller 
            if (User.IsInRole(SD.SellerEndUser))
            {
                TempData.Add("Added", " You have successfully Added a Property to our system, one of our advisers will contact you to arrange a payment. Thank you");
            }
        
            return RedirectToAction(nameof(Index));

        }





        // GET : Edit
        /// <summary>
        /// GET edit action - passing id parameter of the product that user wants to edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser + "," + SD.Employee)]
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
        /// <summary>
        /// Product Edit POST action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser + "," + SD.Employee)]
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

                // update  properties 
                productsFromDb.Name = ProductsVM.Products.Name;
                productsFromDb.Price = ProductsVM.Products.Price;
                productsFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productsFromDb.TagsId = ProductsVM.Products.TagsId;
                productsFromDb.FurnishDetail = ProductsVM.Products.FurnishDetail;
                productsFromDb.Available = ProductsVM.Products.Available;
                productsFromDb.Description = ProductsVM.Products.Description;
                // save
                await _db.SaveChangesAsync();

                // redirect
                return RedirectToAction(nameof(Index));

            }

            return View(ProductsVM);
        }

        /// <summary>
        /// GET Details action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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



        /// <summary>
        /// GET action - Delete product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser + "," + SD.Employee)]
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

        /// <summary>
        /// Delete POST action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser + "," + SD.Employee)]
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

        /// <summary>
        /// A method for downloading excel list of products
        /// </summary>
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser + "," + SD.Employee)]
        public void ProdListDownload()
        {
            // get
            var productDW = from a in _db.Products.Include(a=>a.ProductTypes)
                            orderby a.Name descending
                            select new
                            {
                                Name = a.Name,
                                Price = a.Price,
                                Type = a.ProductTypes.Name,
                                Avaiable = a.Available,
                                FurnishLevel = a.FurnishDetail,
                                Extras = a.Tags.Name,

                            };


            // how many rows is there?
            int numRows = productDW.Count();

            // lets check if there is any data
            if (numRows > 0) // if there is data
            {
                // create new instance of excel package - from scratch - it may be ideal to have a static template in DB for reuse but like I said lets keep it simple for now
                ExcelPackage excel = new ExcelPackage();

                // add excel worksheet
                var workSheet = excel.Workbook.Worksheets.Add("Products");

                // lets throw some data at our worksheet
                // collection; bool for Load Headers;
                workSheet.Cells[3, 1].LoadFromCollection(productDW, true);
                //workSheet.Column(1).Style.Numberformat.Format = "yyyy-mm-dd HH:MM";

                //We can define block od cells Cells[startRow, startColumn, endRow, endColumn]
                workSheet.Cells[4, 1, numRows + 3, 2].Style.Font.Bold = true;

                // style heading a little
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.AliceBlue);
                }

                // fit columns size
                workSheet.Cells.AutoFitColumns();


                // lets add title to the top
                workSheet.Cells[1, 1].Value = "Products List Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                {
                    Rng.Merge = true; // Merge columns start and end range
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // time of report - time issue - time zones? server time?

                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 6])
                {

                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                }

                // ok, its time to download our excel file


                //one thing to bare in mind is file size and memory, on a local pc it's fine we have plenty of memory,
                //but on a server it may be an issue to load the whole thing - possible out of memory exceptions
                // what we can do to make sure we are thinking about memory is to stream the data
                // so, lets set up MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers["content-disposition"] = "attachment; filename=ProductList.xlsx"; // could add date time to file
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                }

            }

        }


    }
}