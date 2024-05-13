using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ProductAPI.Data;
using Services.ProductAPI.Models;
using Services.ProductAPI.Models.DTO;

namespace Services.ProductAPI.Controllers
{
	[Route("api/product")]
	[ApiController]
	//[Authorize]
	public class ProductAPIController : ControllerBase
	{
		private readonly ApplicationDBContext _dbContext;
		private readonly ResponseDto _responseDto;
		private readonly IMapper _mapper;

		public ProductAPIController(ApplicationDBContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_responseDto = new ResponseDto();
		}

		[HttpGet]
		public ResponseDto Get()
		{
			try
			{
				IEnumerable<Product> productList = _dbContext.Products.ToList();
				_responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(productList);
				_responseDto.IsSuccess = true;
				_responseDto.Message = "Product retrived successfuly";

			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = $"Fail to retrive product : {ex.Message}";

			}
			return _responseDto;

		}
		[HttpGet]
		[Route("{id:int}")]
		public object Get(int id)
		{
			try
			{
				Product objProduct = _dbContext.Products.First(u => u.ProductId == id);
				_responseDto.Result = _mapper.Map<ProductDto>(objProduct);
				_responseDto.IsSuccess = true;
				_responseDto.Message = $"Product retrived successfuly for coupnid: {id}";

			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = $"Fail to retrive Product : {ex.Message}";

			}
			return _responseDto;

		}		

		[HttpPost]
		[Authorize(Roles = "ADMIN")]
		public object post(ProductDto ProductDto)
		{
			try
			{
				Product product = _mapper.Map<Product>(ProductDto);
				_dbContext.Add(product);
				_dbContext.SaveChanges();

                if (ProductDto.Image != null)
                {

                    string fileName = product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    //I have added the if condition to remove the any image with same name if that exist in the folder by any change
                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    FileInfo file = new FileInfo(directoryLocation);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        ProductDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }
                _dbContext.Products.Update(product);
				_dbContext.SaveChanges();
				_responseDto.Result = _mapper.Map<ProductDto>(product);
				_responseDto.IsSuccess = true;
				_responseDto.Message = $"Product created successfuly.";

			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = $"Fail to create Product : {ex.Message}";

			}
			return _responseDto;

		}

		[HttpPut]
		[Authorize(Roles = "ADMIN")]
		public object put(ProductDto ProductDto)
		{
			try
			{
				Product product = _mapper.Map<Product>(ProductDto);
                if (ProductDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        var filedirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                        FileInfo imagefile = new FileInfo(filedirectory);
                        if (imagefile.Exists)
                        {
                            imagefile.Delete();
                        }

                    }
                    string fileName = product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    //I have added the if condition to remove the any image with same name if that exist in the folder by any change
                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    FileInfo file = new FileInfo(directoryLocation);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        ProductDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                _dbContext.Update(product);
				_dbContext.SaveChanges();

				_responseDto.Result = _mapper.Map<ProductDto>(product);
				_responseDto.IsSuccess = true;
				_responseDto.Message = $"Product updated successfuly.";

			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = $"Fail to update Product : {ex.Message}";

			}
			return _responseDto;

		}

		[HttpDelete]
		[Route("{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public object delete(int id)
		{
			try
			{
				Product objProduct = _dbContext.Products.First(u => u.ProductId == id);
				if(!string.IsNullOrEmpty(objProduct.ImageLocalPath))
				{
					var filedirectory = Path.Combine(Directory.GetCurrentDirectory(),objProduct.ImageLocalPath);
					FileInfo file = new FileInfo(filedirectory);
					if(file.Exists)
					{
						file.Delete();
					}

				}
				_dbContext.Remove(objProduct);
				_dbContext.SaveChanges();


				_responseDto.IsSuccess = true;
				_responseDto.Message = $"Product deleted successfuly.";

			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = $"Fail to delete Product : {ex.Message}";

			}
			return _responseDto;

		}
	}
}
