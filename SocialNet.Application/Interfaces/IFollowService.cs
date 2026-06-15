using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.Interfaces;


//ესეც მერე შევქმენი
public interface IFollowService
{
    Task FollowAsync(string followerId, string followingId);
    Task UnFollowAsync(string followerId, string followingId);
    Task<bool> IsFollowingAsync(string followerId, string followingId);
    Task<int> GetFollowersCountAsync(string userId);
    Task<int> GetFollowingCountAsync(string userId);


    // 
    Task<List<string>> GetFollowersAsync(string userId);
    Task<List<string>> GetFollowingAsync(string  userId);
}
