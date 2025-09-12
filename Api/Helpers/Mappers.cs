using Api.DTO;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Helpers
{
    public static class Mappers
    {
        // -- Groups --
        public static GroupDTO FromRaw(Group group)
        {
            return new GroupDTO
            {
                Id = group.Id,
                Name = group.Name,
                MemberCount = group.MemberCount,
                DateModified = group.DateModified
            };
        }

        public static List<GroupDTO> ToGroupList(List<Group> groups) => groups.Select(g => FromRaw(g)).ToList();

        public static PagedResultDTO<GroupDTO> FromRaw(GroupEntityListing groupEntityListing)
        {
            return new PagedResultDTO<GroupDTO>
            {
                Entities = ToGroupList(groupEntityListing.Entities),
                Total = groupEntityListing.Total,
                PageCount = groupEntityListing.PageCount,
                PageNumber = groupEntityListing.PageNumber,
                PageSize = groupEntityListing.PageSize,
                FirstUri = groupEntityListing.FirstUri,
                LastUri = groupEntityListing.LastUri,
                SelfUri = groupEntityListing.SelfUri,
                NextUri = groupEntityListing.NextUri,
                PreviousUri = groupEntityListing.PreviousUri,
            };
        }

        // -- Users --
        public static UserDTO FromRaw(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                ListaGrupos = (user.Groups ?? new List<Group>()).Select(g => new GroupDTO { Id = g.Id, Name = g.Name }).ToList(),
            };
        }

        public static List<UserDTO> ToUserList(List<User> users) => users.Select(u => FromRaw(u)).ToList();

        public static PagedResultDTO<UserDTO> FromRaw(UserEntityListing userEntityListing)
        {
            return new PagedResultDTO<UserDTO>
            {
                Entities = ToUserList(userEntityListing.Entities),
                Total = userEntityListing.Total,
                PageCount = userEntityListing.PageCount,
                PageNumber = userEntityListing.PageNumber,
                PageSize = userEntityListing.PageSize,
                FirstUri = userEntityListing.FirstUri,
                LastUri = userEntityListing.LastUri,
                SelfUri = userEntityListing.SelfUri,
                NextUri = userEntityListing.NextUri,
                PreviousUri = userEntityListing.PreviousUri,
            };
        }
    }
}
