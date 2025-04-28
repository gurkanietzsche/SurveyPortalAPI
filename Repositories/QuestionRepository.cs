using Microsoft.EntityFrameworkCore;
using SurveyPortalAPI.Models;

public class QuestionRepository
{
    private readonly AppDbContext _context;

    public QuestionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Question> CreateQuestionAsync(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task UpdateQuestionAsync(Question question)
    {
        _context.Entry(question).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteQuestionAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question != null)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Question>> GetQuestionsBySurveyIdAsync(int surveyId)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .Where(q => q.SurveyId == surveyId)
            .OrderBy(q => q.Order)
            .ToListAsync();
    }
}