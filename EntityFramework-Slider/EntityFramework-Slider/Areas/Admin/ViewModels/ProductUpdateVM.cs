﻿using EntityFramework_Slider.Models;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class ProductUpdateVM
    {

        public ICollection<ProductImage> Images { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public decimal Price { get; set; }  
        [Required]
        public int Count { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> Photos { get; set; }

    }
}
