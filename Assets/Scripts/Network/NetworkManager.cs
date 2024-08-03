using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Eidolon.Network
{
    public class NetworkManager : MonoBehaviour
    {
        [field: SerializeField]
        public string ServerUrl { get; private set; }

        private readonly HttpClient client = new HttpClient();

        public async Task<HttpStatusCode> GetAsync<T>(string endpoint)
        {
            try
            {
                Debug.Log($"[GET]: {ServerUrl}/{endpoint}");
                HttpResponseMessage response = await client.GetAsync($"{ServerUrl}/{endpoint}");

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
                return HttpStatusCode.NotFound;
            }
        }

        public async Task<HttpStatusCode> PostAsync<T>(string endpoint, Payload<T> payload)
        {
            try
            {
                string json = payload.GetJson();
                HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                Debug.Log($"[POST]: {ServerUrl}/{endpoint}\nPayload:\n{json}");
                HttpResponseMessage response = await client.PostAsync($"{ServerUrl}/{endpoint}", content);

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
                return HttpStatusCode.NotFound;
            }
        }
    }
}