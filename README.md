# FTP

## 1. 上传

account: FTP服务器允许上传的账号；

password: 对应账号的密码；

remoteFileUrl: 完整的上传地址，包括上传后的文件名；（例如：ftp://127.0.0.1/uploadTest.txt）

localFileUrl: 需要上传的本地文件的地址；

onFinish: 上传的回调；

```c#
public static async void Upload(string account, string password, string remoteFileUrl, string localFileUrl, Action<FTPResponse> onFinish);
```



## 2. 下载

account: FTP服务器允许下载的账号；

password: 对应账号的密码；

remoteFileUrl: 完整的文件上传地址，包括文件名；（例如：ftp://127.0.0.1/fileTest.txt）

localFileUrl: 下载到本地文件的地址；

onFinish: 下载的回调；

```c#
public static async void Download(string account, string password, string remoteFileUrl, string localFileUrl, Action<FTPResponse> onFinish);
```



## 3. 删除

account: FTP服务器允许删除操作的账号；

password: 对应账号的密码；

remoteFileUrl: 删除的完整的文件地址，包括文件名；（例如：ftp://127.0.0.1/fileTest.txt）

onFinish: 删除操作的回调

```c#
public static async void DeleteFile(string account, string password, string remoteFileUrl, Action<FTPResponse> onFinish);
```



## 4. 获取文件大小

account: FTP服务器操作的账号；

password: 对应账号的密码；

remoteFileUrl: 完整的文件地址，包括文件名；（例如：ftp://127.0.0.1/fileTest.txt）

onFinish: 操作的回调

```c#
public static async void GetFileSize(string account, string password, string remoteFileUrl, Action<FTPResponse> onFinish);
```



## 5. 创建文件夹

account: FTP服务器操作的账号；

password: 对应账号的密码；

remoteDirectoryUrl: 服务器上的文件夹地址；（例如：ftp://127.0.0.1/demo/）

onFinish: 操作的回调

```c#
public static async void MakeDirectory(string account, string password, string remoteDirectoryUrl, Action<FTPResponse> onFinish);
```



## 6. 获取文件列表

account: FTP服务器操作的账号；

password: 对应账号的密码；

remoteDirectoryUrl: 服务器上的文件夹地址；（例如：ftp://127.0.0.1/demo/）

onFinish: 操作的回调

```c#
public static async void GetFileList(string account, string password, string remoteDirectoryUrl, Action<FTPResponse> onFinish);
```



# Http

## 1. 上传

account: Http服务器操作的账号；

password: 对应账号的密码；

remoteUrl: 服务器上的文件夹地址；（例如：ftp://127.0.0.1/demo/）

fileName: 上传之后的文件名字；

localFileUrl: 需要上传的本地的文件路径；

onFinish: 操作的回调

```c#
public static async void Upload(string account, string password, string remoteUrl, string fileName, string localFileUrl, Action<HttpResponse> onFinish);
```



## 2. 下载

remoteFileUrl: 服务器上的文件地址；（例如：ftp://127.0.0.1/demo/file.txt）

localFileUrl: 需要下载到本地的文件路径，包括文件名；

onFinish: 操作的回调

```c#
public static async void Download(string remoteFileUrl, string localFileUrl, Action<HttpResponse> onFinish);
```



## 3. 文件是否存在

remoteFileUrl: 服务器上的文件地址；（例如：ftp://127.0.0.1/demo/file.txt）

onFinish: 操作的回调

```c#
public static async void IsExist(string remoteFileUrl, Action<HttpResponse> onFinish);
```



# WWW

> 主要用于游戏运行时的功能；

## 1. 下载AssetBundle

```c#
public void DownloadAssetBundle(string url, Action<float> onProgress, Action<bool, string, AssetBundle> onFinish);
```

## 2. 下载图片

```c#
public void DownloadTexture(string url, Action<float> onProgress, Action<bool, string, Texture> onFinish);
```

## 3. 下载音频

```c#
public void DownloadAudioClip(string url, Action<float> onProgress, Action<bool, string, AudioClip> onFinish);
```

## 4. 下载文本

```c#
public void DownloadText(string url, Action<float> onProgress, Action<bool, string, string> onFinish);
```

## 5. 下载字节数组

```c#
public void DownloadBytes(string url, Action<float> onProgress, Action<bool, string, byte[]> onFinish);
```

## 6. 下载其它类型的资源

```c#
public void Download<T>(string url, Action<float> onProgress, Action<bool, string, T> onFinish);
```

## 7. 上传访问数据

主要是通过WWWForm post的方式来上传访问数据服务器；

如果使用get方式，可以直接使用第 6 个方法，把参数直接拼接到url当中；

```c#
public void Request<T>(string url, WWWForm form, Action<float> onProgress, Action<bool, string, T> onFinish);
```



# UnityWebRequest

> 主要用于游戏运行时的功能；

## 1. 下载AssetBundle

```c#
public void DownloadAssetBundle(string remoteFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish);
```

## 2. 下载图片

```c#
public void DownloadTexture(string remoteFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish);
```

## 3. 下载音频

```c#
public void DownloadAudioClip(string remoteFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish);
```

## 4. 下载文本

```c#
public void RequestText(string remoteUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish);
```

## 5. 下载文件

```c#
public void DownloadFile(string remoteFileUrl, string localFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish);
```

## 6. 上传文件

```c#
public void UploadFile(string remoteUrl, string fileName, string localFileUrl, Action<float, ulong> onProgress, Action<UnityWebResponse> onFinish);
```



# Socket

> 适用于建立长连接通讯；
>
> 每条发送的消息格式：4个字节（消息类型） + 4个字节（消息长度） + 消息内容；
>
> 内部包含分包黏包的处理；

## 1. 创建连接

```c#
UnitySocket unitySocket = UnitySocketManager.Instance.CreateSocket(socketName, "127.0.0.1", 9036, (isSuccess, message) => {
    Log.Info(socketName, isSuccess, message);
});
```

## 2. 注册消息

```c#
unitySocket.OnReceiveMessage += OnReceiveMessageEvent; // 接收的数据事件 4个字节（消息类型） + 消息内容
unitySocket.OnDisconnectPassively += OnDisconnectPassivelyEvent; // 连接中断事件
unitySocket.OnNetworkError += OnNetworkErrorEvent; // 网络错误事件
```

