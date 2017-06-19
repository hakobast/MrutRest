using System.Collections.Generic;
using BestHTTP;

namespace Mrut{
    public class Response<T> {
        public T Data { get; private set; }
        public ResponseBody ErrorBody { get; private set; }
        public HTTPResponse HttpResponse { get; private set; }

        public bool IsSuccess {
            get { return HttpResponse.IsSuccess; }
        }

        public Dictionary<string, List<string>> Headers {
            get { return HttpResponse.Headers; }
        }

        public int Status {
            get { return HttpResponse.StatusCode; }
        }

        public string Message {
            get { return HttpResponse.Message; }
        }

        public Response(T data, HTTPResponse httpResponse) {
            Data = data;
            HttpResponse = httpResponse;
        }

        public Response(ResponseBody error, HTTPResponse httpResponse) {
            ErrorBody = error;
            HttpResponse = httpResponse;
        }

        public bool HasHeader(string key) {
            return Headers.ContainsKey(key);
        }

        public string GetHeader(string key) {
            if (HasHeader(key) && Headers[key].Count > 0) {
                return Headers[key][0];
            } else {
                return null;
            }
        }
    }
}