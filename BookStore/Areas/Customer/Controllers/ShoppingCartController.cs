using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class ShoppingCartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

    [ActivatorUtilitiesConstructor]
    public ShoppingCartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity=(ClaimsIdentity)User.Identity;
        var userId=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartViewModel = new ShoppingCartViewModel()
        {
            ShoppingCartList =_unitOfWork.ShoppingCart.GetAll(sp => sp.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new OrderHeader()
        };
        foreach(var cart in ShoppingCartViewModel.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }
        return View(ShoppingCartViewModel);
    }
    public IActionResult Summary()
    {
        return View();
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else
        {
            if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
    public IActionResult Plus(int cartId)
    {
        var cartFromDb=_unitOfWork.ShoppingCart.Get(sc=>sc.Id == cartId);
        cartFromDb.Count +=1;

        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb=_unitOfWork.ShoppingCart.Get(sc=>sc.Id==cartId);
        if (cartFromDb.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Delete(cartFromDb);
        }
        else
        {
            cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }

        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cartFromDb=_unitOfWork.ShoppingCart.Get(sc=>sc.Id==cartId);

        _unitOfWork.ShoppingCart.Delete(cartFromDb);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }
}
