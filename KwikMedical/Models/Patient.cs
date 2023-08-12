using System.ComponentModel.DataAnnotations.Schema;

namespace KwikMedical.Models;

public class Patient
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This attribute specifies that the Id field is auto-generated.
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NHSNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Postcode { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }  // Navigation property
}
