using Api.DTO;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Helpers
{
    public static class Mappers
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

    }
}
