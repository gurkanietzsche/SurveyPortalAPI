public class Answer
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    public int SurveyResponseId { get; set; }
    public SurveyResponse SurveyResponse { get; set; }
    public string? TextAnswer { get; set; }
    public int? SelectedOptionId { get; set; }
    public QuestionOption? SelectedOption { get; set; }
}