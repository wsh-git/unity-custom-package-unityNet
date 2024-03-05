using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text;

namespace Wsh.Net.Https {
    
    public class Http {

        private const string HTTP_BOUNDARY_DEFINE = "wsh";

        private static HttpWebRequest CreateRequest(string remoteFileUrl, string method, int timeout = 2000) {
            Uri uri = new Uri(remoteFileUrl);
            HttpWebRequest req = HttpWebRequest.Create(uri) as HttpWebRequest;
            req.Timeout = timeout;
            req.Method = method;
            return req;
        }

        public static async void IsExist(string remoteFileUrl, Action<HttpResponse> onFinish) {
            HttpResponse res = new HttpResponse();
            await Task.Run(() => { StartCheckExist(remoteFileUrl, ref res);});
            onFinish?.Invoke(res);
        }

        private static void StartCheckExist(string remoteFileUrl, ref HttpResponse resHttp) {
            try {
                HttpWebRequest req = CreateRequest(remoteFileUrl, WebRequestMethods.Http.Head);
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                resHttp.SetInfo(res.StatusCode, "receive result from server.");
                res.Close();
            } catch(WebException w) {
                resHttp.SetInfo( HttpStatusCode.BadRequest, "code: " + w.Status + "message: " + w.Message);
            } catch(Exception e) {
                resHttp.SetInfo( HttpStatusCode.BadRequest, e.Message);
            }
        }
        
        public static async void Download(string remoteFileUrl, string localFileUrl, Action<HttpResponse> onFinish) {
            HttpResponse resExist = new HttpResponse();
            await Task.Run(() => { StartCheckExist(remoteFileUrl, ref resExist);});
            if(!resExist.IsSuccess) {
                onFinish?.Invoke(resExist);
                return;
            }
            HttpResponse resDownload = new HttpResponse();
            await Task.Run(() => { StartDownload(remoteFileUrl, localFileUrl, ref resDownload); });
            onFinish?.Invoke(resDownload);
        }

        private static void StartDownload(string remoteFileUrl, string localFileUrl, ref HttpResponse resHttp) {
            try {
                HttpWebRequest req = CreateRequest(remoteFileUrl, WebRequestMethods.Http.Get);
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                if(res.StatusCode == HttpStatusCode.OK) {
                    using(FileStream fileStream = File.Create(localFileUrl)) {
                        Stream stream = res.GetResponseStream();
                        byte[] bytes = new byte[1024];
                        int length = stream.Read(bytes, 0, bytes.Length);
                        while(length != 0) {
                            fileStream.Write(bytes, 0, length);
                            length = stream.Read(bytes, 0, bytes.Length);
                        }
                        fileStream.Close();
                        stream.Close();
                    }
                    resHttp.SetInfo(HttpStatusCode.OK, "download success.");
                } else {
                    resHttp.SetInfo(res.StatusCode, "download failed.");
                }
                res.Close();
            } catch(WebException w) {
                resHttp.SetInfo(HttpStatusCode.BadRequest, "code: " + w.Status + "message: " + w.Message);
            } catch(Exception e) {
                resHttp.SetInfo(HttpStatusCode.BadRequest, e.Message);
            }
        }

        // 上传时测试了不需要账号密码的情况，是可以通过的；验证用的是hfs.exe这个现成的http服务器
        // 设置账号密码的情况暂时没通过，待后面的测试(放在Unity工程中，加入账号密码就测试成功了)；
        // 关键主要看资源服务器是什么类型的，要约定什么类型的格式，一般为了安全上传是需要账号和密码凭证的
        public static async void Upload(string account, string password, string remoteUrl, string fileName, string localFileUrl, Action<HttpResponse> onFinish) {
            HttpResponse res = new HttpResponse();
            await Task.Run(() => { StartUpload(account, password, remoteUrl, fileName, localFileUrl, ref res); });
            onFinish?.Invoke(res);
        }

        // 此方法上传不会覆盖原文件
        private static void StartUpload(string account, string password, string remoteUrl, string fileName, string localFileUrl, ref HttpResponse resHttp) {
            try{
                HttpWebRequest req = CreateRequest(remoteUrl, WebRequestMethods.Http.Post, 500000);
                req.ContentType = "multipart/form-data;boundary=" + HTTP_BOUNDARY_DEFINE;

                req.Credentials = new NetworkCredential(account, password);
                req.PreAuthenticate = true;

                string head = string.Format("--{0}\r\n" +
                    "Content-Disposition:form-data;name=\"file\";filename=\"{1}\"\r\n" +
                    "Content-Type:application/octet-stream\r\n\r\n", HTTP_BOUNDARY_DEFINE, fileName);
                byte[] headBytes = Encoding.UTF8.GetBytes(head);

                byte[] endBytes = Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--\r\n", HTTP_BOUNDARY_DEFINE));

                using(FileStream fileStream = File.OpenRead(localFileUrl)) {
                    req.ContentLength = headBytes.Length + fileStream.Length + endBytes.Length;
                    Stream stream = req.GetRequestStream();
                    stream.Write(headBytes, 0, headBytes.Length);
                    
                    byte[] contentBytes = new byte[1024];
                    int contentLength = fileStream.Read(contentBytes, 0, contentBytes.Length);
                    while(contentLength != 0) {
                        stream.Write(contentBytes, 0, contentLength);
                        contentLength = fileStream.Read(contentBytes, 0, contentBytes.Length);
                    }
                    stream.Write(endBytes, 0, endBytes.Length);
                    fileStream.Close();
                    stream.Close();
                }

                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                if(res.StatusCode == HttpStatusCode.OK) {
                    resHttp.SetInfo(res.StatusCode, "upload success.");
                } else {
                    resHttp.SetInfo(res.StatusCode, "upload failed.");
                }
                res.Close();
            } catch(WebException w) {
                resHttp.SetInfo(HttpStatusCode.BadRequest, "code: " + w.Status + "message: " + w.Message);
            } catch(Exception e) {
                resHttp.SetInfo(HttpStatusCode.BadRequest, e.Message);
            }
        }

    }

}
