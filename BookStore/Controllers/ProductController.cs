﻿using BookStore.DataAccess.Data;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Product> productlist = _context.Products.ToList();
            return View(productlist);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (product.Name.Length > 15)
            {
                ModelState.AddModelError("Name", "The name must not be longer than 15 characters");
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index", "Product");
            }

            return View();
        }

        public IActionResult Edit(int? productId)
        {
            if (productId == null || productId == 0)
            {
                return NotFound();



            }
            Product? product = _context.Products.FirstOrDefault(c => c.Id == productId);
            //Category? category1 = _context.Categories.Find(categoryId);
            //Category? category2 = _context.Categories.Where(c=>c.Id== categoryId).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                TempData["success"] = "Product edited successfully";
                return RedirectToAction("Index", "Product");

            }

            return View();
        }
        public IActionResult Delete(int? productId)
        {
            if (productId == null || productId == 0)
            {
                return NotFound();
            }
            Product? product = _context.Products.FirstOrDefault(c => c.Id == productId);
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
            Product? product= _context.Products.FirstOrDefault(c => c.Id == productId);

            if (product == null)
            {
                return NotFound();

            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index", "Product");

        }
    }
}