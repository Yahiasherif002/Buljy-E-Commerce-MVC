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
    public class OrderHeader
    {
        public  int Id { get; set; }

        [ValidateNever]
        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser  { get; set; }

        public DateTime? OrderDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public double? OrderTotal { get; set; }

        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        // Those for a customer with company role who can pay later
        [ValidateNever]
        public DateTime?  PaymentDate{ get; set; }
        [ValidateNever]

        public DateOnly? PaymentDueDate { get; set; }
        //------------------------------------------------

        
        public string? Name { get; set; }

        public string? Phone { get; set; }

        
        public string? StreetAddress { get; set; }

        public string? City { get; set; }
       
        public string? State { get; set; }

        
        public string? PostalCode { get; set; }



    }
}
