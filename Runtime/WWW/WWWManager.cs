using System;
using System.Collections;
using Wsh.Singleton;
using UnityEngine;

namespace Wsh.Net {
    public class WWWManager : Singleton<WWWManager> {

        public void DownloadAssetBundle(string url, Action<float> onProgress, Action<bool, string, AssetBundle> onFinish) {
            Download<AssetBundle>(url, onProgress, onFinish);
        }
        
        public void DownloadTexture(string url, Action<float> onProgress, Action<bool, string, Texture> onFinish) {
            Download<Texture>(url, onProgress, onFinish);
        }
        
        public void DownloadAudioClip(string url, Action<float> onProgress, Action<bool, string, AudioClip> onFinish) {
            Download<AudioClip>(url, onProgress, onFinish);
        }
        
        public void DownloadText(string url, Action<float> onProgress, Action<bool, string, string> onFinish) {
            Download<string>(url, onProgress, onFinish);
        }
        
        public void DownloadBytes(string url, Action<float> onProgress, Action<bool, string, byte[]> onFinish) {
            Download<byte[]>(url, onProgress, onFinish);
        }
        
        public void Download<T>(string url, Action<float> onProgress, Action<bool, string, T> onFinish) where T : class {
            Request(url, null, onProgress, onFinish);
        }
        
        private void ConvertTypeToObj<T>(Type t, WWW www, out T obj) where T : class {
            if(t == typeof(AssetBundle)) {
                obj = www.assetBundle as T;
            } else if(t == typeof(AudioClip)) {
                obj = www.GetAudioClip() as T;
            } else if(t == typeof(string)) {
                obj = www.text as T;
            } else if(t == typeof(byte[])) {
                obj = www.bytes as T;
            } else if(t == typeof(Texture)) {
                obj = www.texture as T;
            } else {
                obj = null;
            }
        }
        
        private WWW CreateWWW(string url, WWWForm form) {
            if(form == null) {
                return new WWW(url);
            }
            return new WWW(url, form);
        }
        
        public void Request<T>(string url, WWWForm form, Action<float> onProgress, Action<bool, string, T> onFinish) where T : class {
            StartCoroutine(StartRequest<T>(url, form, onProgress, onFinish));
        }

        private IEnumerator StartRequest<T>(string url, WWWForm form, Action<float> onProgress, Action<bool, string, T> onFinish) where T : class {
            WWW www = CreateWWW(url, form);
            while(!www.isDone) {
                onProgress?.Invoke(www.progress);
                yield return null;
            }
            onProgress?.Invoke(www.progress);
            if(string.IsNullOrEmpty(www.error)) {
                T obj = null;
                Type t = typeof(T);
                ConvertTypeToObj(t, www, out obj);
                if(obj == null) {
                    onFinish?.Invoke(false, "do not contain the type: " + t.ToString(), null);
                } else {
                    onFinish?.Invoke(true, "download success.", obj);
                }
            } else {
                onFinish?.Invoke(false, www.error, null);
            }
        }

    }
    
}