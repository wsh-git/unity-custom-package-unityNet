using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wsh.Net {
    public class FTP {

        private static FtpWebRequest CreateFtpWebRequest(string account, string password, string remoteFileUrl, string method, out FtpWebResponse res) {
            try {
                Uri uri = new Uri(remoteFileUrl);
                FtpWebRequest req = FtpWebRequest.Create(uri) as FtpWebRequest;
                NetworkCredential networkCredential = new NetworkCredential(account, password);
                req.Credentials = networkCredential;
                req.Proxy = null;
                req.Method = method;
                req.UseBinary = true;
                req.KeepAlive = false;
                if(method == WebRequestMethods.Ftp.UploadFile){
                    res = null;
                } else {
                    res = req.GetResponse() as FtpWebResponse;
                }
                return req;
            } catch(Exception e) {
                throw;
            }
        }

        public static async void Upload(string account, string password, string remoteFileUrl, string localFileUrl, Action<FTPResponse> onFinish) {
            FTPResponse res = new FTPResponse();
            await Task.Run(() => { StartUpload(account, password, remoteFileUrl, localFileUrl, ref res); });
            onFinish?.Invoke(res);
        }

        private static void StartUpload(string account, string password, string remoteFileUrl, string localFileUrl, ref FTPResponse resFtp) {
            try {
                FtpWebResponse res;
                FtpWebRequest req = CreateFtpWebRequest(account, password, remoteFileUrl, WebRequestMethods.Ftp.UploadFile, out res);
                Stream stream = req.GetRequestStream();
                using(FileStream fileStream = File.OpenRead(localFileUrl)) {
                    byte[] bytes = new byte[1024];
                    int readLength = fileStream.Read(bytes, 0, bytes.Length);
                    while(readLength != 0) {
                        stream.Write(bytes, 0, readLength);
                        readLength = fileStream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    stream.Close();
                }
                resFtp.SetInfo(true, "upload success.");
            } catch(Exception e) {
                resFtp.SetInfo(false, e.Message);
            }
        }

        public static async void Download(string account, string password, string remoteFileUrl, string localFileUrl, Action<FTPResponse> onFinish) {
            FTPResponse res = new FTPResponse();
            await Task.Run(() => { StartDownload(account, password, remoteFileUrl, localFileUrl, ref res); });
            onFinish?.Invoke(res);
        }

        private static void StartDownload(string account, string password, string remoteFileUrl, string localFileUrl, ref FTPResponse resFtp) {
            try {
                FtpWebResponse res;
                FtpWebRequest req = CreateFtpWebRequest(account, password, remoteFileUrl, WebRequestMethods.Ftp.DownloadFile, out res);
                Stream stream = res.GetResponseStream();
                using(FileStream fileStream = File.Create(localFileUrl)) {
                    byte[] bytes = new byte[1024];
                    int readLength = stream.Read(bytes, 0, bytes.Length);
                    while(readLength != 0) {
                        fileStream.Write(bytes, 0, readLength);
                        readLength = stream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    stream.Close();
                    resFtp.SetInfo(true, "download success.");
                }
                res.Close();
            } catch(Exception e) {
                resFtp.SetInfo(false, e.Message);
            }
        }

        public static async void DeleteFile(string account, string password, string remoteFileUrl, Action<FTPResponse> onFinish) {
            FTPResponse res = new FTPResponse();
            await Task.Run(() => { StartDeleteFile(account, password, remoteFileUrl, ref res);});
            onFinish?.Invoke(res);
        }

        private static void StartDeleteFile(string account, string password, string remoteFileUrl, ref FTPResponse resFtp) {
            try {
                FtpWebResponse res;
                FtpWebRequest req = CreateFtpWebRequest(account, password, remoteFileUrl, WebRequestMethods.Ftp.DeleteFile, out res);
                res.Close();
                resFtp.SetInfo(true, "delete success.");
            } catch (Exception e) {
                resFtp.SetInfo(false, e.Message);
            }
        }

        public static async void GetFileSize(string account, string password, string remoteFileUrl, Action<FTPResponse> onFinish) {
            FTPResponse res = new FTPResponse();
            await Task.Run(() => { StartGetFileSize(account, password, remoteFileUrl, ref res); });
            onFinish?.Invoke(res);
        }

        private static void StartGetFileSize(string account, string password, string remoteFileUrl, ref FTPResponse resFtp) {
            try {
                FtpWebResponse res;
                FtpWebRequest req = CreateFtpWebRequest(account, password, remoteFileUrl, WebRequestMethods.Ftp.GetFileSize, out res);
                resFtp.SetInfo(true, res.ContentLength, "get file size success.");
                res.Close();
            } catch (Exception e) {
                resFtp.SetInfo(false, 0, e.Message);
            }
        }

        public static async void MakeDirectory(string account, string password, string remoteDirectoryUrl, Action<FTPResponse> onFinish) {
            FTPResponse res = new FTPResponse();
            await Task.Run(() => { StartMakeDirectory(account, password, remoteDirectoryUrl, ref res);});
            onFinish?.Invoke(res);
        }

        private static void StartMakeDirectory(string account, string password, string remoteDirectoryUrl, ref FTPResponse resFtp) {
            try {
                FtpWebResponse res;
                FtpWebRequest req = CreateFtpWebRequest(account, password, remoteDirectoryUrl, WebRequestMethods.Ftp.MakeDirectory, out res);
                resFtp.SetInfo(true, "create directory success.");
                res.Close();
            } catch (Exception e) {
                resFtp.SetInfo(false, e.Message);
            }
        }

        public static async void GetFileList(string account, string password, string remoteDirectoryUrl, Action<FTPResponse> onFinish) {
            FTPResponse res = new FTPResponse();
            await Task.Run(() => { StartGetFileList(account, password, remoteDirectoryUrl, ref res);});
            onFinish?.Invoke(res);
        }

        private static void StartGetFileList(string account, string password, string remoteDirectoryUrl, ref FTPResponse resFtp) {
            try {
                FtpWebResponse res;
                FtpWebRequest req = CreateFtpWebRequest(account, password, remoteDirectoryUrl, WebRequestMethods.Ftp.ListDirectory, out res);
                Stream stream = res.GetResponseStream();
                using(StreamReader streamReader = new StreamReader(stream)) {
                    List<string> list = new List<string>();
                    string fileName = streamReader.ReadLine();
                    while(!string.IsNullOrEmpty(fileName)) {
                        list.Add(fileName);
                        fileName = streamReader.ReadLine();
                    }
                    resFtp.SetInfo(true, list.ToArray(), "get file list success.");
                    streamReader.Close();
                    stream.Close();
                    res.Close();
                }
            } catch (Exception e) {
                resFtp.SetInfo(false, null, e.Message);
            }
        }

        public static async void IsExistDirectory(string account, string password, string remoteDirectoryUrl, string directoryName, Action<FTPResponse> onFinish) {
            FTPResponse res = new FTPResponse();
            await Task.Run(() => { StartIsExistDirectory(account, password, remoteDirectoryUrl, directoryName, ref res); });
            onFinish?.Invoke(res);
        }

        // 此方法有待详细的验证，不是很精确
        private static void StartIsExistDirectory(string account, string password, string remoteDirectoryUrl, string directoryName, ref FTPResponse resFtp) {
            try {
                FtpWebResponse res;
                FtpWebRequest req = CreateFtpWebRequest(account, password, remoteDirectoryUrl, WebRequestMethods.Ftp.ListDirectoryDetails, out res);
                Stream stream = res.GetResponseStream();
                using(StreamReader streamReader = new StreamReader(stream)) {
                    bool isExist = false;
                    string directoryDetails = streamReader.ReadToEnd();
                    Console.WriteLine(directoryDetails);
                    List<string> directoryList = new List<string>();
                    // 解析目录信息
                    string[] lines = directoryDetails.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines) {
                        // 根据FTP服务器的不同，可能需要根据实际格式解析出目录名
                        // 这里简单示例，假设目录名在每行的末尾，且行以 "d" 开头表示目录

                        if(line.StartsWith("d")) {
                            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string directoryNames = parts[parts.Length - 1];
                            // 添加到目录列表
                            directoryList.Add(directoryNames);
                        }
                    }
                    isExist = directoryList.Contains(directoryName);
                    resFtp.SetInfo(true, isExist, "print working directory success.");
                    streamReader.Close();
                    stream.Close();
                    res.Close();
                }
            } catch(Exception e) {
                resFtp.SetInfo(false, false, e.Message);
            }
        }

    }
}
