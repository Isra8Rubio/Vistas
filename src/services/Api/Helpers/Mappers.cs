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
                DateModified = group.DateModified,
                ListaUsuarios = (group.Owners ?? new List<User>())
                    .Select(u => new UserDTO { Id = u.Id, Name = u.Name, Email = u.Email })
                    .ToList()
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
                ListaGrupos = (user.Groups ?? new List<Group>())
                    .Select(g => new GroupDTO { Id = g.Id, Name = g.Name })
                    .ToList(),
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

        // ===== Queues =====
        public static QueueDTO FromRaw(Queue q)
        {
            return new QueueDTO
            {
                Id = q.Id,
                Name = q.Name,
                MemberCount = q.MemberCount,
                DateModified = q.DateModified,
                Division = q.Division == null ? null : new DivisionSummaryDTO
                {
                    Id = q.Division.Id,
                    Name = q.Division.Name
                }
            };
        }

        public static List<QueueDTO> ToQueueList(List<Queue> queues)
            => queues.Select(FromRaw).ToList();

        public static PagedResultDTO<QueueDTO> FromRaw(QueueEntityListing listing)
        {
            return new PagedResultDTO<QueueDTO>
            {
                Entities = ToQueueList(listing.Entities ?? new List<Queue>()),
                Total = listing.Total,
                PageCount = listing.PageCount,
                PageNumber = listing.PageNumber,
                PageSize = listing.PageSize,
                FirstUri = listing.FirstUri,
                LastUri = listing.LastUri,
                SelfUri = listing.SelfUri,
                NextUri = listing.NextUri,
                PreviousUri = listing.PreviousUri
            };
        }

        // ===== Divisions =====
        public static DivisionDTO FromRaw(AuthzDivision d)
        {
            return new DivisionDTO
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                HomeDivision = d.HomeDivision,
                ObjectCounts = d.ObjectCounts?.ToDictionary(kv => kv.Key, kv => (long)(kv.Value ?? 0))
            };
        }

        public static List<DivisionDTO> ToDivisionList(List<AuthzDivision> list)
            => (list ?? new List<AuthzDivision>()).Select(FromRaw).ToList();

        public static PagedResultDTO<DivisionDTO> FromRaw(AuthzDivisionEntityListing listing)
        {
            return new PagedResultDTO<DivisionDTO>
            {
                Entities = ToDivisionList(listing.Entities ?? new List<AuthzDivision>()),
                Total = listing.Total,
                PageCount = listing.PageCount,
                PageNumber = listing.PageNumber,
                PageSize = listing.PageSize,
                FirstUri = listing.FirstUri,
                NextUri = listing.NextUri,
                PreviousUri = listing.PreviousUri,
                LastUri = listing.LastUri,
                SelfUri = listing.SelfUri
            };
        }
    }
}
