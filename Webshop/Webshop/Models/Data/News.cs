using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class News
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vänligen sätt en titel")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vänligen ange artikeltext")]
        public string Text { get; set; }

        public DateTime NewsDate { get; set; }

        //public IEnumerable<News> NewsCollection { get; set; }
    }
}
