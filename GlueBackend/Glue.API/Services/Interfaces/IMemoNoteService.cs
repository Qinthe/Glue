using Glue.API.Models.Dtos.MemoNote;

namespace Glue.API.Services.Interfaces;

public interface IMemoNoteService
{
    Task<IEnumerable<MemoNoteDto>> GetUserMemosAsync(string userId, string? category = null);
    Task<MemoNoteDto?> GetByIdAsync(string id, string userId);
    Task<MemoNoteDto> CreateAsync(string userId, MemoNoteRequestDto requestDto);
    Task<bool> UpdateAsync(string id, string userId, MemoNoteRequestDto requestDto);
    Task<bool> DeleteAsync(string id, string userId);
    Task<IEnumerable<MemoNoteDto>> SearchByKeywordAsync(string userId, string keyword);
}
