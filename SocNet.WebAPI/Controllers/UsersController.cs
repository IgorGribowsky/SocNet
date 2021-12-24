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
using SocNet.Services.LikesManaging;
using SocNet.Services.UtilityModels;
using SocNet.Services.AuthenticationManaging;

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
        private readonly ILikesManagingService _likesManager;
        private readonly ICustomAuthenticationService _authenticationManager;

        public UsersController(IUsersManagingService usersManager, IPostsManagingService postsManager, ISubscriptionManagingService subscriptionManager, ILikesManagingService likesManager, ICustomAuthenticationService authenticationManager)
        {
            _usersManager = usersManager;
            _postsManager = postsManager;
            _subscriptionManager = subscriptionManager;
            _likesManager = likesManager;
            _authenticationManager = authenticationManager;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetByIdAsync(int id)
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
        public async Task<ActionResult<List<Post>>> GetPostsByUserIdAsync([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int page_size = 10)
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
        public async Task<ActionResult<List<Post>>> GetFeedAsync([FromQuery] int page = 1, [FromQuery] int page_size = 10)
        {
            if (!_authenticationManager.TryGetUserId(HttpContext.User, out int userId))
            {
                return Unauthorized(new { message = "Provide valid bearer token" });
            }

            var posts = await _postsManager.GetFeedByUserIdAsync(id: userId, page: page, pageSize: page_size);

            return Ok(posts);
        }

        [HttpGet("{id}/subscriptions")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSubscriptionsAsync([FromRoute] int id)
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
        public async Task<IActionResult> SubscribeAsync([FromRoute] int id)
        {
            if (!_authenticationManager.TryGetUserId(HttpContext.User, out int subscriberId))
            {
                return Unauthorized(new { message = "Provide valid bearer token" });
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
        public async Task<IActionResult> UnsubscribeAsync(int id)
        {
            if (!_authenticationManager.TryGetUserId(HttpContext.User, out int userId))
            {
                return Unauthorized(new { message = "Provide valid bearer token" });
            }

            var requestedUser = await _usersManager.GetByIdAsync(id);

            if (requestedUser is null)
            {
                return NotFound();
            }

            await _subscriptionManager.UnsubscribeFromUserByIdAsync(userId, id);

            return Ok();
        }

        [HttpGet("{userId}/likes")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<User>>> GetLikedPostsByUserId([FromRoute] int userId, [FromQuery] int page = 1, [FromQuery] int page_size = 10)
        {
            if (userId < 1)
            {
                return NotFound(new { message = "User not found" });
            }

            var user = await _usersManager.GetByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new { message = "User not found" });
            }

            var posts = await _likesManager.GetLikedPostsByUserId(userId, new RequestPageData { PageIndex = page, PageSize = page_size });

            return Ok(posts);
        }
    }
}
