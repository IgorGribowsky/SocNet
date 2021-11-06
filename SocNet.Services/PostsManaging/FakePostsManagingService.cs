using SocNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Services.PostsManaging
{
    public class FakePostsManagingService : IPostsManagingService
    {
        private readonly List<Post> _fakePosts;

        public FakePostsManagingService()
        {
            int fakePostsNumber = 10;
            _fakePosts = new List<Post>();
            var rand = new Random();

            for (int i = 1; i <= fakePostsNumber; i++)
            {
                var userId = rand.Next(0, 99);
                _fakePosts.Add(new Post 
                { 
                    Id = i, 
                    UserId = userId,
                    Content = $"This is post {i} from user {userId}", 
                    ParentPostId = i > 1 ? i - 1 : null, 
                    LikeCount = rand.Next(0, 99),
                    CommentCount = rand.Next(0, 99),
                });
            }
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            var requestedPost = _fakePosts.FirstOrDefault(p => p.Id == id);

            return requestedPost;
        }

        public async Task<IEnumerable<Post>> GetChildrenAsync(int id)
        {
            var childrenPosts = _fakePosts.Where(p => p.ParentPostId == id).Select(p => p);

            return childrenPosts;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(int page = 1, int pageSize = 10)
        {
            var skippedPosts = (page - 1) * pageSize;

            var posts = _fakePosts.Select(p => p).Skip(skippedPosts).Take(pageSize);

            return posts;
        }
    }
}
