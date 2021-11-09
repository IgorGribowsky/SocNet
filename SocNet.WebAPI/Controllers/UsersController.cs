using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocNet.Core.Entities;
using SocNet.Services.UsersManaging;
using SocNet.Services.PostsManaging;

namespace SocNet.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersManagingService _usersManager;
        private readonly IPostsManagingService _postsManager;

        public UsersController(IUsersManagingService usersManager, IPostsManagingService postsManager)
        {
            _usersManager = usersManager;
            _postsManager = postsManager;
        }

        [HttpGet("{id}")]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        public async Task<ActionResult<List<User>>> Get([FromQuery] int page = 1, [FromQuery] int page_size = 5)
        {
            var users = await _usersManager.GetUsersAsync(page: page, pageSize: page_size);

            return Ok(users);
        }

        [HttpGet("{id}/posts")]
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

        [HttpGet("{id}/feed")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Post>>> GetFeed([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int page_size = 10)
        {
            var requestedUser = await _usersManager.GetByIdAsync(id);

            if (requestedUser is null)
            {
                return NotFound();
            }

            var posts = await _postsManager.GetFeedByUserIdAsync(id: id, page: page, pageSize: page_size);

            return Ok(posts);
        }
    }
}
