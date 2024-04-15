using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers;

[Area("Admin")]
public class CompanyController : Controller
{

    private readonly IUnitOfWork _unitOfWork;
   

    public CompanyController(IUnitOfWork unitofwork)
    {
        _unitOfWork = unitofwork;
        
        
    }
    public IActionResult Index()
    {

        List<Company> companylist = _unitOfWork.Company.GetAll().ToList();
        return View(companylist);
    }

    #region API Calls
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
        return Json(new { data = companyList });
    }
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var company = _unitOfWork.Company.Get(p => p.Id == id);
        if (company == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        else
        {
            _unitOfWork.Company.Delete(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted successfully" });
        }
    }
    #endregion

    public IActionResult Upsert(int? id)
    {
        /*ViewData["CategoryList"]=categoryList; drugi način i u asp-items--> "@(ViewData["CategoryList"]) as IEnumerable<SelectListItem>"*/
        /*ViewBag, ViewData, TempData istražiti razlike za DZ*/

        Company company = new Company();

        if (id == null || id == 0)
        {
            //Create
            return View(company);
        }
        else
        {
            //Update
            company = _unitOfWork.Company.Get(p => p.Id == id);
            return View(company);

        }
    }
    [HttpPost]
    public IActionResult Upsert(Company company)
    {
        if (ModelState.IsValid)
        {
            if (company.Id == 0)
            {
                _unitOfWork.Company.Add(company);
            }
            else
            {
                _unitOfWork.Company.Update(company);
            }
            _unitOfWork.Save();
            //TempData["success"] = "Product created successfully";
            return RedirectToAction("Index", "Company");
        }

        

        return View();
    }


}
