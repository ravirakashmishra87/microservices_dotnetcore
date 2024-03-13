using Newtonsoft.Json;
using Services.ProductAPI.Models.DTO;
using Services.ShoppingCartAPI.Models.DTO;
using Services.ShoppingCartAPI.Service.IService;
using System.Net.Http;

namespace Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("product");
            var response = await client.GetAsync($"api/product");
            var apiContent =await  response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if(result.IsSuccess ) {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(result.Result));
            }
            return new List<ProductDto>();

        }
    }
}
