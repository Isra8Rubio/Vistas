namespace Api.DTO
{
    public class GroupDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public long? MemberCount { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
