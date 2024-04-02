using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Models
{

    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Title {  get; set; }
        public string Description { get; set; }

        public string ISBN { get; set; }

        public string Author { get; set; }
        [DisplayName("List price")]
        [Range(1D,1000D)]
        public double ListPrice { get; set; }
        [Range(1D, 1000D)]
        public double Price { get; set; }
        [DisplayName("Price for 50+")]

        public double Price50 { get; set; }
        [DisplayName("Price for 100+")]
        public double Price100 { get; set; }

        public int CategoryID {  get; set; }
        [ForeignKey("CategoryID")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public string ImageURL { get; set; }
        
    }
}
