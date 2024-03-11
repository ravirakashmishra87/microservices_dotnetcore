using MS_Web.Models.DTO;
using MS_Web.Models;

namespace MS_Web.Service.IService
{
    public interface IProductService
    {        
        Task<ResponseDto?> GetAllProductAsync();
        Task<ResponseDto?> GetProductByIdAsync(int id);
        Task<ResponseDto?> CreateProductAsync(ProductDto product);
        Task<ResponseDto?> DeleteProductAsync(int id);
        Task<ResponseDto?> UpdateProductAsync(ProductDto product);
      

    }
}
