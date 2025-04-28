public class CreateAnswerDTO
{
    public int QuestionId { get; set; }
    public string? TextAnswer { get; set; }
    public int? SelectedOptionId { get; set; }
}

public class AnswerDTO
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public string? TextAnswer { get; set; }
    public int? SelectedOptionId { get; set; }
    public string? SelectedOptionText { get; set; }
}
