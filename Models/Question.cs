public class Question
{
    public int Id { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public int SurveyId { get; set; }
    public Survey Survey { get; set; }
    public ICollection<QuestionOption> Options { get; set; }
    public ICollection<Answer> Answers { get; set; }
}