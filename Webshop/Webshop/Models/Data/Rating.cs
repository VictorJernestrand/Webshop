using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserEmail {get; set;}

        public string UserName { get; set; }

        public bool CanComment { get; set; } = true;

        public int ProductId { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public DateTime RateDate { get; set; }



        // SQL-table relations

        public User User { get; set; }

        public Product Product { get; set; }

    }
}
