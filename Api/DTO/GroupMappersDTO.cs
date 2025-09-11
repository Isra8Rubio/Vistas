using PureCloudPlatform.Client.V2.Model;

namespace Api.DTO
{
    public static class GroupMappersDTO
    {
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

        public static List<GroupDTO> ToGroupList(Group[] groups) => groups.Select(g => FromRaw(g)).ToList();
    }
}
