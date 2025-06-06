﻿using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject.Services
{
    public class ProductService : IProductService
    {
        private ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.Where(c => c.IsProduct == true).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAllServices()
        {
            return await _context.Products.Where(c => c.IsProduct == false).ToListAsync();
        }
        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }
    }
}
