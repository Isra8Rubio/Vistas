namespace Api.DTO
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public List<GroupDTO> ListaGrupos { get; set; } = new();
    }
}
