using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.MemoNote;
using Glue.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Glue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemoNoteController : BaseController
{
    private readonly IMemoNoteService _service;

    public MemoNoteController(IMemoNoteService service)
    {
        _service = service;
    }

    #region -- GetList()
    [HttpGet]
    public async Task<ApiResponseDto<IEnumerable<MemoNoteDto>>> GetList(
    [FromQuery] string? category,
    [FromQuery] string? keyword,
    [FromQuery] string? tag,
    [FromQuery] string? sort = "updated-desc")
    {
        var userId = GetUserId();

        IEnumerable<MemoNoteDto> memos;

        if (!string.IsNullOrEmpty(keyword))
        {
            memos = await _service.SearchByKeywordAsync(userId, keyword);
        }
        else
        {
            memos = await _service.GetUserMemosAsync(userId, category);
        }

        // 排序
        memos = sort switch
        {
            "created-asc" => memos.OrderBy(m => m.CreatedAt),
            "created-desc" => memos.OrderByDescending(m => m.CreatedAt),
            "updated-asc" => memos.OrderBy(m => m.UpdatedAt),
            "updated-desc" => memos.OrderByDescending(m => m.UpdatedAt),
            _ => memos.OrderByDescending(m => m.UpdatedAt)
        };

        return ApiResponseDto<IEnumerable<MemoNoteDto>>.Ok(memos);
    }
    #endregion

    #region -- GetById()
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<MemoNoteDto>>> GetById([FromRoute] string id)
    {
        var userId = GetUserId();
        var memo = await _service.GetByIdAsync(id, userId);

        if (memo == null)
            return NotFound(ApiResponseDto<MemoNoteDto>.Error("备忘录不存在或无权访问"));

        return Ok(ApiResponseDto<MemoNoteDto>.Ok(memo));
    }
    #endregion

    #region -- Create()
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<MemoNoteRequestDto>>> Create([FromBody] MemoNoteRequestDto request)
    {
        var userId = GetUserId();
        var memo = await _service.CreateAsync(userId, request);

        return Ok(ApiResponseDto<MemoNoteDto>.Ok(memo, "创建成功"));
    }
    #endregion

    #region -- Update()
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<MemoNoteDto>>> Update(string id, [FromBody] MemoNoteRequestDto request)
    {
        var userId = GetUserId();
        var result = await _service.UpdateAsync(id, userId, request);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("更新失败，备忘录不存在或无权操作"));

        return Ok(ApiResponseDto<object>.Ok(null, "更新成功"));
    }
    #endregion

    #region -- Delete()
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(string id)
    {
        var userId = GetUserId();
        var result = await _service.DeleteAsync(id, userId);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("删除失败，备忘录不存在或无权操作"));

        return Ok(ApiResponseDto<object>.Ok(null, "删除成功"));
    }
    #endregion
}