namespace DAL_SHOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Medias")]
    public partial class Media
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string MediasName { get; set; }

        [Required]
        [StringLength(255)]
        public string PathLink { get; set; }

        public int MediaType { get; set; }

        public int ProductId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual Product Product { get; set; }
    }
}
