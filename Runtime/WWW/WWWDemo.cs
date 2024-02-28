using UnityEngine;
using UnityEngine.UI;

namespace Wsh.Net {
    public class WWWDemo : MonoBehaviour {
        
        [SerializeField] private RawImage m_rawImage;
        
        private void OnGUI() {
            if(GUILayout.Button("Download Texture", GUILayout.Width(200), GUILayout.Height(80))) {
                WWWManager.Instance.DownloadTexture("http://httpserver.wsh.com/file/test.png", progress => {
                    Log.Info(progress);
                }, (isSuccess, message, texture) => {
                    if(isSuccess) {
                        m_rawImage.texture = texture;                        
                    }
                    Log.Info(isSuccess, message);
                });
            }
        }
    }
}