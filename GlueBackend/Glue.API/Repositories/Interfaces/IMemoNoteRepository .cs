using Glue.API.Database.Entities.GlueMemoNote;

namespace Glue.API.Repositories.Interfaces;

public interface IMemoNoteRepository : IBaseRepository<GlueMemoNote>
{
    Task<IEnumerable<GlueMemoNote>> GetByUserIdAsync(string userId);
    Task<IEnumerable<GlueMemoNote>> SearchByKeywordAsync(string userId, string keyword);
}
