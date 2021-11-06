﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Services.PostsManaging
{
    public interface IPostsManagingService
    {
        public Task<Post> GetByIdAsync(int id);

        public Task<IEnumerable<Post>> GetChildrenAsync(int id);

        public Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize);
    }
}
