using CapstoneProject.Models;

namespace CapstoneProject.Services.Contracts
{
    public interface IShoppingCartService
    {
        Task AddShoppingCart(ShoppingCart shoppingcart);
        Task DeleteShoppingCartById(ShoppingCart cart);
        Task UpdateShoppingCart(ShoppingCart shoppingcart);
        Task<ShoppingCart> GetShoppingCart(string identityUserId, int productId);
        Task<List<ShoppingCart>> GetAllShoppingCarts(string identityUserId);
        Task<ShoppingCart> GetShoppingCartByCartId(string identityUserId, int cartId);
    }
}
