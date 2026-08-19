using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.Task;
using Glue.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Glue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : BaseController
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service)
    {
        _service = service;
    }

    #region -- GetWorkbench()
    /// <summary>
    /// 获取任务工作台（今天的任务）
    /// </summary>
    [HttpGet("workbench")]
    public async Task<ActionResult<ApiResponseDto<TaskWorkbenchDto>>> GetWorkbench()
    {
        var userId = GetUserId();
        var workbench = await _service.GetWorkbenchAsync(userId);
        return Ok(ApiResponseDto<TaskWorkbenchDto>.Ok(workbench));
    }
    #endregion

    #region -- GetList()
    /// <summary>
    /// 获取任务列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<TaskDto>>>> GetList(
        [FromQuery] DateTime? date = null)
    {
        var userId = GetUserId();
        var tasks = await _service.GetUserTasksAsync(userId, date);
        return Ok(ApiResponseDto<IEnumerable<TaskDto>>.Ok(tasks));
    }
    #endregion

    #region -- GetById()
    /// <summary>
    /// 根据 ID 获取任务
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<TaskDto>>> GetById(string id)
    {
        var userId = GetUserId();
        var task = await _service.GetByIdAsync(id, userId);

        if (task == null)
            return NotFound(ApiResponseDto<TaskDto>.Error("任务不存在或无权访问"));

        return Ok(ApiResponseDto<TaskDto>.Ok(task));
    }
    #endregion

    #region -- Create()
    /// <summary>
    /// 创建任务
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TaskDto>>> Create([FromBody] TaskRequestDto request)
    {
        var userId = GetUserId();
        var task = await _service.CreateAsync(userId, request);
        return Ok(ApiResponseDto<TaskDto>.Ok(task, "任务创建成功"));
    }
    #endregion

    #region -- Update()
    /// <summary>
    /// 更新任务
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<TaskDto>>> Update(string id, [FromBody] TaskRequestDto request)
    {
        var userId = GetUserId();
        var task = await _service.UpdateAsync(id, userId, request);

        if (task == null)
            return BadRequest(ApiResponseDto<TaskDto>.Error("更新失败，任务不存在或无权操作"));

        return Ok(ApiResponseDto<TaskDto>.Ok(task, "任务更新成功"));
    }
    #endregion

    #region -- Delete()
    /// <summary>
    /// 删除任务
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(string id)
    {
        var userId = GetUserId();
        var result = await _service.DeleteAsync(id, userId);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("删除失败，任务不存在或无权操作"));

        return Ok(ApiResponseDto<object>.Ok(null, "任务已删除"));
    }
    #endregion

    #region -- Complete()
    /// <summary>
    /// 完成任务
    /// </summary>
    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<ApiResponseDto<TaskDto>>> Complete(string id)
    {
        var userId = GetUserId();
        var task = await _service.CompleteTaskAsync(id, userId);

        if (task == null)
            return BadRequest(ApiResponseDto<TaskDto>.Error("操作失败，任务不存在或无权操作"));

        return Ok(ApiResponseDto<TaskDto>.Ok(task, "任务已完成"));
    }
    #endregion

    #region -- UpdateReminder()
    /// <summary>
    /// 更新任务提醒
    /// </summary>
    [HttpPatch("{id}/reminder")]
    public async Task<ActionResult<ApiResponseDto<TaskDto>>> UpdateReminder(string id, [FromBody] TaskDto request)
    {
        var userId = GetUserId();
        var task = await _service.UpdateReminderAsync(id, userId, request);

        if (task == null)
            return BadRequest(ApiResponseDto<TaskDto>.Error("更新失败，任务不存在或无权操作"));

        return Ok(ApiResponseDto<TaskDto>.Ok(task, "提醒设置已更新"));
    }
    #endregion
}
