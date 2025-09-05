using System;
using System.Collections.Generic;

namespace Api.DTO
{
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

        public static PagedResult<T> Empty() =>
            new() { TotalItems = 0, Items = Array.Empty<T>() };
    }
}
