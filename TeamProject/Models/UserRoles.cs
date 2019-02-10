namespace TeamProject.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserRoles
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(5)]
        public string Role { get; set; }

        public virtual User User { get; set; }
    }
}
