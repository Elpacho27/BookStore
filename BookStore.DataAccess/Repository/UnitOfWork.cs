using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    public ICategoryRepository Category {  get; private set; }
    public IProductRepository Product { get; private set; }

    public ICompanyRepository Company { get; private set; }


    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Category = new CategoryRepository(context);
        Product = new ProductRepository(context);
        Company =new CompanyRepository(context);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
