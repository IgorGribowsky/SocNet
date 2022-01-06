using SocNet.Core.Entities;
using SocNet.Services.UtilityModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocNet.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SocNet.Services.LikesManaging;

public class LikesManagingService : ILikesManagingService
{
    private readonly IRepository _repository;

    public LikesManagingService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CheckIfPostLiked(int userId, int postId)
    {
        return await _repository.Query<Like>().AnyAsync(l => l.SenderUserId == userId && l.PostId == postId);
    }

    public async Task DeleteAllLikesFromPostById(int postId)
    {
        var likes = await _repository.Query<Like>().Where(l => l.PostId == postId).ToListAsync();

        await _repository.DeleteManyAsync(likes);
    }

    public async Task<IEnumerable<Post>> GetLikedPostsByUserId(int id, RequestPageData pageData)
    {
        var postsIds = await _repository.Query<Like>().Where(l => l.SenderUserId == id).Select(l => l.PostId).ToListAsync();

        return await _repository.Query<Post>().Where(p => postsIds.Contains(p.UserId))
            .OrderByDescending(p => p.CreationTime).Skip(pageData.SkippedEntities).Take(pageData.PageSize).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetLikesByPostIdAsync(int id, RequestPageData pageData)
    {
        var usersIds = await _repository.Query<Like>().Where(l => l.PostId == id).Select(l => l.SenderUserId).ToListAsync();

        return await _repository.Query<User>().Where(u => usersIds.Contains(u.Id))
            .OrderBy(u => u.FirstName).Skip(pageData.SkippedEntities).Take(pageData.PageSize).ToListAsync();
    }

    public async Task LikePostById(int userId, int postId)
    {
        var like = new Like { PostId = postId, SenderUserId = userId };

        await _repository.CreateAsync(like);
    }

    public async Task UnlikePostById(int userId, int postId)
    {
        var like = await _repository.Query<Like>().FirstOrDefaultAsync(l => l.SenderUserId == userId && l.PostId == postId);

        await _repository.DeleteAsync(like);
    }
}
