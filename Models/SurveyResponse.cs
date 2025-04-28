using SurveyPortalAPI.Models;

public class SurveyResponse
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public Survey Survey { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public DateTime SubmittedDate { get; set; }
    public ICollection<Answer> Answers { get; set; }
}