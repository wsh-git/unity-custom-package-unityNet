using UnityEngine;
using UnityEngine.UI;

namespace Wsh.Net {
    
    public class UnityWebRequestDemo : MonoBehaviour {

        private const string HTTP_SERVER_URL = "http://192.168.1.149:8080/UploadTest/";
        private const string LOCAL_FILE_URL = "D:/Projects/yiyiyaya/DownloadTest/localUploadTest.png";
        private const string HTTP_SERVER_AUDIOCLIP_URL = "http://192.168.1.149:8080/UploadTest/soundTest.mp3";

        [SerializeField] private RawImage m_rawImage; 
        [SerializeField] private AudioSource m_audioSource;
        
        private void OnGUI() {
            if(GUILayout.Button("Upload File", GUILayout.Width(200), GUILayout.Height(80))) {
                UnityWebRequestManager.Instance.UploadFile(HTTP_SERVER_URL, "unitywebrequestUploadTest.png", LOCAL_FILE_URL, (uploadProgress, uploadBytes) => {
                        Log.Info("upload progress", uploadProgress, uploadBytes);
                    }, res => {
                        Log.Info(res.IsSuccess, res.Result, res.Message);
                    });
            }

            if(GUILayout.Button("Download File", GUILayout.Width(200), GUILayout.Height(80))) {
                UnityWebRequestManager.Instance.DownloadFile(HTTP_SERVER_URL + "onlineTest.mp4", "D:/Projects/yiyiyaya/DownloadTest/downloadOnlineTest.mp4", (progress, size) => {
                    Log.Info("download progress", progress, size);
                }, res => {
                    Log.Info(res.IsSuccess, res.Result, res.Message);
                });
            }

            if(GUILayout.Button("Download Texture", GUILayout.Width(200), GUILayout.Height(80))) {
                UnityWebRequestManager.Instance.DownloadTexture(HTTP_SERVER_URL + "uploadTest111.png",
                    (progress, size) => {
                        Log.Info("download progress", progress, size);
                    }, res => {
                        Log.Info(res.IsSuccess, res.Result, res.Message);
                        if(res.IsSuccess) {
                            m_rawImage.texture = res.Texture;
                        }
                    });
            }

            if(GUILayout.Button("Download AudioClip", GUILayout.Width(200), GUILayout.Height(80))) {
                UnityWebRequestManager.Instance.DownloadAudioClip(HTTP_SERVER_AUDIOCLIP_URL, (progress, size) => {
                    Log.Info("download progress", progress, size);
                }, res => {
                    Log.Info(res.IsSuccess, res.Result, res.Message);
                    if(res.IsSuccess) {
                        m_audioSource.clip = res.AudioClip;
                        m_audioSource.Play();
                    }
                });
            }

            if(GUILayout.Button("Load Text", GUILayout.Width(200), GUILayout.Height(80))) {
                UnityWebRequestManager.Instance.RequestText(HTTP_SERVER_URL + "hfs.comments.txt", (progress, size) => {
                    Log.Info("download progress", progress, size);
                }, res => {
                    Log.Info(res.IsSuccess, res.Result, res.Message, res.Text);
                });
            }

        }
    }
    
}