using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Service.GenesysAPI
{
    public class ConversationApiService : GenesysAPIService
    {
        private readonly ConversationsApi _conversationsApi = new();
        private readonly GenesysAuthService _genesysAuthService;

        public DateTimeOffset StartDate { get; private set; }
        public DateTimeOffset EndDate { get; private set; }
        public string? IntervalExtract { get; private set; }

        public ConversationApiService(GenesysAuthService authService, ILogger<ConversationApiService> logger) : base(logger)
        {
            _genesysAuthService = authService;
        }

        public void SetIntervalExtract(DateTimeOffset from)
        {
            DateTimeOffset start = from.DateTime;
            DateTimeOffset end = from.DateTime.AddDays(1);

            StartDate = start;
            EndDate = end;

             IntervalExtract = $"{start:yyyy-MM-ddTHH:mm:ss}/{end:yyyy-MM-ddTHH:mm:ss}";
            _logger.LogInformation("Genesys Api interval extraction set: {interval}", IntervalExtract);
        }

        #region Llamadas

        public async Task<string?> Analytics_ConversationsDetails_RequestJobAsync()
        {
            _logger.LogDebug("Analytics_ConversationsDetails_RequestJobAsync --> {interval}", IntervalExtract);
            _genesysAuthService.CheckToken();
            try
            {
                return await ExecuteWithRetry(async () =>
                {
                    var body = new AsyncConversationQuery(Interval: IntervalExtract, StartOfDayIntervalMatching: true, OrderBy: AsyncConversationQuery.OrderByEnum.Conversationstart);
                    var data = await _conversationsApi.PostAnalyticsConversationsDetailsJobsAsync(body);
                    return data?.JobId;
                }, "Analytics_ConversationsDetails_RequestJobAsync");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Analytics_ConversationsDetails_RequestJobAsync error");
                return null;
            }
        }

        public async Task<AsyncQueryStatus?> Analytics_ConversationsDetails_GetJobStatusAsync(string jobId)
        {
            _genesysAuthService.CheckToken();
            try
            {
                return await ExecuteWithRetry(async () => await _conversationsApi.GetAnalyticsConversationsDetailsJobAsync(jobId), "Analytics_ConversationsDetails_GetJobStatusAsync");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Analytics_ConversationsDetails_GetJobStatusAsync error");
                return null;
            }
        }

        public async Task<List<AnalyticsConversation>> Analytics_ConversationsDetails_GetJobResultsAsync(string jobId)
        {
            List<AnalyticsConversation> result = [];
            _genesysAuthService.CheckToken();
            try
            {
                bool isFirstRequest = true;
                string? cursor = null;

                do
                {
                    var data = await ExecuteWithRetry(async () =>
                    {
                        return string.IsNullOrWhiteSpace(cursor) ?
                            await _conversationsApi.GetAnalyticsConversationsDetailsJobResultsAsync(jobId) :
                            await _conversationsApi.GetAnalyticsConversationsDetailsJobResultsAsync(jobId, cursor);
                    }, "Analytics_ConversationsDetails_GetJobResultsAsync");

                    if (data != null)
                    {
                        if (isFirstRequest)
                        {
                            _logger.LogInformation("Data available until '{Date}'", data.DataAvailabilityDate);
                            isFirstRequest = false;
                        }
                        result.AddRange(data.Conversations);
                        cursor = data.Cursor;
                    }
                    else
                        cursor = null;

                } while (cursor != null);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Analytics_ConversationsDetails_GetJobResultsAsync error");
            }

            return result;
        }
        #endregion
    }
}
