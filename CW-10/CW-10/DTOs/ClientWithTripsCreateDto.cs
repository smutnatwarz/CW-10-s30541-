using System.ComponentModel.DataAnnotations;

namespace CW_10.DTOs;

public class ClientWithTripsCreateDto
{
    [Required]
    [MaxLength(120)]
    public string FirstName { get; set; } 
    
    [Required]
    [MaxLength(120)]
    public string LastName { get; set; }
    
    [Required]
    [MaxLength(120)]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [Phone]
    public string Telephone { get; set; }
    

    [Required]
    [MaxLength(11)]
    [MinLength(11)]
    public string Pesel { get; set; }
    
    [Required]    
    public int IdTrip { get; set; }
    
    public DateTime? PaymentDate { get; set; }

}