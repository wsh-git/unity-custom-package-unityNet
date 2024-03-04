using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Wsh.Net.Sockets {
    
    public class UnitySocket {

        public event Action<UnitySocket, byte[]> OnReceiveMessage;
        public event Action OnDisconnectPassively;
        public event Action<string> OnNetworkError;

        public string Name => m_name;
        public string Ip => m_ip;
        public int Port => m_port;
        public bool IsConnected { get { return m_socket.Connected && !m_isDisconnectPassively; } }

        private string m_name;
        private string m_ip;
        private int m_port;
        private Socket m_socket;
        private byte[] m_cacheBytes;
        private int m_cacheNum;
        private Queue<byte[]> m_receiveQueue;
        private float m_heartMessageTime;
        private bool m_isDisconnectPassively;
        private int m_cacheDisconnectMessageType;
        private byte[] m_cacheDisconnectMessage;
        private SocketAsyncEventArgs m_heartSendAsyncEventArgs;

        private void InitData() {
            InitHeartMessageTime();
            m_isDisconnectPassively = false;
            m_cacheBytes = new byte[UnitySocketDefine.MESSAGE_RECEIVE_CACHE_BYTE_SIZE];
            m_cacheNum = 0;
            m_receiveQueue = new Queue<byte[]>();
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        
        public UnitySocket(string name, string ip, int port) {
            m_name = name;
            m_ip = ip;
            m_port = port;
            InitData();
            CreateHeartSendAsyncEventArgs();
        }

        private void InitHeartMessageTime() {
            m_heartMessageTime = UnitySocketDefine.MESSAGE_HEART_SEND_TIME_SPACE;
        }

        private void CreateHeartSendAsyncEventArgs() {
            m_heartSendAsyncEventArgs = new SocketAsyncEventArgs();
            byte[] messageBytes = new byte[10];
            m_heartSendAsyncEventArgs.SetBuffer(messageBytes, 0, messageBytes.Length);
            m_heartSendAsyncEventArgs.Completed += (object socket, SocketAsyncEventArgs eventArgs) => {
                if(eventArgs.SocketError != SocketError.Success) {
                    HandlerNetworkError(eventArgs.SocketError);
                }
            };
        }

        public void StartConnect(Action<bool, string> onFinish) {
            if(IsConnected) {
                onFinish?.Invoke(true, $"The socket({Name}) connected already.");
            } else {
                m_socket.ConnectAsync(CreateConnectSocketAsyncEventArgs(onFinish));
            }
        }

        private SocketAsyncEventArgs CreateConnectSocketAsyncEventArgs(Action<bool, string> onFinish) {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(m_ip), m_port);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = iPEndPoint;
            args.Completed += (object sender, SocketAsyncEventArgs eventArgs) => {
                if(eventArgs.SocketError == SocketError.Success) {
                    // 连接成功之后，可以在这里开启一个心跳消息，例如1s间隔就发一条心跳消息
                    StartReceiveMessage();
                    onFinish?.Invoke(true, $"The socket({Name}) connect success.");
                } else {
                    onFinish?.Invoke(false, $"The socket({Name}) connect failed. {eventArgs.SocketError}");
                }
            };
            return args;
        }

        private void StartReceiveMessage() {
            SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.SetBuffer(m_cacheBytes, 0, m_cacheBytes.Length);
            receiveEventArgs.Completed += OnReceiveMessageEvent;
            m_socket.ReceiveAsync(receiveEventArgs);
        }

        public void SendMessage(byte[] data, Action<bool, string> onFinish) {
            SendMessage(0, data, onFinish);
        }

        private void HandlerDisconnectPassively(int id, byte[] data) {
            if(m_isDisconnectPassively) {
                return;
            }
            if(id != 0) {
                m_cacheDisconnectMessageType = id;
                m_cacheDisconnectMessage = data;
            }
            OnDisconnectPassively?.Invoke();
            m_isDisconnectPassively = true;
        }

        private void HandlerNetworkError(SocketError socketError) {
            if(!m_isDisconnectPassively) {
                OnNetworkError?.Invoke(socketError.ToString() + ":" + (int)socketError);                
            }
        }

        public void Reconnect(Action<bool, string> onFinish) {
            ForceDestroySocket();
            InitData();
            StartConnect((isSuccess, message) => {
                if(isSuccess) {
                    if(m_cacheDisconnectMessageType != 0) {
                        SendMessage(m_cacheDisconnectMessageType, m_cacheDisconnectMessage, null);
                    }
                    m_cacheDisconnectMessageType = 0;
                    m_isDisconnectPassively = false;
                }
                onFinish?.Invoke(isSuccess, message);
            });
        }
        
        public void SendMessage(int id, byte[] data, Action<bool, string> onFinish) {
            if(IsConnected) {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] messageBytes = new byte[UnitySocketDefine.MESSAGE_HEAD_BYTES_LENGTH + data.Length];
                Array.Copy(BitConverter.GetBytes(id), 0, messageBytes, 0, UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH);
                Array.Copy(BitConverter.GetBytes(data.Length), 0, messageBytes, UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH, UnitySocketDefine.MESSAGE_LENGTH_BYTES_LENGTH);
                Array.Copy(data, 0, messageBytes, UnitySocketDefine.MESSAGE_HEAD_BYTES_LENGTH, data.Length);
                args.SetBuffer(messageBytes, 0, messageBytes.Length);
                args.Completed += (object socket, SocketAsyncEventArgs eventArgs) => {
                    if(eventArgs.SocketError == SocketError.Success) {
                        onFinish?.Invoke(true, "Message send success.");
                    } else {
                        onFinish?.Invoke(false, "Message end failed. " + eventArgs.SocketError);
                        HandlerNetworkError(eventArgs.SocketError);
                    }
                };
                m_socket.SendAsync(args);
            } else {
                HandlerDisconnectPassively(id, data);
                onFinish?.Invoke(false, $"The socket({Name} is not connected.)");
            }
        }

        private void TrySendHeartMessage() {
            if(IsConnected) {
                m_socket.SendAsync(m_heartSendAsyncEventArgs);
            } else {
                HandlerDisconnectPassively(0, null);
            }
        }

        private void OnReceiveMessageEvent(object sender, SocketAsyncEventArgs args) {
            if(args.SocketError == SocketError.Success) {
                HandleReceiveMessage(args.BytesTransferred);
                args.SetBuffer(m_cacheNum, args.Buffer.Length - m_cacheNum);
                m_socket.ReceiveAsync(args);
            } else {
                HandlerNetworkError(args.SocketError);
            }
        }

        //处理接受消息 分包、黏包问题的方法
        private void HandleReceiveMessage(int receiveLength) {
            int msgID = 0;
            int msgLength = 0;
            int nowIndex = 0;

            m_cacheNum += receiveLength;

            while(true) {
                //每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
                msgLength = -1;
                //处理解析一条消息
                if(m_cacheNum - nowIndex >= UnitySocketDefine.MESSAGE_HEAD_BYTES_LENGTH) {
                    //解析ID
                    msgID = BitConverter.ToInt32(m_cacheBytes, nowIndex);
                    nowIndex += UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH;
                    //解析长度
                    msgLength = BitConverter.ToInt32(m_cacheBytes, nowIndex);
                    nowIndex += UnitySocketDefine.MESSAGE_LENGTH_BYTES_LENGTH;
                }

                if(m_cacheNum - nowIndex >= msgLength && msgLength != -1) {
                    //解析消息体
                    /*BaseMsg baseMsg = null;
                    switch(msgID) {
                        case 1001:
                        baseMsg = new PlayerMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    }
                    if(baseMsg != null) {
                        receiveQueue.Enqueue(baseMsg);
                    }*/
                    
                    byte[] messageBytes = new byte[UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH + msgLength];
                    Array.Copy(BitConverter.GetBytes(msgID), 0, messageBytes, 0, UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH);
                    Array.Copy(m_cacheBytes, nowIndex, messageBytes, UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH, msgLength);
                    m_receiveQueue.Enqueue(messageBytes);
                    nowIndex += msgLength;
                    if(nowIndex == m_cacheNum) {
                        m_cacheNum = 0;
                        break;
                    }
                } else {
                    if(msgLength != -1) {
                        nowIndex -= UnitySocketDefine.MESSAGE_HEAD_BYTES_LENGTH;
                    }
                        
                    //就是把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                    Array.Copy(m_cacheBytes, nowIndex, m_cacheBytes, 0, m_cacheNum - nowIndex);
                    m_cacheNum = m_cacheNum - nowIndex;
                    break;
                }
            }
        }

        public void OnUpdate(float deltaTime) {
            if(IsConnected) {
                if(m_receiveQueue.Count > 0) {
                    OnReceiveMessage?.Invoke(this, m_receiveQueue.Dequeue());
                }
                CheckSendHeartMessage(deltaTime);
            }
        }

        private void CheckSendHeartMessage(float deltaTime) {
            if(m_heartMessageTime > 0) {
                m_heartMessageTime -= deltaTime;
            } else {
                TrySendHeartMessage();
                InitHeartMessageTime();
            }
        }

        public void CloseConnect() {
            if(IsConnected) {
                // 可自定义断开连接的消息，服务端不能百分百检测到客户端已断开连接
                // CloseMessage

                ForceDestroySocket();
            }
        }
        
        public void Destroy() {
            CloseConnect();
        }

        private void ForceDestroySocket() {
            if(m_socket != null) {
                m_socket.Shutdown(SocketShutdown.Both);
                m_socket.Disconnect(false);
                m_socket.Close();
            }
            m_socket = null;
        }
        
    }

}

