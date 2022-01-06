using SocNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocNet.Infrastructure.Interfaces;
using SocNet.Services.SubscriptionManaging;
using SocNet.Services.LikesManaging;

namespace SocNet.Services.PostsManaging;

public class PostsManagingService : IPostsManagingService
{
    private readonly IRepository _repository;
    private readonly ISubscriptionManagingService _subscriptionManager;
    private readonly ILikesManagingService _likesManager;


    public PostsManagingService(IRepository repository, ISubscriptionManagingService subscriptionManager, ILikesManagingService likesManager)
    {
        _repository = repository;
        _subscriptionManager = subscriptionManager;
        _likesManager = likesManager;
    }

    public async Task<Post> CreateAsync(Post post)
    {
        post.CreationTime = DateTime.Now;
        var createdPost = await _repository.CreateAsync<Post>(post);

        return createdPost;
    }

    public async Task DeleteByIdAsync(int id)
    {
        await _repository.DeleteByIdAsync<Post>(id);

        await _likesManager.DeleteAllLikesFromPostById(id);
    }

    public async Task<Post> GetByIdAsync(int id)
    {
        var user = await Task.Run(() => _repository.GetById<Post>(id));

        return user;
    }

    public async Task<IEnumerable<Post>> GetChildrenAsync(int id)
    {
        var childrenPosts =
            await Task.Run(() => _repository.Query<Post>().Where(p => p.ParentPostId == id).Select(p => p).OrderBy(p => p.CreationTime));

        return childrenPosts;
    }

    public async Task<IEnumerable<Post>> GetFeedByUserIdAsync(int id, int page, int pageSize)
    {
        var skippedPosts = (page - 1) * pageSize;

        var subsIds = (await _subscriptionManager.GetSubscriptionsByUserIdAsync(id)).Select(u => u.Id);
        var posts = await _repository.Query<Post>().Where(p => subsIds.Contains(p.UserId))
            .OrderByDescending(p => p.CreationTime).Skip(skippedPosts).Take(pageSize).ToListAsync();
        return posts;
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(int page = 1, int pageSize = 10)
    {
        var skippedPosts = (page - 1) * pageSize;

        var posts =
            await Task.Run(() => _repository.Query<Post>().Select(p => p).OrderByDescending(p => p.CreationTime).Skip(skippedPosts).Take(pageSize));

        return posts;
    }

    public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(int id, int page = 1, int pageSize = 10)
    {
        var skippedPosts = (page - 1) * pageSize;

        var posts =
            await Task.Run(() => _repository.Query<Post>().Where(p => p.UserId == id).Select(p => p).OrderByDescending(p => p.CreationTime).Skip(skippedPosts).Take(pageSize));

        return posts;
    }
    public async Task AddLikeByPostIdAsync(int userId, int postId)
    {
        await _likesManager.LikePostById(userId, postId);

        var post = await _repository.GetByIdAsync<Post>(postId);

        post.LikeCount++;

        await _repository.UpdateAsync(post);
    }

    public async Task RemoveLikeByPostIdAsync(int userId, int postId)
    {
        await _likesManager.UnlikePostById(userId, postId);

        var post = await _repository.GetByIdAsync<Post>(postId);

        post.LikeCount--;

        await _repository.UpdateAsync(post);
    }

    public async Task<bool> ValidatePostDataAsync(Post post)
    {
        if (post.ParentPostId is not null)
        {
            var parentPost = await GetByIdAsync((int)post.ParentPostId);

            if (parentPost is null)
                return false;
        }

        return true;
    }
}
