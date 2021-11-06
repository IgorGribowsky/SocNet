using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        public async Task<ActionResult<List<Post>>> Get([FromQuery] int page = 1, [FromQuery] int page_size = 10)
        {
            var posts = await _postManager.GetPostsAsync(page: page, pageSize: page_size);

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var requestedPost = await _postManager.GetByIdAsync(id);

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
            var requestedPost = await _postManager.GetByIdAsync(id);

            if (requestedPost is null)
            {
                return NotFound();
            }

            if (requestedPost.ParentPostId is null)
            {
                return Ok(null);
            }

            var parentPost = await _postManager.GetByIdAsync((int)requestedPost.ParentPostId);

            return Ok(parentPost); // even if it is null!
        }

        [HttpGet("{id}/children")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Post>>> GetChildren(int id)
        {
            var requestedPost = await _postManager.GetByIdAsync(id);

            if (requestedPost is null)
            {
                return NotFound();
            }

            var childrenPosts = await _postManager.GetChildrenAsync(requestedPost.Id);

            return Ok(childrenPosts);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Post>> CreateAsync([FromBody] InputPostData postData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var post = new Post { Content = postData.Content, UserId = postData.UserId, ParentPostId = postData.ParentPostId };

            var publishedPost = await _postManager.Create(post);

            return CreatedAtAction(nameof(GetById), new { id = publishedPost.Id }, publishedPost);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            return NoContent();
        }
    }
}
