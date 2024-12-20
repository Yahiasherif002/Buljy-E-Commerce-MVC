﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string ISBN { get; set; }
        public string Author { get; set; }

        [Required]
        [Display(Name = "List Price")]
        [Range(1, 10000, ErrorMessage = "{0} should be greater than $1 and less than $10000")]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 10000, ErrorMessage = "{0} should be greater than $1 and less than $10000")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 10000, ErrorMessage = "{0} should be greater than $1 and less than $10000")]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 10000, ErrorMessage = "{0} should be greater than $1 and less than $10000")]
        public double Price100 { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        //[AllowNull]
        public Category Category { get; set; }

        [ValidateNever]
        public string ImageUrl  { get; set; }




    }
}
