using Microsoft.EntityFrameworkCore;
using SurveyPortalAPI.Models;

namespace SurveyPortalAPI.Repositories
{
    public class SurveyRepository
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Survey>> GetAllSurveysAsync()
        {
            return await _context.Surveys
                .Include(s => s.CreatedBy)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .ToListAsync();
        }

        public async Task<Survey> GetSurveyByIdAsync(int id)
        {
            return await _context.Surveys
                .Include(s => s.CreatedBy)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Survey> CreateSurveyAsync(Survey survey)
        {
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();
            return survey;
        }

        public async Task UpdateSurveyAsync(Survey survey)
        {
            _context.Entry(survey).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSurveyAsync(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey != null)
            {
                _context.Surveys.Remove(survey);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Survey>> GetActiveSurveysAsync()
        {
            var now = DateTime.Now;
            return await _context.Surveys
                .Include(s => s.CreatedBy)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .Where(s => s.IsActive &&
                           (!s.StartDate.HasValue || s.StartDate <= now) &&
                           (!s.EndDate.HasValue || s.EndDate >= now))
                .ToListAsync();
        }

        public async Task<IEnumerable<Survey>> GetSurveysByUserIdAsync(string userId)
        {
            return await _context.Surveys
                .Include(s => s.CreatedBy)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .Where(s => s.CreatedById == userId)
                .ToListAsync();
        }
    }
}