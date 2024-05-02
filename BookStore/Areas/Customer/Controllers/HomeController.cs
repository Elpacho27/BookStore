using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;


namespace BookStore.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        return View(productList);
    }


    public IActionResult Details(int productId)
    {
        ShoppingCart cart = new()
        {
            Product = _unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category"),
            Count = 1,
            ProductId = productId
        };

        return View(cart);

    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart cart)
    {
        var claimsIdentity=(ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        cart.ApplicationUserId= userId;

        ShoppingCart cartFromDb=_unitOfWork.ShoppingCart.Get(sp=>sp.ApplicationUserId == userId && sp.ProductId==cart.ProductId);

        if(cartFromDb != null)
        {
            cartFromDb.Count += cart.Count;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        else
        {
            _unitOfWork.ShoppingCart.Add(cart);
        }
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
       
    }
}
