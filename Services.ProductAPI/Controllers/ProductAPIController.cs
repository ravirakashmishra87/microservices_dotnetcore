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
		public object post([FromBody] ProductDto ProductDto)
		{
			try
			{
				Product objProduct = _mapper.Map<Product>(ProductDto);
				_dbContext.Add(objProduct);
				_dbContext.SaveChanges();

				_responseDto.Result = _mapper.Map<ProductDto>(objProduct);
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
		public object put([FromBody] ProductDto ProductDto)
		{
			try
			{
				Product objProduct = _mapper.Map<Product>(ProductDto);
				_dbContext.Update(objProduct);
				_dbContext.SaveChanges();

				_responseDto.Result = _mapper.Map<ProductDto>(objProduct);
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
