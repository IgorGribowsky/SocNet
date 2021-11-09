using SocNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Infrastructure.Interfaces;

namespace SocNet.Services.PostsManaging
{
    public class PostsManagingService : IPostsManagingService
    {
        private readonly IRepository _repository;

        public PostsManagingService(IRepository repository)
        {
            _repository = repository;
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
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            var user = await Task.Run(() => _repository.GetById<Post>(id));

            return user;
        }

        public async Task<IEnumerable<Post>> GetChildrenAsync(int id)
        {
            var childrenPosts = 
                await Task.Run(() => _repository.Query<Post>().Where(p => p.ParentPostId == id).Select(p => p).OrderBy(p=>p.CreationTime));
            
            return childrenPosts;
        }

        public Task<IEnumerable<Post>> GetFeedByUserIdAsync(int id, int page, int pageSize)
        {
            throw new NotImplementedException();
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
    }
}
