using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Wsh.Net.Sockets {

    public class UnitySocketDemo : MonoBehaviour {

        [SerializeField] private string m_message;

        private List<UnitySocket> m_unitySocketList = new List<UnitySocket>();

        private void OnGUI() {
            if(GUILayout.Button("CreateAndConnectSocket", GUILayout.Width(200), GUILayout.Height(80))) {
                CreateSocketList();
            }

            if(GUILayout.Button("SendToServer", GUILayout.Width(200), GUILayout.Height(80))) {
                SocketListSendMessageToServer();
            }
        }

        private void CreateSocketList() {
            for(int i = 0; i < 1; i++) {
                string socketName = "Game" + (i + 1).ToString();
                UnitySocket unitySocket = UnitySocketManager.Instance.CreateSocket(socketName, "127.0.0.1", 9036, (isSuccess, message) => {
                    Log.Info(socketName, isSuccess, message);
                });
                unitySocket.OnReceiveMessage += OnReceiveMessageEvent;
                unitySocket.OnDisconnectPassively += OnDisconnectPassivelyEvent;
                unitySocket.OnNetworkError += OnNetworkErrorEvent;
                m_unitySocketList.Add(unitySocket);
            }
        }

        private void OnNetworkErrorEvent(string message) {
            Log.Info("OnNetworkError", message);
        }

        private void OnDisconnectPassivelyEvent() {
            Log.Info("Disconnect passively");
        }

        private void SocketListSendMessageToServer() {
            if(string.IsNullOrEmpty(m_message)) {
                return;
            }
            for(int i = 0; i < m_unitySocketList.Count; i++) {
                m_unitySocketList[i].SendMessage(i, Encoding.UTF8.GetBytes(m_message), (isSuccess, msg) => {
                    Log.Info("SendMessage", isSuccess, msg);
                });
            }
        }

        private void OnReceiveMessageEvent(UnitySocket sender, byte[] data) {
            int nowIndex = 0;
            int messageType = BitConverter.ToInt32(data);
            string message = Encoding.UTF8.GetString(data, UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH, data.Length - UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH);
            Log.Info("Receive:", sender.Name, messageType, message);
        }

        private byte[] GetSendMessageBytes(int index, string message) {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] data = new byte[UnitySocketDefine.MESSAGE_HEAD_BYTES_LENGTH + messageBytes.Length];
            Array.Copy(BitConverter.GetBytes(index), 0, data, 0, UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH);
            Array.Copy(BitConverter.GetBytes(messageBytes.Length), 0, data, UnitySocketDefine.MESSAGE_TYPE_BYTES_LENGTH, UnitySocketDefine.MESSAGE_LENGTH_BYTES_LENGTH);
            Array.Copy(messageBytes, 0, data, UnitySocketDefine.MESSAGE_HEAD_BYTES_LENGTH, messageBytes.Length);
            return data;
        }
        
    }

}
