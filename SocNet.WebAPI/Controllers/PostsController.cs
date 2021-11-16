using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocNet.Core.Entities;
using SocNet.Services.PostsManaging;
using SocNet.WebAPI.Models;
using System.Net.Mime;

namespace SocNet.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostsManagingService _postManager;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostsManagingService postManager, ILogger<PostsController> logger)
        {
            _postManager = postManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        public async Task<ActionResult<List<Post>>> Get([FromQuery] int page = 1, [FromQuery] int page_size = 10)
        {
            var posts = await _postManager.GetPostsAsync(page: page, pageSize: page_size);

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var requestedPost = await _postManager.GetByIdAsync(id);

            if (requestedPost is null)
            {
                return NotFound(new { message = "PPost doesn't exist" });
            }

            return Ok(requestedPost);
        }

        [HttpGet("{id}/parent")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetParent(int id)
        {
            var requestedPost = await _postManager.GetByIdAsync(id);

            if (requestedPost is null)
            {
                return NotFound(new { message = "Can't provide parent of nonexistent post" });
            }

            if (requestedPost.ParentPostId is null)
            {
                return Ok(null);
            }

            var parentPost = await _postManager.GetByIdAsync((int)requestedPost.ParentPostId);

            return Ok(parentPost); // even if it is null!
        }

        [HttpGet("{id}/children")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Post>>> GetChildren(int id)
        {
            var requestedPost = await _postManager.GetByIdAsync(id);

            if (requestedPost is null)
            {
                return NotFound(new { message = "Can't provide children of nonexistent post" });
            }

            var childrenPosts = await _postManager.GetChildrenAsync(requestedPost.Id);

            return Ok(childrenPosts);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Post>> CreateAsync([FromBody] InputPostData postData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdStr = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (userIdStr is null || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var post = new Post { Content = postData.Content, UserId = userId, ParentPostId = postData.ParentPostId };

            if (!await _postManager.ValidatePostDataAsync(post))
            {
                return BadRequest();
            }

            var publishedPost = await _postManager.CreateAsync(post);

            return CreatedAtAction(nameof(GetById), new { id = publishedPost.Id }, publishedPost);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Delete(int id)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (userIdStr is null || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var targetPost = await _postManager.GetByIdAsync(id);

            if (targetPost is null)
            {
                return NotFound(new { message = "Post you are trying to delete doesn't exist" });
            }

            if (targetPost.UserId != userId)
            {
                return Unauthorized(new { message = "You don't have rights to delete this post"});
            }

            await _postManager.DeleteByIdAsync(id);

            return NoContent();
        }
    }
}
