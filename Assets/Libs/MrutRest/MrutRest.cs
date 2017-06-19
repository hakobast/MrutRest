using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using Mrut.Converter;
using UnityEngine;

namespace Mrut{
    public class MrutRest{
        public delegate void RequestSuccessDelegate<T>(Response<T> response);
        public delegate void RequestErrorDelegate(Exception e);

        private readonly Dictionary<string, string> globalHeaders;
        private AbstractObjectConverter converter;

        public string BaseUrl { get; private set; }

        public MrutRest(string baseUrl, AbstractObjectConverter converter) {
            this.BaseUrl = baseUrl;
            this.converter = converter;
            this.globalHeaders = new Dictionary<string, string>();
        }
        
        public void RequestGet<T>(
            RequestSuccessDelegate<T> success,
            RequestErrorDelegate fail,
            string path,
            Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null
        ) {
            HTTPRequest request = new HTTPRequest(CreateUri(path, queryParams), HTTPMethods.Get,
                delegate(HTTPRequest originalRequest, HTTPResponse httpResponse) {
                    HandleResponse(originalRequest, httpResponse, success, fail);
                }
            );
            
            Debug.Log("CALLING SERVER Uri: " + request.Uri);

            AddHeaders(request, headers);
            request.Send();
        }

        public void RequestPost<T>(
            RequestSuccessDelegate<T> success,
            RequestErrorDelegate fail,
            string path,
            object body = null,
            Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null
        ) {
            HTTPRequest request = new HTTPRequest(CreateUri(path, queryParams), HTTPMethods.Post,
                delegate(HTTPRequest originalRequest, HTTPResponse httpResponse) {
                    HandleResponse(originalRequest, httpResponse, success, fail);
                }
            );
            Debug.Log("CALLING SERVER Uri: " + request.Uri + " Body: " + converter.ToString(body));

            AddHeaders(request, headers);
            if (body != null) {
                request.RawData = converter.ToRawData(body);
            }
            request.Send();
        }

        public void RequestPut<T>(
            RequestSuccessDelegate<T> success,
            RequestErrorDelegate fail,
            string path,
            object body = null,
            Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null
        ) {
            HTTPRequest request = new HTTPRequest(CreateUri(path, queryParams), HTTPMethods.Put,
                delegate(HTTPRequest originalRequest, HTTPResponse httpResponse) {
                    HandleResponse(originalRequest, httpResponse, success, fail);
                }
            );
            if (body != null) {
                Debug.Log("CALLING SERVER Uri: " + request.Uri + " Body: " + converter.ToString(body));
            } else {
                Debug.Log("CALLING SERVER Uri: " + request.Uri);
            }

            AddHeaders(request, headers);
            if (body != null) {
                request.RawData = converter.ToRawData(body);
            }
            request.Send();
        }
        
        public void AddGlobalHeader(string key, string value) {
            if (globalHeaders.ContainsKey(key)) {
                globalHeaders[key] = value;
            } else {
                globalHeaders.Add(key, value);
            }
        }

        public void RemoveGlobalHeader(string key) {
            if (globalHeaders.ContainsKey(key)) {
                globalHeaders.Remove(key);
            }
        }

        public void SetConverter(AbstractObjectConverter converter) {
            this.converter = converter;
        }

        private void HandleResponse<T>(
            HTTPRequest originalRequest,
            HTTPResponse httpResponse,
            RequestSuccessDelegate<T> success,
            RequestErrorDelegate fail
        ) {
            Response<T> response = null;
            try{
                Debug.Log("SERVER RESPONSE Uri: " + originalRequest.Uri + " Response: " + httpResponse.DataAsText);
                response = ProcessResponse<T>(originalRequest, httpResponse);
            }catch(Exception e){
                Debug.Log("INTERNAL ERROR: " + e.Message);
                fail(e);
            }

            if (response != null) {
                success(response);
            }
        }

        private Response<T> ProcessResponse<T>(HTTPRequest request, HTTPResponse httpResponse) {
            Response<T> response;
            if (httpResponse.IsSuccess) {
                if (typeof(T) == typeof(ResponseBody)) {
                    
                    ResponseBody responseBody = new ResponseBody(
                        httpResponse.Data,
                        httpResponse.GetFirstHeaderValue("Content-Type")
                    );

                    response = new Response<T>((T)Convert.ChangeType(responseBody, typeof(T)), httpResponse);
                } else {
                    response = new Response<T>(converter.ToObject<T>(httpResponse.DataAsText), httpResponse);
                }
            } else {
                ResponseBody responseBody = new ResponseBody(
                    httpResponse.Data,
                    httpResponse.GetFirstHeaderValue("Content-Type")
                );
                response = new Response<T>(responseBody, httpResponse);
            }

            return response;
        }

        private Uri CreateUri(string path, Dictionary<string, object> queryParams) {
            StringBuilder query = new StringBuilder();
            if (queryParams != null) {
                foreach (KeyValuePair<string, object> pair in queryParams) {
                    query.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
                }

                if (query.Length > 0) {
                    query.Remove(query.Length - 1, 1);
                }
            }
            return new Uri(BaseUrl + path + (query.Length > 0 ? "?" + query : ""));
        }

        private void AddHeaders(HTTPRequest request, Dictionary<string, string> headers) {
            foreach (KeyValuePair<string, string> pair in globalHeaders) {
                request.AddHeader(pair.Key, pair.Value);
            }

            if (headers == null) {
                return;
            }

            foreach (KeyValuePair<string, string> pair in headers) {
                request.AddHeader(pair.Key, pair.Value);
            }
        }
    }
}