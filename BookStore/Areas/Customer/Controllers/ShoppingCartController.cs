using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
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
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartViewModel = new ShoppingCartViewModel()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sp => sp.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new OrderHeader()
        };
        var applicationUser = new ApplicationUser();
        applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        ShoppingCartViewModel.OrderHeader.Name = applicationUser.Name;
        ShoppingCartViewModel.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
        ShoppingCartViewModel.OrderHeader.StreetAddress = applicationUser.StreetAddress;
        ShoppingCartViewModel.OrderHeader.City = applicationUser.City;
        ShoppingCartViewModel.OrderHeader.State = applicationUser.State;
        ShoppingCartViewModel.OrderHeader.PostalCode = applicationUser.PostalCode;
        foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartViewModel);
    }

	[HttpPost]
	[ActionName("Summary")]
	public IActionResult SummaryPOST()
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
		ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sp => sp.ApplicationUserId == userId, includeProperties: "Product");
		ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
		ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;
		ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(au => au.Id == userId);
		foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
		{
			cart.Price = GetPriceBasedOnQuantity(cart);
			ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
		}

		if (applicationUser.CompanyId.GetValueOrDefault() == 0)
		{
			// Customer account
			ShoppingCartViewModel.OrderHeader.PaymentStatus = PaymentStatus.Pending;
			ShoppingCartViewModel.OrderHeader.OrderStatus = OrderStatus.Pending;
		}
		else
		{
			// Company account
			ShoppingCartViewModel.OrderHeader.PaymentStatus = PaymentStatus.Delayed;
			ShoppingCartViewModel.OrderHeader.OrderStatus = OrderStatus.Approved;
		}
		_unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
		_unitOfWork.Save();

		foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
		{
			OrderDetail orderDetail = new()
			{
				ProductId = cart.ProductId,
				OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
				Price = cart.Price,
				Count = cart.Count,
			};

			_unitOfWork.OrderDetail.Add(orderDetail);
			_unitOfWork.Save();
		}

		return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
	}

	public IActionResult OrderConfirmation(int id)
	{
		return View(id);
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
        var cartFromDb=_unitOfWork.ShoppingCart.Get(c=>c.Id == cartId);
        cartFromDb.Count=cartFromDb.Count+1;

        _unitOfWork.ShoppingCart.Update(cartFromDb);

        _unitOfWork.Save();

        return RedirectToAction("Index");
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb=_unitOfWork.ShoppingCart.Get(c=>c.Id==cartId);
        if (cartFromDb.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Delete(cartFromDb);
        }
        else
        {
            cartFromDb.Count = cartFromDb.Count-1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }

        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cartFromDb=_unitOfWork.ShoppingCart.Get(c=>c.Id==cartId);

        _unitOfWork.ShoppingCart.Delete(cartFromDb);

        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }
}
