using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyPortalAPI.Models;
using SurveyPortalAPI.Repositories;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SurveysController : ControllerBase
{
    private readonly SurveyRepository _surveyRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public SurveysController(
        SurveyRepository surveyRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _surveyRepository = surveyRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetSurveys()
    {
        var surveys = await _surveyRepository.GetActiveSurveysAsync();
        var surveyDTOs = _mapper.Map<List<SurveyDTO>>(surveys);
        return Ok(surveyDTOs);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<SurveyDTO>> GetSurvey(int id)
    {
        var survey = await _surveyRepository.GetSurveyByIdAsync(id);
        if (survey == null)
        {
            return NotFound();
        }
        return Ok(MapToSurveyDTO(survey));
    }

    [HttpGet("my-surveys")]
    public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetMySurveys()
    {
        var userId = _userManager.GetUserId(User);
        var surveys = await _surveyRepository.GetSurveysByUserIdAsync(userId);
        var surveyDTOs = surveys.Select(s => MapToSurveyDTO(s)).ToList();
        return Ok(surveyDTOs);
    }

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.Admin + "," + ApplicationRoles.Surveyor)]
    public async Task<ActionResult<SurveyDTO>> CreateSurvey([FromBody] CreateSurveyDTO model)
    {
        var userId = _userManager.GetUserId(User);

        var survey = _mapper.Map<Survey>(model);
        survey.CreatedById = userId;

        // Manually map questions and options
        survey.Questions = model.Questions?.Select(q => new Question
        {
            Text = q.Text,
            Type = q.Type,
            IsRequired = q.IsRequired,
            Order = q.Order,
            Options = q.Options?.Select(o => new QuestionOption
            {
                Text = o.Text,
                Order = o.Order
            }).ToList()
        }).ToList();

        var createdSurvey = await _surveyRepository.CreateSurveyAsync(survey);
        var result = await _surveyRepository.GetSurveyByIdAsync(createdSurvey.Id);

        return CreatedAtAction(nameof(GetSurvey), new { id = result.Id }, _mapper.Map<SurveyDTO>(result));
    }

    [HttpGet("surveyor-surveys")]
    [Authorize(Roles = ApplicationRoles.Surveyor)]
    public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetSurveyorSurveys()
    {
        var userId = _userManager.GetUserId(User);
        var surveys = await _surveyRepository.GetSurveysByUserIdAsync(userId);
        var surveyDTOs = _mapper.Map<List<SurveyDTO>>(surveys);
        return Ok(surveyDTOs);
    }

    [HttpGet("all-surveys")]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetAllSurveys()
    {
        var surveys = await _surveyRepository.GetAllSurveysAsync();
        var surveyDTOs = _mapper.Map<List<SurveyDTO>>(surveys);
        return Ok(surveyDTOs);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSurvey(int id, [FromBody] UpdateSurveyDTO model)
    {
        var survey = await _surveyRepository.GetSurveyByIdAsync(id);
        if (survey == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (survey.CreatedById != userId && !User.IsInRole(ApplicationRoles.Admin))
        {
            return Forbid();
        }

        survey.Title = model.Title;
        survey.Description = model.Description;
        survey.StartDate = model.StartDate;
        survey.EndDate = model.EndDate;
        survey.IsActive = model.IsActive;

        await _surveyRepository.UpdateSurveyAsync(survey);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSurvey(int id)
    {
        var survey = await _surveyRepository.GetSurveyByIdAsync(id);
        if (survey == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (survey.CreatedById != userId && !User.IsInRole(ApplicationRoles.Admin))
        {
            return Forbid();
        }

        await _surveyRepository.DeleteSurveyAsync(id);
        return NoContent();
    }

    private SurveyDTO MapToSurveyDTO(Survey survey)
    {
        return new SurveyDTO
        {
            Id = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            CreatedDate = survey.CreatedDate,
            StartDate = survey.StartDate,
            EndDate = survey.EndDate,
            IsActive = survey.IsActive,
            CreatedById = survey.CreatedById,
            CreatedByName = survey.CreatedBy != null ? $"{survey.CreatedBy.FirstName} {survey.CreatedBy.LastName}" : "",
            Questions = survey.Questions?.Select(q => new QuestionDTO
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
                }).ToList()
            }).ToList()
        };
    }
}