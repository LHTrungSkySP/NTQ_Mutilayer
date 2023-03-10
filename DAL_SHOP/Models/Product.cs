namespace DAL_SHOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Medias = new HashSet<Media>();
            Reviews = new HashSet<Review>();
            WishLists = new HashSet<WishList>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Productname { get; set; }

        public int CategoryId { get; set; }

        [StringLength(255)]
        public string Slug { get; set; }

        public int Shopid { get; set; }
        public int? Views { get; set; }
        public int? IsPublished { get; set; }

        [Column(TypeName = "text")]
        public string ProductDetail { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public bool? Trending { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Media> Medias { get; set; }

        public virtual Shop Shop { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> Reviews { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WishList> WishLists { get; set; }
    }
}
