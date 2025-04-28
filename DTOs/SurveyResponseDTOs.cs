public class CreateSurveyResponseDTO
{
    public int SurveyId { get; set; }
    public List<CreateAnswerDTO> Answers { get; set; }
}

public class SurveyResponseDTO
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string SurveyTitle { get; set; }
    public string UserId { get; set; }
    public string UserFullName { get; set; }
    public DateTime SubmittedDate { get; set; }
    public List<AnswerDTO> Answers { get; set; }
}