namespace Api.DTO
{
    public class QueueDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public long? MemberCount { get; set; }
        public DateTime? DateModified { get; set; }
        public List<UserDTO> ListaUsuarios { get; set; } = new();
    }
}
