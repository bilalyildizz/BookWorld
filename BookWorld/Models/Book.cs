using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookWorld.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PageNumber { get; set; }
        public string Image  { get; set; }
        [Range(1, int.MaxValue)]
        public int Price { get; set; }
        public int AuthorId { get; set; }
        public int SubcategoryId { get; set; }
        public int? PublisherId { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public int TranslatorId { get; set; }
        public Translator Translator { get; set; }
        public Publisher Publisher { get; set; }
        public Subcategory Subcategory { get; set; }
        public Author Author { get; set; }


    }
}
