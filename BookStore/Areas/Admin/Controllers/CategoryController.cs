using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //private readonly ICategoryRepository _categoryRepository;


        private IUnitOfWork _unitOfWork;
        //public CategoryController(ICategoryRepository categoryRepository,ApplicationDbContext context)
        //{
        //    _categoryRepository = categoryRepository;
        //   _context = context;
        //}

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index()
        {
            List<Category> categorylist = _unitOfWork.Category.GetAll().ToList();
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
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            }

            return View();
        }

        public IActionResult Edit(int? categoryId)
        {
            if (categoryId == null || categoryId == 0)
            {
                return NotFound();



            }
            Category? category = _unitOfWork.Category.Get(c => c.Id == categoryId);
            //Category? category1 = _context.Categories.Find(categoryId);
            //Category? category2 = _context.Categories.Where(c=>c.Id== categoryId).FirstOrDefault();
            if (category == null)
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
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
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
            Category? category = _unitOfWork.Category.Get(c => c.Id == categoryId);
            //Category? category1 = _context.Categories.Find(categoryId);
            //Category? category2 = _context.Categories.Where(c=>c.Id== categoryId).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? categoryId)
        {
            Category? category = _unitOfWork.Category.Get(c => c.Id == categoryId);

            if (category == null)
            {
                return NotFound();

            }
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index", "Category");

        }
    }
}
