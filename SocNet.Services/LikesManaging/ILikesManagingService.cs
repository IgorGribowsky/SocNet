using SocNet.Core.Entities;
using SocNet.Services.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Services.LikesManaging
{
    public interface ILikesManagingService
    {
        public Task<IEnumerable<User>> GetLikesByPostIdAsync(int id, RequestPageData pageData);

        public Task<IEnumerable<Post>> GetLikedPostsByUserId(int id, RequestPageData pageData);

        public Task LikePostById(int userId, int postId);

        public Task UnlikePostById(int userId, int postId);

        public Task<bool> CheckIfPostLiked(int userId, int postId);

        public Task DeleteAllLikesFromPostById(int postId);
    }
}
