using DAL_SHOP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BO_SHOP.Models
{
    public class ProductRow
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Shopname { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public int Views { get; set; }
        public int? IsPublished { get; set; }
        public string ProductDetail { get; set; }
        public decimal Price { get; set; }
        public bool? Trending { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreatedAt { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? UpdateAt { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DeletedAt { get; set; }

        public List<Media> medias = new List<Media>();
    }
}