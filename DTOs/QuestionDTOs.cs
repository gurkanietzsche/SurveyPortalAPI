public class CreateQuestionDTO
{
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public List<CreateQuestionOptionDTO> Options { get; set; }
}
// DTOs/UpdateQuestionDTO.cs
public class UpdateQuestionDTO
{
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
}

public class QuestionDTO
{
    public int Id { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public List<QuestionOptionDTO> Options { get; set; }
}