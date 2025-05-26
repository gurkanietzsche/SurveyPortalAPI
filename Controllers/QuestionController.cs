using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyPortalAPI.Models;
using AutoMapper;
using SurveyPortalAPI.Repositories;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController : ControllerBase
{
    private readonly QuestionRepository _questionRepository;
    private readonly SurveyRepository _surveyRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public QuestionsController(
        QuestionRepository questionRepository,
        SurveyRepository surveyRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _questionRepository = questionRepository;
        _surveyRepository = surveyRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet("survey/{surveyId}")]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetQuestionsBySurveyId(int surveyId)
    {
        var survey = await _surveyRepository.GetSurveyByIdAsync(surveyId);
        if (survey == null)
        {
            return NotFound("Survey not found");
        }

        var questions = await _questionRepository.GetQuestionsBySurveyIdAsync(surveyId);
        var questionDTOs = questions.Select(q => new QuestionDTO
        {
            Id = q.Id,
            Text = q.Text,
            Type = q.Type,
            IsRequired = q.IsRequired,
            Order = q.Order,
            Options = q.Options?.Select(o => new QuestionOptionDTO
            {
                Id = o.Id,
                Text = o.Text,
                Order = o.Order
            }).ToList() ?? new List<QuestionOptionDTO>()
        }).ToList();

        return Ok(questionDTOs);
    }

    [HttpPost("survey/{surveyId}")]
    [Authorize(Roles = ApplicationRoles.Admin + "," + ApplicationRoles.Surveyor)]
    public async Task<ActionResult<QuestionDTO>> CreateQuestion(int surveyId, [FromBody] CreateQuestionDTO model)
    {
        var survey = await _surveyRepository.GetSurveyByIdAsync(surveyId);
        if (survey == null)
        {
            return NotFound("Survey not found");
        }

        var userId = _userManager.GetUserId(User);
        if (survey.CreatedById != userId && !User.IsInRole(ApplicationRoles.Admin))
        {
            return Forbid();
        }

        var question = new Question
        {
            Text = model.Text,
            Type = model.Type,
            IsRequired = model.IsRequired,
            Order = model.Order,
            SurveyId = surveyId,
            Options = model.Options?.Select(o => new QuestionOption
            {
                Text = o.Text,
                Order = o.Order
            }).ToList() ?? new List<QuestionOption>()
        };

        var createdQuestion = await _questionRepository.CreateQuestionAsync(question);
        var result = new QuestionDTO
        {
            Id = createdQuestion.Id,
            Text = createdQuestion.Text,
            Type = createdQuestion.Type,
            IsRequired = createdQuestion.IsRequired,
            Order = createdQuestion.Order,
            Options = createdQuestion.Options?.Select(o => new QuestionOptionDTO
            {
                Id = o.Id,
                Text = o.Text,
                Order = o.Order
            }).ToList() ?? new List<QuestionOptionDTO>()
        };

        return CreatedAtAction(nameof(GetQuestionsBySurveyId), new { surveyId = surveyId }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = ApplicationRoles.Admin + "," + ApplicationRoles.Surveyor)]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionDTO model)
    {
        var question = await _questionRepository.GetQuestionByIdAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        var survey = await _surveyRepository.GetSurveyByIdAsync(question.SurveyId);
        if (survey == null)
        {
            return NotFound("Survey not found");
        }

        var userId = _userManager.GetUserId(User);
        if (survey.CreatedById != userId && !User.IsInRole(ApplicationRoles.Admin))
        {
            return Forbid();
        }

        question.Text = model.Text;
        question.Type = model.Type;
        question.IsRequired = model.IsRequired;
        question.Order = model.Order;

        await _questionRepository.UpdateQuestionAsync(question);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = ApplicationRoles.Admin + "," + ApplicationRoles.Surveyor)]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _questionRepository.GetQuestionByIdAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        var survey = await _surveyRepository.GetSurveyByIdAsync(question.SurveyId);
        if (survey == null)
        {
            return NotFound("Survey not found");
        }

        var userId = _userManager.GetUserId(User);
        if (survey.CreatedById != userId && !User.IsInRole(ApplicationRoles.Admin))
        {
            return Forbid();
        }

        await _questionRepository.DeleteQuestionAsync(id);
        return NoContent();
    }
}