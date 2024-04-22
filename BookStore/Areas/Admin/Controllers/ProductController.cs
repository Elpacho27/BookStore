using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Reflection.Metadata.Ecma335;

namespace BookStore.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = Role.Role_Admin)]
public class ProductController : Controller
{


    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        List<Product> productlist = _unitOfWork.Product.GetAll().ToList();
        return View(productlist);
    }

    public IActionResult Upsert(int? id)
    {
        IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
        {
            Text = c.Name,
            Value = c.Id.ToString()
        });

        ViewBag.CategoryList = categoryList;

        /*ViewData["CategoryList"]=categoryList; drugi način i u asp-items--> "@(ViewData["CategoryList"]) as IEnumerable<SelectListItem>"*/
        /*ViewBag, ViewData, TempData istražiti razlike za DZ*/

        ProductViewModel productViewModel = new ProductViewModel()
        {
            CategoryList = categoryList,
            Product = new Product()

        };

        if (id == null || id == 0)
        {
            //Create
            return View(productViewModel);
        }
        else
        {
            //Update
            productViewModel.Product = _unitOfWork.Product.Get(p => p.Id == id);
            return View(productViewModel);
        }
    }
    [HttpPost]
    public IActionResult Upsert(ProductViewModel productViewModel, IFormFile file)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                if (!string.IsNullOrEmpty(productViewModel.Product.ImageURL))
                {
                    var oldImagePath=Path.Combine(wwwRootPath,productViewModel.Product.ImageURL.Trim('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);


                }
                productViewModel.Product.ImageURL = @"\images\product\" + fileName;
            }

            if(productViewModel.Product.Id== 0)
            {
                _unitOfWork.Product.Add(productViewModel.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productViewModel.Product);
            }    
                _unitOfWork.Save();
               //TempData["success"] = "Product created successfully";
                return RedirectToAction("Index", "Product");
         }

            else
            {
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }

            return View(productViewModel);
        }


        /*public IActionResult Delete(int? productId)
        {
            if (productId == null || productId == 0)
            {
                return NotFound();
            }
            Product? product = _unitOfWork.Product.Get(c => c.Id == productId);
            //Category? category1 = _context.Categories.Find(categoryId);
            //Category? category2 = _context.Categories.Where(c=>c.Id== categoryId).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? productId)
        {
            Product? product = _unitOfWork.Product.Get(c => c.Id == productId);

            if (product == null)
            {
                return NotFound();

            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index", "Product");

        }*/

    #region API Calls
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        return Json(new { data = productList });
    }
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = _unitOfWork.Product.Get(p => p.Id == id);
        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        else
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageURL.Trim('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted successfully" });
        }
    }
    #endregion
}



