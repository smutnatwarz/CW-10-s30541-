namespace CW_10.DTOs;

public class TripsWithDetailsGetDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public  List<CountryGetDto> countries  { get; set; }  
    public  List<ClientGetDto> clients  { get; set; } 
}