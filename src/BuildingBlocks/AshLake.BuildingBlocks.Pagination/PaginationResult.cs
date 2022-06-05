﻿namespace AshLake.BuildingBlocks.Pagination;

/// <summary>
/// Represents the result of the keyset pagination.
/// </summary>
/// <typeparam name="T">The type of the data list item.</typeparam>
/// <param name="Data">The data list.</param>
/// <param name="TotalCount">The total count of the data.</param>
/// <param name="PageSize">The size of the page. This can be different from the actual size of <see cref="Data"/>.</param>
/// <param name="HasPrevious">Whether there's previous data to the list.</param>
/// <param name="HasNext">Whether there's next data to the list.</param>
public record KeysetPaginationResult<T>(
	IReadOnlyList<T> Data,
	int TotalCount,
	int PageSize,
	bool HasPrevious,
	bool HasNext);

/// <summary>
/// Represents the result of the offset pagination.
/// </summary>
/// <typeparam name="T">The type of the data list item.</typeparam>
/// <param name="Data">The data list.</param>
/// <param name="TotalCount">The total count of the data.</param>
/// <param name="PageSize">The size of the page. This can be different from the actual size of <see cref="Data"/>.</param>
/// <param name="Page">The page number of this result.</param>
public record OffsetPaginationResult<T>(
	IReadOnlyList<T> Data,
	int TotalCount,
	int PageSize,
	int Page);
