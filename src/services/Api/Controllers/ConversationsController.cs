using Api.Service;
using Microsoft.AspNetCore.Mvc;
using PureCloudPlatform.Client.V2.Model;
using System.Globalization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly ILogger<ConversationsController> logger;
        private readonly IHttpContextAccessor httpContext;
        private readonly CallService callService;

        public ConversationsController(ILogger<ConversationsController> logger, IHttpContextAccessor httpContext, CallService callService)
        {
            this.logger = logger;
            this.httpContext = httpContext;
            this.callService = callService;
        }

        /// <summary>
        /// Crea un job de Analytics Conversations Details para el día indicado (from -> [yyyy-MM-dd]).
        /// El intervalo se fija como [from 00:00:00, from+1d 00:00:00).
        /// </summary>
        /// <param name="from">Fecha base (formato recomendado: yyyy-MM-dd). Ej: 2025-10-08</param>
        [HttpPost("jobs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> CreateJobAsync(string from)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                logger.LogInformation("[{TraceId}] Call: CreateJobAsync(from={From})", traceId, from);

                if (string.IsNullOrWhiteSpace(from))
                    return BadRequest(new { Message = "Parámetro 'from' requerido. Formato recomendado: yyyy-MM-dd" });

                // Intentamos parsear yyyy-MM-dd primero, luego fallback a DateTimeOffset.TryParse
                DateTimeOffset start;
                if (DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                           DateTimeStyles.AssumeLocal, out var dateOnly))
                {
                    start = new DateTimeOffset(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0, TimeSpan.Zero)
                                .ToLocalTime(); // por si quieres asegurar local
                }
                else if (!DateTimeOffset.TryParse(from, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out start))
                {
                    return BadRequest(new
                    {
                        Message = "No se pudo interpretar la fecha 'from'. Usa 'yyyy-MM-dd' (ej: 2025-10-08) o formatos estándar."
                    });
                }

                // Fijamos el intervalo en el servicio y pedimos el job
                callService.Conversations_SetIntervalExtract(start);
                var jobId = await callService.Conversations_RequestJobAsync();

                if (string.IsNullOrWhiteSpace(jobId))
                    return StatusCode(502, new { Message = "No se pudo crear el job en Genesys (sin JobId)." });

                logger.LogInformation("[{TraceId}] FinishCall: CreateJobAsync -> JobId={JobId}", traceId, jobId);
                return Ok(new { JobId = jobId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] CreateJobAsync error", traceId);
                return StatusCode(500, new { Message = "Error creando job de conversaciones", Detail = ex.Message });
            }
        }

        /// <summary>
        /// Devuelve el estado de un job de Analytics Conversations Details.
        /// </summary>
        [HttpGet("jobs/{jobId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AsyncQueryStatus))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AsyncQueryStatus>> GetJobStatusAsync(string jobId)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                logger.LogInformation("[{TraceId}] Call: GetJobStatusAsync(jobId={JobId})", traceId, jobId);

                var status = await callService.Conversations_GetJobStatusAsync(jobId);
                if (status is null) return NotFound();

                logger.LogInformation("[{TraceId}] FinishCall: GetJobStatusAsync -> State={State}", traceId, status.State);
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetJobStatusAsync error", traceId);
                return StatusCode(500, new { Message = "Error obteniendo el estado del job", Detail = ex.Message });
            }
        }

        /// <summary>
        /// Descarga todos los resultados de un job (itera internamente por cursor).
        /// </summary>
        [HttpGet("jobs/{jobId}/results")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AnalyticsConversation>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AnalyticsConversation>>> GetJobResultsAsync(string jobId)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                logger.LogInformation("[{TraceId}] Call: GetJobResultsAsync(jobId={JobId})", traceId, jobId);

                var items = await callService.Conversations_GetJobResultsAsync(jobId);
                if (items is null) return NotFound();

                logger.LogInformation("[{TraceId}] FinishCall: GetJobResultsAsync -> Count={Count}", traceId, items.Count);
                return Ok(items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetJobResultsAsync error", traceId);
                return StatusCode(500, new { Message = "Error obteniendo resultados del job", Detail = ex.Message });
            }
        }
    }
}