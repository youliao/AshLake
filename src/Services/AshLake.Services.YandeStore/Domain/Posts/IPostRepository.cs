﻿using AshLake.Services.YandeStore.Infrastructure.Repositories.Posts;

namespace AshLake.Services.YandeStore.Domain.Posts;

public interface IPostRepository
{
    Task<int> AddAsync(Post post);
    Task<int> AddRangeAsync(IEnumerable<Post> post);
    Task<int> UpdateAsync(Post post);
    Task<int> UpdateRangeAsync(IEnumerable<Post> post);
    Task<int> AddOrUpdateAsync(Post post);
    Task<int> DeleteAsync(int postId);
    Task<Post?> GetAsync(int postId);
    Task<IEnumerable<int>> GetChildIdsAsync(int parentId);
    Task<KeysetPaginationResult<PostListItemDto>> KeysetPaginateAsync(List<string> tags, List<PostRating> ratings,
        List<PostStatus> statuses, PostSortColumn orderColumn, KeysetPaginationDirection direction, int pageSize, int? referenceId);
}
