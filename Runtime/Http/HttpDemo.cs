using UnityEngine;

namespace Wsh.Net {
    
    public class HttpDemo : MonoBehaviour {

        private void OnGUI() {
            
            if(GUILayout.Button("Check Exist", GUILayout.Width(200), GUILayout.Height(80))) {
                Http.IsExist("http://httpserver.wsh.com/file/test.png", res => {
                    Log.Info(res.IsSuccess + "    " + res.StatusCode + "    " + res.Message);
                });
            }

            if(GUILayout.Button("Download", GUILayout.Width(200), GUILayout.Height(80))) {
                Http.Download("http://httpserver.wsh.com/file/test.png", "D:/Projects/yiyiyaya/DownloadTest/downloadTest.png", res => {
                    Log.Info(res.IsSuccess + "    " + res.StatusCode + "    " + res.Message);
                });
            }

            if(GUILayout.Button("Upload", GUILayout.Width(200), GUILayout.Height(80))) {
                Http.Upload("wsh", "123123", "http://192.168.1.149:8080/UploadTest/", "uploadTest111.png", "D:/Projects/yiyiyaya/DownloadTest/localUploadTest.png", res => {
                    Log.Info(res.IsSuccess + "    " + res.StatusCode + "    " + res.Message);
                });
            }

        }
        
    }
    
}