using Microsoft.EntityFrameworkCore;
using SurveyPortalAPI.Models;
public class SurveyResponseRepository
{
    private readonly AppDbContext _context;

    public SurveyResponseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SurveyResponse> CreateResponseAsync(SurveyResponse response)
    {
        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<IEnumerable<SurveyResponse>> GetResponsesBySurveyIdAsync(int surveyId)
    {
        return await _context.SurveyResponses
            .Include(r => r.User)
            .Include(r => r.Survey)
            .Include(r => r.Answers)
                .ThenInclude(a => a.Question)
            .Include(r => r.Answers)
                .ThenInclude(a => a.SelectedOption)
            .Where(r => r.SurveyId == surveyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SurveyResponse>> GetResponsesByUserIdAsync(string userId)
    {
        return await _context.SurveyResponses
            .Include(r => r.Survey)
            .Include(r => r.Answers)
                .ThenInclude(a => a.Question)
            .Include(r => r.Answers)
                .ThenInclude(a => a.SelectedOption)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<SurveyResponse> GetResponseByIdAsync(int id)
    {
        return await _context.SurveyResponses
            .Include(r => r.User)
            .Include(r => r.Survey)
            .Include(r => r.Answers)
                .ThenInclude(a => a.Question)
            .Include(r => r.Answers)
                .ThenInclude(a => a.SelectedOption)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}