using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private List<Post> _fakePosts;

        public PostsController()
        {
            int fakePostsNumber = 10;
            _fakePosts = new List<Post>();

            for (int i = 1; i <= fakePostsNumber; i++)
            {
                _fakePosts.Add(new Post { Id = i, Content = $"This is post {i}", ParentPostId = i > 1 ? i - 1 : null });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        public async Task<ActionResult<List<Post>>> Get()
        {
            var posts = _fakePosts.Select(p => p);

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var requestedPost = _fakePosts.FirstOrDefault(p => p.Id == id);

            if (requestedPost is null)
            {
                return NotFound();
            }

            return Ok(requestedPost);
        }

        [HttpGet("{id}/parent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetParent(int id)
        {
            var requestedPost = _fakePosts.FirstOrDefault(p => p.Id == id);

            if (requestedPost is null)
            {
                return NotFound();
            }

            if (requestedPost.ParentPostId is null)
            {
                return Ok(null);
            }

            var parentPost = _fakePosts.FirstOrDefault(p => p.Id == (int)requestedPost.ParentPostId);

            return Ok(parentPost); // even if it is null!
        }

        [HttpGet("{id}/children")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Post>>> GetChildren(int id)
        {
            var requestedPost =  _fakePosts.FirstOrDefault(p => p.Id == id);

            if (requestedPost is null)
            {
                return NotFound();
            }

            var childrenPosts = _fakePosts.Where(p => p.ParentPostId == requestedPost.Id).Select(p => p);

            return Ok(childrenPosts);
        }
    }
}
