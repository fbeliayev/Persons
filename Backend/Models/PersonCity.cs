namespace PersonApi.Models;

public class PersonCity
{
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    
    public bool IsVisited { get; set; }
    public DateTime? VisitedDate { get; set; }
}
