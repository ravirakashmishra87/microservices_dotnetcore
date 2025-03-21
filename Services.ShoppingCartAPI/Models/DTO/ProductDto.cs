﻿using System.ComponentModel.DataAnnotations;

namespace Services.ShoppingCartAPI.Models.DTO
{
	public class ProductDto
	{
		[Key]
		public int ProductId { get; set; }
		[Required]
		public string? Name { get; set; }
		[Range(0, 1000)]
		public double Price { get; set; }
		public string? Description { get; set; }
		public string? CategoryName { get; set; }
		public string? ImageUrl { get; set; }
	}
}
