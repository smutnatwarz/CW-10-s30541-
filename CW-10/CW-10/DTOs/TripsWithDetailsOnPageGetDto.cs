namespace CW_10.DTOs;

public class TripsWithDetailsOnPageGetDto
{
    public int PageNum  { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripsWithDetailsGetDto > Trips { get; set; }
    
}