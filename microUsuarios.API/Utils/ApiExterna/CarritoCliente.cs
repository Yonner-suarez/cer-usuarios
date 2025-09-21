using microUsuarios.API.Model.Response;
using System.Net.Http.Headers;
using System.Text.Json;

namespace microUsuarios.API.Utils.ApiExterna
{
    public static class CarritoCliente
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Obtiene información del carrito de un cliente desde la API de compras
        /// </summary>
        /// <param name="token">JWT Bearer Token</param>
        /// <returns>Objeto Carrito mapeado desde el JSON</returns>
        public static async Task<CarritoResponse> ObtenerCarritoAsync(string token)
        {
            var baseUrl = Variables.APICOMPRAS.url;
            var requestUrl = $"{baseUrl}/api/v1/Carrito";

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // Deserializar a objeto Carrito
                var carrito = JsonSerializer.Deserialize<CarritoResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return carrito!;
            }
        }
    }
}
