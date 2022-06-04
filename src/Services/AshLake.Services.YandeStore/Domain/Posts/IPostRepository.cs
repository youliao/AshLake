﻿namespace AshLake.Services.YandeStore.Domain.Posts;

public interface IPostRepository
{
    Task<int> AddAsync(Post post);
    Task<int> AddRangeAsync(IEnumerable<Post> post);
    Task<int> UpdateAsync(Post post);
    Task<int> AddOrUpdateAsync(Post post);
    Task<int> DeleteAsync(int postId);
    Task<Post?> GetAsync(int postId);
    Task<IEnumerable<int>> GetChildIdsAsync(int parentId);
    Task<IEnumerable<object>> FindAsync(List<string> tags, List<PostRating> rating, List<PostStatus> status,int limit);
}
