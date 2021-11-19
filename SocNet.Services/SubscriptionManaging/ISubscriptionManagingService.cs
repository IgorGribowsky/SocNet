using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Services.SubscriptionManaging
{
    public interface ISubscriptionManagingService
    {
        public Task<IEnumerable<User>> GetSubscriptionsByUserIdAsync(int id);

        public Task<IEnumerable<User>> GetSubscribersByUserIdAsync(int id);

        public Task SubscribeToUserByIdAsync(int subscriberUserId, int targetUserId);

        public Task UnsubscribeFromUserByIdAsync(int subscriberUserId, int targetUserId);

        public Task<bool> CheckSubscriptionExistance(int subscriberUserId, int targetUserId);
    }
}
