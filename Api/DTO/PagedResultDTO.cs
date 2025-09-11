﻿namespace Api.DTO
{
    public class PagedResultDTO<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
