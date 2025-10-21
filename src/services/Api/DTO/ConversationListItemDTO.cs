namespace Api.DTO
{
    public class ConversationListItemDTO
    {
        public string? ConversationId { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public string? Direction { get; set; }
        public string? RemoteDisplay { get; set; }
        public string? QueueId { get; set; }
        public int? DurationConnectedMs { get; set; }
    }
}
