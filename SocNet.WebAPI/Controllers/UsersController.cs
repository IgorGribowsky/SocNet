using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocNet.Core.Entities;
using SocNet.Services.UsersManaging;
using SocNet.Services.PostsManaging;
using SocNet.Services.SubscriptionManaging;

namespace SocNet.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersManagingService _usersManager;
        private readonly IPostsManagingService _postsManager;
        private readonly ISubscriptionManagingService _subscriptionManager;

        public UsersController(IUsersManagingService usersManager, IPostsManagingService postsManager, ISubscriptionManagingService subscriptionManager)
        {
            _usersManager = usersManager;
            _postsManager = postsManager;
            _subscriptionManager = subscriptionManager;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var requestedUser = await _usersManager.GetByIdAsync(id);

            if (requestedUser is null)
            {
                return NotFound();
            }

            return Ok(requestedUser);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        public async Task<ActionResult<List<User>>> Get([FromQuery] int page = 1, [FromQuery] int page_size = 5)
        {
            var users = await _usersManager.GetUsersAsync(page: page, pageSize: page_size);

            return Ok(users);
        }

        [HttpGet("{id}/posts")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Post>>> GetPostsByUserId([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int page_size = 10)
        {
            var requestedUser = await _usersManager.GetByIdAsync(id);

            if (requestedUser is null)
            {
                return NotFound();
            }

            var posts = await _postsManager.GetPostsByUserIdAsync(id: id, page: page, pageSize: page_size);

            return Ok(posts);
        }

        [HttpGet("/api/feed")] // absolute path looks like a bad idea
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Post>>> GetFeed([FromQuery] int page = 1, [FromQuery] int page_size = 10)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (userIdStr is null || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var posts = await _postsManager.GetFeedByUserIdAsync(id: userId, page: page, pageSize: page_size);

            return Ok(posts);
        }

        [HttpGet("{id}/subscriptions")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSubscriptions([FromRoute] int id)
        {
            var requestedUser = await _usersManager.GetByIdAsync(id);

            if (requestedUser is null)
            {
                return NotFound();
            }

            var subs = await _subscriptionManager.GetSubscriptionsByUserIdAsync(id);

            return Ok(subs);
        }

        [HttpGet("{id}/subscribers")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSubscribers([FromRoute] int id)
        {
            var requestedUser = await _usersManager.GetByIdAsync(id);

            if (requestedUser is null)
            {
                return NotFound();
            }

            var subs = await _subscriptionManager.GetSubscribersByUserIdAsync(id);

            return Ok(subs);
        }

        [HttpPost("{id}/subscriptions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Subscribe([FromRoute] int id)
        {
            var subscriberIdStr = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (subscriberIdStr is null || !int.TryParse(subscriberIdStr, out var subscriberId))
            {
                return Unauthorized();
            }

            var targetUser = await _usersManager.GetByIdAsync(id);
            if (targetUser is null)
            {
                return NotFound(new { message = "User doesn't exist" });
            }

            var isSubscriptionExists = await _subscriptionManager.CheckSubscriptionExistance(subscriberUserId: subscriberId, targetUserId: targetUser.Id);
            if (isSubscriptionExists)
            {
                return BadRequest(new { message = "You already subscribed to this user"});
            }

            await _subscriptionManager.SubscribeToUserByIdAsync(subscriberUserId: subscriberId, targetUserId: targetUser.Id);

            return Ok();
        }

        [HttpDelete("{id}/subscriptions")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (userIdStr is null || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var requestedUser = await _usersManager.GetByIdAsync(id);

            if (requestedUser is null)
            {
                return NotFound();
            }

            await _subscriptionManager.UnsubscribeFromUserByIdAsync(userId, id);

            return Ok();
        }
    }
}
