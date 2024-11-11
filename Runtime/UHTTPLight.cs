using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace UHTTPLights
{
    public static class UHTTPLight
    {
        private static UHTTPLightConfig config;
        private static UHTTPLightConfig Config => config ??= Resources.Load<UHTTPLightConfig>(nameof(UHTTPLightConfig));

        private static Action onTokenExpired { get; set; }
        private static void OnTokenExpired(Action action) =>
            onTokenExpired = action;

        private static Action<bool> LoadingAction;
        public static void SetLoading(Action<bool> action) =>
            LoadingAction = action;

        private static string token = null;
        public static void SetToken(string token) =>
            UHTTPLight.token = token;

        public static UnityWebRequest CreateRequest(string appendUrl, string method, string body = null, List<KeyValuePair<string, string>> headers = default)
        {
            UnityWebRequest req = new UnityWebRequest(Config.baseURL + appendUrl, method);

            req.downloadHandler = new DownloadHandlerBuffer();
            if(body != null)
                req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));

            foreach (var header in Config.defaultHeaders)
                req.SetRequestHeader(header.Key, header.Value);

            if(headers != null && headers.Count > 0)
                foreach (var header in headers)
                    req.SetRequestHeader(header.Key, header.Value);

            return req;
        }

        public static void AddToken(this UnityWebRequest request) 
            => request.SetRequestHeader("Authorization",Config.useBearerPrefixAuthHeader ? $"Bearer {token}" : token);

        public static void Send(this UnityWebRequest request, Action onComplete = null,bool addTokenIfExist = true, bool haveLoading = false)
        {
            if(haveLoading)
                LoadingAction?.Invoke(true);

            if(addTokenIfExist)
                request.AddToken();

            request.SendWebRequest().completed += Response;

            void Response(AsyncOperation ao)
            {
                if(haveLoading) LoadingAction?.Invoke(false);

                if(request.responseCode == 401 && addTokenIfExist && token != null && onTokenExpired != null)
                {
                    token = null;
                    onTokenExpired();
                    return;
                }

                onComplete?.Invoke();
            }
        }

        public static IEnumerator Stream(this UnityWebRequest request, Action<UnityWebRequest> streamCallback)
        {
            while(!request.isDone)
            {
                streamCallback?.Invoke(request);
                yield return new WaitForEndOfFrame();
            }
        }

        public static T GetData<T>(this UnityWebRequest request) where T : class
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                return null;
            }

            return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        } 
    }
}