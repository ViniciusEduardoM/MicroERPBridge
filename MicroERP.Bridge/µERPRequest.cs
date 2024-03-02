using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace MicroERP.Bridge
{
    public class µERPRequest
    {
        private readonly µERPConnection _erpConnection;

        internal IFlurlRequest FlurlRequest { get; }

        internal µERPRequest(µERPConnection connection, IFlurlRequest flurlRequest)
        {
            _erpConnection = connection;
            this.FlurlRequest = flurlRequest;
        }
        
        public async Task<T> GetAsync<T>()
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                string stringResult = await FlurlRequest.GetStringAsync();
                var jObject = JObject.Parse(stringResult);

                
                    return jObject.ToObject<T>();
                
            });
        }
        
        public async Task<dynamic> GetAsync()
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.GetJsonAsync();
            });
        }
        
        public async Task<string> GetStringAsync()
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.GetStringAsync();
            });
        }
        
        public async Task<T> GetAnonymousTypeAsync<T>(T anonymousTypeObject, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                string stringResult = await FlurlRequest.GetStringAsync();
                return JsonConvert.DeserializeAnonymousType(stringResult, anonymousTypeObject, jsonSerializerSettings);
            });
        }
        
        public async Task<byte[]> GetBytesAsync()
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.GetBytesAsync();
            });
        }
        
        public async Task<Stream> GetStreamAsync()
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.GetStreamAsync();
            });
        }

        public async Task<T> PostAsync<T>(object data)
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                string stringResult = await FlurlRequest.PostJsonAsync(data).ReceiveString();
                var jObject = JObject.Parse(stringResult);

                return jObject.ToObject<T>();
            });
        }

        public async Task<T> PostStringAsync<T>(string data)
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                string stringResult = await FlurlRequest.PostStringAsync(data).ReceiveString();
                var jObject = JObject.Parse(stringResult);

                return jObject.ToObject<T>();

            });
        }

        public async Task<T> PostAsync<T>()
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                string stringResult = await FlurlRequest.PostAsync().ReceiveString();
                var jObject = JObject.Parse(stringResult);

                return jObject.ToObject<T>();

            });
        }

        public async Task PostAsync(object data)
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PostJsonAsync(data);
            });
        }

        public async Task<string> PostReceiveStringAsync()
        {
            return await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PostAsync().ReceiveString();
            });
        }

        public async Task PostStringAsync(string data)
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PostStringAsync(data);
            });
        }

        public async Task PostAsync()
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PostAsync();
            });
        }

        public async Task PatchAsync(object data)
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PatchJsonAsync(data);
            });
        }
        
        public async Task PatchStringAsync(string data)
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PatchStringAsync(data);
            });
        }

        public async Task PutAsync(object data)
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PutJsonAsync(data);
            });
        }

        public async Task PutStringAsync(string data)
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.PutStringAsync(data);
            });
        }

        public async Task DeleteAsync()
        {
            await _erpConnection.ExecuteRequest(async () =>
            {
                return await FlurlRequest.DeleteAsync();
            });
        }
    }
}
