using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;
using SocNet.Infrastructure.Interfaces;

namespace SocNet.Services.SubscriptionManaging
{
    public class SubscriptionManagingService : ISubscriptionManagingService
    {
        private readonly IRepository _repository;

        public SubscriptionManagingService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CheckSubscribtionExistance(int subscriberUserId, int targetUserId)
        {
            return await Task.Run(() => 
                _repository.Query<Subscription>().Any(s => s.SubscriberUserId == subscriberUserId && s.TargetUserId == targetUserId)
            );
        }

        public async Task<IEnumerable<User>> GetSubscriptionsByUserIdAsync(int id)
        {
            var ids = await Task.Run(() =>
                _repository.Query<Subscription>().Where(s => s.SubscriberUserId == id).Select(s => s.TargetUserId)
            );

            return await Task.Run(() => _repository.Query<User>().Where(u => ids.Contains(u.Id)));
        }

        public async Task<IEnumerable<User>> GetSubscribersByUserIdAsync(int id)
        {
            var ids = await Task.Run(() =>
                _repository.Query<Subscription>().Where(s => s.TargetUserId == id).Select(s => s.SubscriberUserId)
            );

            return await Task.Run(() => _repository.Query<User>().Where(u => ids.Contains(u.Id)));
        }

        public async Task SubscribeToUserByIdAsync(int subscriberUserId, int targetUserId)
        {
            var subscription = new Subscription {SubscriberUserId = subscriberUserId, TargetUserId = targetUserId};
            await _repository.CreateAsync(subscription);
        }

        public async Task UnsubscribeFromUserByIdAsync(int subscriberUserId, int targetUserId)
        {
            var subscription = await Task.Run(() =>
                _repository.Query<Subscription>().FirstOrDefault(s => s.SubscriberUserId == subscriberUserId && s.TargetUserId == targetUserId)
            );

            await _repository.DeleteAsync<Subscription>(subscription);
        }
    }
}
