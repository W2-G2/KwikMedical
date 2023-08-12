using System.ComponentModel.DataAnnotations.Schema;

namespace KwikMedical.Models;

public class Hospital
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This attribute specifies that the Id field is auto-generated.
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public List<Ambulance> Ambulances { get; set; }

}
