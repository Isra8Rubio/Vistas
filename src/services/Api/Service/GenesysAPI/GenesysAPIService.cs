using PureCloudPlatform.Client.V2.Client;

namespace Api.Service.GenesysAPI
{
    public class GenesysAPIService
    {
        protected const int MAX_RETRIES = 5;

        public readonly ILogger<GenesysAPIService> logger;
        public GenesysAPIService(ILogger<GenesysAPIService> _logger)
        {
            logger = _logger;
        }

        protected async Task<T> ExecuteWithRetry<T>(Func<Task<T>> action, string actionName)
        {
            int retryCount = 0; // Inicializa el contador de reintentos a 0.
            int cont = 1;
            while (true) // Bucle infinito para seguir intentando hasta que tenga éxito o se produzca un error no manejado.
            {
                try
                {
                    T result = await action(); // Intenta ejecutar la acción pasada como parámetro.
                    if(result != null) 
                        return result; // Devuelve el resultado de la acción si tiene éxito.
                    else // Si no me devuelve nada la llamada, lo reintento (esto ocurre sobre todo al ir a pedir las grabaciones de una conversacion)
                    {
                        retryCount++; // Incrementa el contador de reintentos.
                        HandleNullResult(retryCount, actionName); // Maneja la excepción de limitación de tasa.
                    }
                }
                catch (ApiException ex) when (ex.ErrorCode == 429) // Captura excepciones de tipo ApiException con código de error 429 (rate limit).
                {
                    retryCount++; // Incrementa el contador de reintentos.
                    HandleRateLimitException(ex, retryCount, actionName); // Maneja la excepción de limitación de tasa.
                }
                catch (Exception ex) // Captura cualquier otra excepción.
                {
                    logger.LogError(ex, "{actionName} ExecuteWithRetry error", actionName); // Registra el error.
                    throw; // Lanza la excepción para que sea manejada por el llamador.
                }
                logger.LogDebug("{action} Entro {cont}", actionName, cont++);

            }
        }

        protected void HandleNullResult(int retryCount, string method)
        {
            if (retryCount > MAX_RETRIES)
            {
                logger.LogError("Max retry attempts exceeded. Aborting.");
                throw new InvalidOperationException("Max retry attempts exceeded.");
            }
            int retryAfterSeconds = 2; // Valor predeterminado en caso de que no se pueda extraer
            logger.LogWarning("[{method}] Rate limit exceeded. Retrying in {RetryAfter} seconds...", method, retryAfterSeconds);
            Thread.Sleep(retryAfterSeconds * 1000); // Esperar el tiempo especificado
        }

        protected void HandleRateLimitException(ApiException ex, int retryCount, string method)
        {
            if (retryCount > MAX_RETRIES)
            {
                logger.LogError("Max retry attempts exceeded. Aborting.");
                throw new InvalidOperationException("Max retry attempts exceeded.", ex);
            }

            int retryAfterSeconds = 60; // Valor predeterminado en caso de que no se pueda extraer
            if (ex.Headers.TryGetValue("Retry-After", out var retryAfterValues) && retryAfterValues.Length != 0)
            {
                if (!int.TryParse(retryAfterValues, out retryAfterSeconds))
                {
                    retryAfterSeconds = 60; // Valor predeterminado en caso de que no se pueda extraer
                }
            }

            logger.LogWarning("[{method}] Rate limit exceeded. Retrying in {RetryAfter} seconds...", method, retryAfterSeconds);
            Thread.Sleep((retryAfterSeconds + 1) * 1000); // Esperar el tiempo especificado + 1 segundo
        }
    }
}
