using SurveyPortalAPI.Models;

public class Survey
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; }
    public ICollection<Question> Questions { get; set; }
    public ICollection<SurveyResponse> Responses { get; set; }
}