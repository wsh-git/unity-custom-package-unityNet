using System;
using System.Collections.Generic;
using UnityEngine;
using Wsh.Singleton;

namespace Wsh.Net.Sockets {
    public class UnitySocketManager : Singleton<UnitySocketManager>, ISingleton {

        private List<UnitySocket> m_socketList;
        private int m_index;

        public void OnInit() {
            m_socketList = new List<UnitySocket>();
        }

        public UnitySocket CreateSocket(string name, string ip, int port, Action<bool, string> onFinish) {
            UnitySocket unitySocket = new UnitySocket(name, ip, port);
            unitySocket.StartConnect(onFinish);
            m_socketList.Add(unitySocket);
            return unitySocket;
        }

        protected override void Update() {
            for(m_index = 0; m_index < m_socketList.Count; m_index++) {
                m_socketList[m_index].OnUpdate(Time.deltaTime);
            }
        }

        public void OnDeinit() {
            m_socketList.Clear();
            m_socketList = null;
        }

    }
}

