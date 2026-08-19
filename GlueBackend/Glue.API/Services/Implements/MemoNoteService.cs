using Glue.API.Mappers;
using Glue.API.Models.Dtos.MemoNote;
using Glue.API.Repositories.Interfaces;
using Glue.API.Services.Interfaces;

namespace Glue.API.Services.Implements;

public class MemoNoteService : IMemoNoteService
{
    private readonly IMemoNoteRepository _repository;

    public MemoNoteService(IMemoNoteRepository repository)
    {
        _repository = repository;
    }

    #region -- GetUserMemosAsync()
    public async Task<IEnumerable<MemoNoteDto>> GetUserMemosAsync(string userId, string? category = null)
    {
        var memos = await _repository.GetByUserIdAsync(userId);

        if (!string.IsNullOrEmpty(category))
        {
            memos = memos.Where(m => m.Category == category);
        }

        return MemoNoteMapper.ToDtoList(memos);
    }
    #endregion

    #region -- GetByIdAsync()
    public async Task<MemoNoteDto?> GetByIdAsync(string id, string userId)
    {
        var memo = await _repository.GetByIdAsync(id);

        if (memo == null || memo.UserId != userId)
            return null;

        return MemoNoteMapper.ToDto(memo);
    }
    #endregion

    #region -- CreateAsync()
    public async Task<MemoNoteDto> CreateAsync(string userId, MemoNoteRequestDto requestDto)
    {
        var dto = MemoNoteMapper.RequestToDto(requestDto);

        var entity = MemoNoteMapper.ToEntity(dto, userId);

        await _repository.CreateAsync(entity);

        return MemoNoteMapper.ToDto(entity);
    }
    #endregion

    #region -- UpdateAsync()
    public async Task<bool> UpdateAsync(string id, string userId, MemoNoteRequestDto requestDto)
    {
        var dto = MemoNoteMapper.RequestToDto(requestDto);

        var existingMemo = await _repository.GetByIdAsync(id);
        if (existingMemo == null || existingMemo.UserId != userId)
            return false;

        var entity = MemoNoteMapper.ToEntity(dto, userId);

        return await _repository.UpdateAsync(entity);
    }
    #endregion

    #region -- DeleteAsync()
    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var memo = await _repository.GetByIdAsync(id);
        if (memo == null || memo.UserId != userId)
            return false;

        return await _repository.DeleteAsync(id);
    }
    #endregion

    #region -- SearchByKeywordAsync()
    public async Task<IEnumerable<MemoNoteDto>> SearchByKeywordAsync(string userId, string keyword)
    {
        var memos = await _repository.SearchByKeywordAsync(userId, keyword);
        return MemoNoteMapper.ToDtoList(memos);
    }
    #endregion
}
