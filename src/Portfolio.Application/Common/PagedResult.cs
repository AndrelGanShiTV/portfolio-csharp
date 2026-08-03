namespace Portfolio.Application.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int CurrentPage { get; init; }

    public int TotalPages { get; init; }

    public int TotalItems { get; init; }
}