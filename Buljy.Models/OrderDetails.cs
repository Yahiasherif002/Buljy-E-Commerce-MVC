using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [ForeignKey("OrderHeaderId")]
        [ValidateNever]
        public int OrderHeaderId { get; set; }
        public OrderHeader OrderHeader { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product product { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
}
