using System;
using System.Collections.Generic;
using Mrut;
using Mrut.Converter;
using UnityEngine;

public class Test : MonoBehaviour{
    private MrutRest rest;

    public void GetUser(Action<User> completeCallback, Action<string> errorCallback){
        RequestGet<User>(response => {
            if (response.IsSuccess) {
                completeCallback(response.Data);
            } else {
                errorCallback(response.ErrorBody.TextData);
            }
        }, exception => errorCallback(exception.Message), "/path-to-get-user");
    }

    public void Register(string name, string email, Action<User> completeCallback, Action<string> errorCallback) {
        User user = new User {Name = name, Email = email};
        RequestPost<User>(response => {
            if (response.IsSuccess) {
                completeCallback(response.Data);
            } else {
                errorCallback(response.ErrorBody.TextData);
            }
        }, exception => errorCallback(exception.Message), "/path-to-register", user);
    }

    private void RequestGet<T>(MrutRest.RequestSuccessDelegate<T> success,
        MrutRest.RequestErrorDelegate fail,
        string path,
        Dictionary<string, object> queryParams = null,
        Dictionary<string, string> headers = null
    ) {
        rest.RequestGet<T>(response => { success(response); }, exception => { fail(exception); }, path, queryParams,
            headers);
    }

    private void RequestPost<T>(MrutRest.RequestSuccessDelegate<T> success,
        MrutRest.RequestErrorDelegate fail,
        string path,
        object body = null,
        Dictionary<string, object> queryParams = null,
        Dictionary<string, string> headers = null
    ) {
        rest.RequestPost<T>(
            response => { success(response); },
            exception => { fail(exception); },
            path, body, queryParams, headers);
    }

    private void RequestPut<T>(MrutRest.RequestSuccessDelegate<T> success,
        MrutRest.RequestErrorDelegate fail,
        string path,
        object body = null,
        Dictionary<string, object> queryParams = null,
        Dictionary<string, string> headers = null
    ) {
        rest.RequestPut<T>(
            response => { success(response); },
            exception => { fail(exception); },
            path, body, queryParams, headers);
    }

    private void AddGlobalHeader(string key, string value) {
        rest.AddGlobalHeader(key, value);
    }

    private void RemoveGlobalHeader(string key) {
        rest.RemoveGlobalHeader(key);
    }

    // Use this for initialization
    private void Start() {
        rest = new MrutRest("your-base-url", new NewtonsoftJsonConverter()); // place your base url
        AddGlobalHeader("Content-Type", "application/json");
    }
}