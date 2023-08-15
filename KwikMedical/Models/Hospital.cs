using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KwikMedical.Models
{
    public class Hospital
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        public virtual ICollection<Ambulance> Ambulances { get; set; }
    }
}
