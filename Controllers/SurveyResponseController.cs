using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyPortalAPI.Models;
using SurveyPortalAPI.Repositories;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SurveyResponsesController : ControllerBase
{
    private readonly SurveyResponseRepository _responseRepository;
    private readonly SurveyRepository _surveyRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public SurveyResponsesController(
        SurveyResponseRepository responseRepository,
        SurveyRepository surveyRepository,
        UserManager<ApplicationUser> userManager)
    {
        _responseRepository = responseRepository;
        _surveyRepository = surveyRepository;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<ActionResult<SurveyResponseDTO>> SubmitResponse([FromBody] CreateSurveyResponseDTO model)
    {
        var survey = await _surveyRepository.GetSurveyByIdAsync(model.SurveyId);
        if (survey == null)
        {
            return NotFound("Survey not found");
        }

        if (!survey.IsActive ||
            (survey.StartDate.HasValue && survey.StartDate > DateTime.Now) ||
            (survey.EndDate.HasValue && survey.EndDate < DateTime.Now))
        {
            return BadRequest("Survey is not active");
        }

        var userId = _userManager.GetUserId(User);

        var response = new SurveyResponse
        {
            SurveyId = model.SurveyId,
            UserId = userId,
            SubmittedDate = DateTime.Now,
            Answers = model.Answers.Select(a => new Answer
            {
                QuestionId = a.QuestionId,
                TextAnswer = a.TextAnswer,
                SelectedOptionId = a.SelectedOptionId
            }).ToList()
        };

        var createdResponse = await _responseRepository.CreateResponseAsync(response);
        var result = await _responseRepository.GetResponseByIdAsync(createdResponse.Id);

        return CreatedAtAction(nameof(GetResponse), new { id = result.Id }, MapToSurveyResponseDTO(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SurveyResponseDTO>> GetResponse(int id)
    {
        var response = await _responseRepository.GetResponseByIdAsync(id);
        if (response == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (response.UserId != userId && !User.IsInRole(ApplicationRoles.Admin))
        {
            return Forbid();
        }

        return Ok(MapToSurveyResponseDTO(response));
    }

    [HttpGet("survey/{surveyId}")]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<ActionResult<IEnumerable<SurveyResponseDTO>>> GetResponsesBySurveyId(int surveyId)
    {
        var responses = await _responseRepository.GetResponsesBySurveyIdAsync(surveyId);
        var responseDTOs = responses.Select(r => MapToSurveyResponseDTO(r)).ToList();
        return Ok(responseDTOs);
    }

    [HttpGet("my-responses")]
    public async Task<ActionResult<IEnumerable<SurveyResponseDTO>>> GetMyResponses()
    {
        var userId = _userManager.GetUserId(User);
        var responses = await _responseRepository.GetResponsesByUserIdAsync(userId);
        var responseDTOs = responses.Select(r => MapToSurveyResponseDTO(r)).ToList();
        return Ok(responseDTOs);
    }

    private SurveyResponseDTO MapToSurveyResponseDTO(SurveyResponse response)
    {
        return new SurveyResponseDTO
        {
            Id = response.Id,
            SurveyId = response.SurveyId,
            SurveyTitle = response.Survey?.Title,
            UserId = response.UserId,
            UserFullName = response.User != null ? $"{response.User.FirstName} {response.User.LastName}" : "",
            SubmittedDate = response.SubmittedDate,
            Answers = response.Answers?.Select(a => new AnswerDTO
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                QuestionText = a.Question?.Text,
                TextAnswer = a.TextAnswer,
                SelectedOptionId = a.SelectedOptionId,
                SelectedOptionText = a.SelectedOption?.Text
            }).ToList()
        };
    }
}