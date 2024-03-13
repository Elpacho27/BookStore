using BookStore.DataAccess.Data;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> categorylist = _context.Categories.ToList();
            return View(categorylist);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name.Length > 15)
            {
                ModelState.AddModelError("Name", "The name must not be longer than 15 characters");
            }

            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            }

            return View();
        }

        public IActionResult Edit(int? categoryId)
        {
            if(categoryId == null || categoryId==0)
            {
                return NotFound();
            

            
            }
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            //Category? category1 = _context.Categories.Find(categoryId);
            //Category? category2 = _context.Categories.Where(c=>c.Id== categoryId).FirstOrDefault();
            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index", "Category");

            }
            
            return View();
        }
        public IActionResult Delete(int? categoryId)
        {
            if (categoryId == null || categoryId == 0)
            {
                return NotFound();
            }
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            //Category? category1 = _context.Categories.Find(categoryId);
            //Category? category2 = _context.Categories.Where(c=>c.Id== categoryId).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? categoryId)
        {
            Category? category= _context.Categories.FirstOrDefault(c=>c.Id == categoryId);

            if (category == null)
            {
                return NotFound();

            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index", "Category");

        }
    }
}
