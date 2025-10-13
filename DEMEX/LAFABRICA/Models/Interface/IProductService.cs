using LAFABRICA.Data.DB;

namespace LAFABRICA.Models.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetById(int id);
        Task<Product> Create(Product product);
        Task Update(int id, Product product);
        Task<bool> Delete(int id);
    }
}