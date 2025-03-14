﻿using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderDetail>> GetOrderDetailsByOrderHeaderId(int orderHeaderId)
        {
            return await _context.OrderDetails.Where(o => o.OrderHeaderId == orderHeaderId).Include(p =>p.Product).ToListAsync();
        }
    }
}
