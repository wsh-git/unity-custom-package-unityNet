using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Wsh.Net {
    
    public class UnityWebResponse : IDisposable {
        
        public bool IsSuccess {
            get {
                return m_result == UnityWebRequest.Result.Success;
            }
        }
        public UnityWebRequest.Result Result => m_result;
        public string Message => m_message;
        public Texture Texture => m_texture;
        public AssetBundle AssetBundle => m_assetBundle;
        public AudioClip AudioClip => m_audioClip;
        
        private UnityWebRequest.Result m_result;
        private string m_message;
        private Texture m_texture;
        private AssetBundle m_assetBundle;
        private AudioClip m_audioClip;

        public void SetInfo(UnityWebRequest.Result result, string message) {
            m_result = result;
            m_message = message;
        }

        public void SetTexture(Texture texture) {
            m_texture = texture;
        }

        public void SetAssetBundle(AssetBundle assetBundle) {
            m_assetBundle = assetBundle;
        }

        public void SetAudioClip(AudioClip audioClip) {
            m_audioClip = audioClip;
        }
        
        public void Dispose() {
            m_result = 0;
            m_texture = null;
            m_message = null;
            m_assetBundle = null;
            m_audioClip = null;
        }
        
    }
    
}