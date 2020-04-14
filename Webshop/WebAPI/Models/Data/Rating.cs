using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Data
{
    public class Rating
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [NotMapped]
        public string UserEmail { get; set; }

        [NotMapped]
        public string UserName { get; set; }

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
