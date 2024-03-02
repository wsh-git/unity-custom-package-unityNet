using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Wsh.Singleton;

namespace Wsh.Net.UnityWebRequests {
    public class UnityWebRequestManager : Singleton<UnityWebRequestManager>, ISingleton {

        public void OnInit() {

        }

        public void RequestText(string remoteUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            StartCoroutine(StartDownload(remoteUrl, null, onProgress, onFinish));
        }
        
        public void DownloadFile(string remoteFileUrl, string localFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            DownloadHandlerFile fileHandler = new DownloadHandlerFile(localFileUrl);
            StartCoroutine(StartDownload(remoteFileUrl, fileHandler, onProgress, onFinish));
        }

        public void DownloadTexture(string remoteFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            DownloadHandlerTexture textureHandler = new DownloadHandlerTexture();
            StartCoroutine(StartDownload(remoteFileUrl, textureHandler, onProgress, res => {
                if(res.IsSuccess) {
                    res.SetTexture(textureHandler.texture);
                }
                onFinish?.Invoke(res);
            }));
        }

        public void DownloadAssetBundle(string remoteFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            DownloadHandlerAssetBundle assetBundleHandler = new DownloadHandlerAssetBundle(remoteFileUrl, 0);
            StartCoroutine(StartDownload(remoteFileUrl, assetBundleHandler, onProgress, res => {
                if(res.IsSuccess) {
                    res.SetAssetBundle(assetBundleHandler.assetBundle);
                }
                onFinish?.Invoke(res);
            }));
        }

        public void DownloadAudioClip(string remoteFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            DownloadHandlerAudioClip audioClipHandler = new DownloadHandlerAudioClip(remoteFileUrl, AudioType.MPEG);
            StartCoroutine(StartDownload(remoteFileUrl, audioClipHandler, onProgress, res => {
                if(res.IsSuccess) {
                    res.SetAudioClip(audioClipHandler.audioClip);
                }
                onFinish?.Invoke(res);
            }));
        }
        
        private UnityWebRequest CreateAndSendWebRequest(string remoteFileUrl, DownloadHandler downloadHandler) {
            UnityWebRequest req = UnityWebRequest.Get(remoteFileUrl);
            if(downloadHandler != null) {
                req.downloadHandler = downloadHandler;
            }
            req.SendWebRequest();
            return req;
        }
        
        private IEnumerator StartDownload(string remoteFileUrl, DownloadHandler downloadHandler, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            using(UnityWebRequest req = CreateAndSendWebRequest(remoteFileUrl, downloadHandler)) {
                UnityWebResponse resUnityWeb = new UnityWebResponse();
                while(!req.isDone) {
                    onProgress?.Invoke(req.downloadProgress, req.downloadedBytes);
                    yield return null;
                }
                onProgress?.Invoke(req.downloadProgress, req.downloadedBytes);
                if(req.result == UnityWebRequest.Result.Success) {
                    string text = "";
                    if(downloadHandler == null) {
                        text = req.downloadHandler.text;
                    }
                    resUnityWeb.SetInfo(req.result, text, "download file success.");
                } else {
                    resUnityWeb.SetInfo(req.result, "","error: " + req.error + "    responseCode: " + req.responseCode);
                }
                onFinish?.Invoke(resUnityWeb);
                resUnityWeb.Dispose();
            }
        }

        public void UploadFile(string remoteUrl, string fileName, string localFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            List<IMultipartFormSection> dataList = new List<IMultipartFormSection>();
            dataList.Add(new MultipartFormFileSection(fileName, File.ReadAllBytes(localFileUrl)));
            StartCoroutine(StartUploadFile(remoteUrl, dataList, onProgress, onFinish));
        }

        private IEnumerator StartUploadFile(string remoteUrl, List<IMultipartFormSection> dataList, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish) {
            UnityWebResponse resUnityWeb = new UnityWebResponse();
            using(UnityWebRequest req = UnityWebRequest.Post(remoteUrl, dataList)) {
                req.SendWebRequest();
                while(!req.isDone) {
                    onProgress?.Invoke(req.uploadProgress, req.uploadedBytes);
                    yield return null;
                }
                onProgress?.Invoke(req.uploadProgress, req.uploadedBytes);
                if(req.result == UnityWebRequest.Result.Success) {
                    resUnityWeb.SetInfo(req.result, "","web request success.");
                } else {
                    resUnityWeb.SetInfo(req.result, "","error: " + req.error + "    responseCode: " + req.responseCode);
                }
                onFinish?.Invoke(resUnityWeb);
                resUnityWeb.Dispose();
            }
        }
        
        public void OnDeinit() {

        }

    }
    
}