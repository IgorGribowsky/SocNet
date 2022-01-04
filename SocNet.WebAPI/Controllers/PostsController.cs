using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocNet.Core.Entities;
using SocNet.Services.AuthenticationManaging;
using SocNet.Services.LikesManaging;
using SocNet.Services.PostsManaging;
using SocNet.Services.UtilityModels;
using SocNet.WebAPI.Models;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SocNet.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostsManagingService _postManager;
    private readonly ILogger<PostsController> _logger;
    private readonly ILikesManagingService _likesManager;
    private readonly ICustomAuthenticationService _authenticationManager;

    public PostsController(IPostsManagingService postManager, ILogger<PostsController> logger, ILikesManagingService likesManager, ICustomAuthenticationService authenticationManager)
    {
        _postManager = postManager;
        _logger = logger;
        _likesManager = likesManager;
        _authenticationManager = authenticationManager;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
    public async Task<ActionResult<List<Post>>> GetAllAsync([FromQuery] int page = 1, [FromQuery] int page_size = 10)
    {
        var posts = await _postManager.GetPostsAsync(page: page, pageSize: page_size);

        return Ok(posts);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Post>> GetByIdAsync([FromRoute] int id)
    {
        var requestedPost = await _postManager.GetByIdAsync(id);

        if (requestedPost is null)
        {
            return NotFound(new { message = "Post doesn't exist" });
        }

        return Ok(requestedPost);
    }

    [HttpGet("{id}/parent")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Post>> GetParentAsync(int id)
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
    public async Task<ActionResult<List<Post>>> GetChildrenAsync(int id)
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
        if (!_authenticationManager.TryGetUserId(HttpContext.User, out int userId))
        {
            return Unauthorized(new { message = "Provide valid bearer token" });
        }

        var post = new Post { Content = postData.Content, UserId = userId, ParentPostId = postData.ParentPostId > 0 ? postData.ParentPostId : null };

        if (!await _postManager.ValidatePostDataAsync(post))
        {
            return BadRequest();
        }

        var publishedPost = await _postManager.CreateAsync(post);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = publishedPost.Id }, publishedPost);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        if (!_authenticationManager.TryGetUserId(HttpContext.User, out int userId))
        {
            return Unauthorized(new { message = "Provide valid bearer token" });
        }

        var targetPost = await _postManager.GetByIdAsync(id);

        if (targetPost is null)
        {
            return NotFound(new { message = "Post you are trying to delete doesn't exist" });
        }

        if (targetPost.UserId != userId)
        {
            return Unauthorized(new { message = "You don't have rights to delete this post" });
        }

        await _postManager.DeleteByIdAsync(id);

        return NoContent();
    }

    [HttpGet("{postId}/likes")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<User>>> GetUsersThatLikedPostAsync([FromRoute] int postId, [FromQuery] int page = 1, [FromQuery] int page_size = 10)
    {
        if (postId < 1)
        {
            return NotFound(new { message = "Post not found" });
        }

        var post = await _postManager.GetByIdAsync(postId);
        if (post is null)
        {
            return NotFound(new { message = "Post not found" });
        }

        var users = await _likesManager.GetLikesByPostIdAsync(postId, new RequestPageData { PageIndex = page, PageSize = page_size });

        return Ok(users);
    }

    [HttpPost("{postId}/likes")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> LikePostByIdAsync([FromRoute] int postId)
    {
        if (!_authenticationManager.TryGetUserId(HttpContext.User, out int userId))
        {
            return Unauthorized(new { message = "Provide valid bearer token" });
        }

        if (postId < 1)
        {
            return NotFound(new { message = "Post not found" });
        }

        var post = await _postManager.GetByIdAsync(postId);
        if (post is null)
        {
            return NotFound(new { message = "Post not found" });
        }

        if (await _likesManager.CheckIfPostLiked(userId, postId))
        {
            return BadRequest(new { message = "You already liked this post" });
        }

        await _postManager.AddLikeByPostIdAsync(userId, postId);
        return NoContent();
    }

    [HttpDelete("{postId}/likes")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UnlikePostByIdAsync([FromRoute] int postId)
    {
        if (!_authenticationManager.TryGetUserId(HttpContext.User, out int userId))
        {
            return Unauthorized(new { message = "Provide valid bearer token" });
        }

        if (postId < 1)
        {
            return NotFound(new { message = "Post not found" });
        }

        var post = await _postManager.GetByIdAsync(postId);
        if (post is null)
        {
            return NotFound(new { message = "Post not found" });
        }

        if (!await _likesManager.CheckIfPostLiked(userId, postId))
        {
            return BadRequest(new { message = "You don't like this post yet" });
        }

        await _postManager.RemoveLikeByPostIdAsync(userId, postId);

        return NoContent();
    }
}
