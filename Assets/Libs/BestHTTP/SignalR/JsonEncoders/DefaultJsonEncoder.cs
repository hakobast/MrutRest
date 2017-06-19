#if !BESTHTTP_DISABLE_SIGNALR

using BestHTTP.JSON;
using System.Collections.Generic;

namespace BestHTTP.SignalR.JsonEncoders
{
    public sealed class DefaultJsonEncoder : IJsonEncoder
    {
        public string Encode(object obj)
        {
            return BestHTTP.JSON.Json.Encode(obj);
        }

        public IDictionary<string, object> DecodeMessage(string json)
        {
            bool ok = false;
            IDictionary<string, object> result = BestHTTP.JSON.Json.Decode(json, ref ok) as IDictionary<string, object>;
            return ok ? result : null;
        }
    }
}

#endif