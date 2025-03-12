using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProject.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddShoppingCart(ShoppingCart shoppingcart)
        {
            _context.ShoppingCarts.Add(shoppingcart);
            await _context.SaveChangesAsync();
        }

        public async Task<ShoppingCart> GetShoppingCart(string identityUserId, int productId)
        {
            return await _context.ShoppingCarts
                .Include(sc => sc.Product)
                .FirstOrDefaultAsync(sc => sc.IdentityUserId == identityUserId && sc.ProductId == productId);
        }
        public async Task<ShoppingCart> GetShoppingCartByCartId(string identityUserId, int cartId)
        {
            return await _context.ShoppingCarts
                .Include(sc => sc.Product)
                .FirstOrDefaultAsync(sc => sc.IdentityUserId == identityUserId && sc.Id == cartId);
        }
        public async Task<List<ShoppingCart>> GetAllShoppingCarts(string identityUserId)
        {
            return await _context.ShoppingCarts
                .Where(sc => sc.IdentityUserId == identityUserId)
                .Include(sc => sc.Product) // Include the related Product entity
                .ToListAsync();
        }
        public async Task UpdateShoppingCart(ShoppingCart shoppingcart)
        {
            _context.ShoppingCarts.Update(shoppingcart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShoppingCartById(ShoppingCart cart)
        {
            _context.ShoppingCarts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}
