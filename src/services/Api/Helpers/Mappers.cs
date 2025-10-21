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
        public static DivisionDTO FromRaw(AuthzDivision authzDivision)
        {
            return new DivisionDTO
            {
                Id = authzDivision.Id,
                Name = authzDivision.Name,
                Description = authzDivision.Description,
                HomeDivision = authzDivision.HomeDivision,
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

        // ===== Conversations (Analytics) =====

        /// <summary>
        /// Convertir en un item plano para listado.
        /// </summary>
        public static ConversationListItemDTO FromRaw(AnalyticsConversation conversation)
        {
            // 1) Elegimos una sesión de VOZ
            var voiceSession = conversation.Participants?
                .SelectMany(p => p?.Sessions ?? Enumerable.Empty<AnalyticsSession>())
                .FirstOrDefault(s => IsVoice(s));

            // 2) Dirección
            var directionText =
                voiceSession?.Direction?.ToString()?.ToLowerInvariant()
                ?? conversation.OriginatingDirection?.ToString()?.ToLowerInvariant()
                ?? "unknown";

            // 3) Texto para mostrar quién es el remoto (cliente/ANI/DNIS)
            var remoteDisplayText =
                voiceSession?.RemoteNameDisplayable
                ?? voiceSession?.Remote
                ?? voiceSession?.Ani
                ?? voiceSession?.Dnis;

            // 4) Primera queueId que aparezca en los segmentos
            var firstQueueId =
                voiceSession?.Segments?
                .Select(seg => seg?.QueueId)
                .FirstOrDefault(id => !string.IsNullOrWhiteSpace(id));

            // 5) tConnected (ms) desde las métricas de la sesión
            int? tConnectedMs = (int?)voiceSession?.Metrics?
                .FirstOrDefault(m => string.Equals(m?.Name, "tConnected", StringComparison.OrdinalIgnoreCase))
                ?.Value;

            return new ConversationListItemDTO
            {
                ConversationId = conversation.ConversationId,
                Start = conversation.ConversationStart ?? DateTimeOffset.MinValue,
                End = conversation.ConversationEnd,
                Direction = directionText,
                RemoteDisplay = remoteDisplayText,
                QueueId = firstQueueId,
                DurationConnectedMs = tConnectedMs
            };
        }

        /// <summary>
        /// Lista plana desde la lista cruda de conversaciones.
        /// </summary>
        public static List<ConversationListItemDTO> ToConversationList(List<AnalyticsConversation> conversations)
            => (conversations ?? new List<AnalyticsConversation>())
               .Select(FromRaw)
               .ToList();

        /// <summary>
        /// Paginación para devolver PagedResultDTO.
        /// </summary>
        public static PagedResultDTO<ConversationListItemDTO> ToPagedResult(
            List<ConversationListItemDTO> allItems,
            int pageNumber,
            int pageSize,
            string? routeBase = null
        )
        {
            var total = allItems?.Count ?? 0;
            var safePageSize = pageSize <= 0 ? 25 : pageSize;
            var pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)safePageSize));
            var page = pageNumber <= 0 ? 1 : Math.Min(pageNumber, pageCount);

            var items = (allItems ?? new List<ConversationListItemDTO>())
                        .Skip((page - 1) * safePageSize)
                        .Take(safePageSize)
                        .ToList();

            string? MakeUri(int p) => string.IsNullOrWhiteSpace(routeBase)
                ? null
                : $"{routeBase}?pageNumber={p}&pageSize={safePageSize}";

            return new PagedResultDTO<ConversationListItemDTO>
            {
                Entities = items,
                Total = total,
                PageCount = pageCount,
                PageNumber = page,
                PageSize = safePageSize,
                FirstUri = MakeUri(1),
                LastUri = MakeUri(pageCount),
                SelfUri = MakeUri(page),
                NextUri = page < pageCount ? MakeUri(page + 1) : null,
                PreviousUri = page > 1 ? MakeUri(page - 1) : null
            };
        }

        /// <summary>
        /// Conversaciones crudas a PagedResultDTO.
        /// </summary>
        public static PagedResultDTO<ConversationListItemDTO> FromRaw(
            List<AnalyticsConversation> conversations,
            int pageNumber,
            int pageSize,
            string? routeBase = null)
        {
            var list = ToConversationList(conversations);
            return ToPagedResult(list, pageNumber, pageSize, routeBase);
        }

        // -------- Helpers internos --------
        private static bool IsVoice(AnalyticsSession? session)
        {
            var mediaType = session?.MediaType?.ToString();
            return string.Equals(mediaType, "voice", StringComparison.OrdinalIgnoreCase);
        }
    }
}
