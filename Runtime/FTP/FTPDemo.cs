using UnityEngine;

namespace Wsh.Net.Ftps {
    
    public class FTPDemo : MonoBehaviour {
        
        private void OnGUI() {
            if(GUILayout.Button("Upload", GUILayout.Width(200), GUILayout.Height(80))) {
                Log.Info("start upload");
                FTP.Upload("wsh", "123", "ftp://127.0.0.1/uploadTest.txt", "D:/Projects/yiyiyaya/Study/FtpDownload/uploadTest.txt", res => {
                    Log.Info("upload finish: " + res.IsSuccess + "    " + res.Message);
                });
                Log.Info("command execute finish");
            }

            if(GUILayout.Button("Upload", GUILayout.Width(200), GUILayout.Height(80))) {
                Log.Info("start download");
                FTP.Download("wsh", "123", "ftp://127.0.0.1/Snipaste123.png", "D:/Projects/yiyiyaya/Study/FtpDownload/123.png", res => {
                    Log.Info("download finish: " + res.IsSuccess + "    " + res.Message);
                });
                Log.Info("command execute finish");
            }

            if(GUILayout.Button("DeleteFile", GUILayout.Width(200), GUILayout.Height(80))) {
                Log.Info("start delete");
                FTP.DeleteFile("wsh", "123", "ftp://127.0.0.1/remove123.txt", res => {
                    Log.Info("delete finish: " + res.IsSuccess + "    " + res.Message);
                });
                Log.Info("command execute finish");
            }

            if(GUILayout.Button("GetFileSize", GUILayout.Width(200), GUILayout.Height(80))) {
                Log.Info("start getFileSize");
                FTP.GetFileSize("wsh", "123", "ftp://127.0.0.1/Snipaste123.png", res => {
                    Log.Info("get file size finish: " + res.IsSuccess + "    length:" + res.Size.ToString() + "    " + res.Message);
                });
                Log.Info("command execute finish");
            }
            
            if(GUILayout.Button("CreateDirectory", GUILayout.Width(200), GUILayout.Height(80))) {
                Log.Info("start makeDirectory");
                FTP.MakeDirectory("wsh", "123", "ftp://127.0.0.1/Snipaste123", res => {
                    Log.Info("create directory finish: " + res.IsSuccess + "    " + res.Message);
                });
                Log.Info("command execute finish");
            }

            if(GUILayout.Button("GetFileList", GUILayout.Width(200), GUILayout.Height(80))) {
                Log.Info("start get file list");
                FTP.GetFileList("wsh", "123", "ftp://127.0.0.1/", res => {
                    if(res.IsSuccess) {
                        for(int i = 0; i < res.List.Length; i++) {
                            Log.Info(res.List[i]);
                        }
                    }
                    Log.Info("get file list finish: " + res.IsSuccess + "    " + res.Message);
                });
                Log.Info("command execute finish");
            }

            if(GUILayout.Button("IsExistDirectory", GUILayout.Width(200), GUILayout.Height(80))) {
                Log.Info("start check directory");
                FTP.IsExistDirectory("wsh", "123", "ftp://127.0.0.1/", "Snipaste", res => {
                    Log.Info("check directory finish: " + res.IsSuccess + "    " + res.IsExist + "    " + res.Message);
                });
                Log.Info("command execute finish");
            }
            
        }
    }
    
}