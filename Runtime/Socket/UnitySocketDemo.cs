using UnityEngine;

namespace Wsh.Net.Sockets {

    public class UnitySocketDemo : MonoBehaviour {

        private void OnGUI() {
            if(GUILayout.Button("CreateAndConnectSocket", GUILayout.Width(200), GUILayout.Height(80))) {
                UnitySocketManager.Instance.CreateSocket("Game", "127.0.0.1", 8090, (isSuccess, message) => {
                    Log.Info(isSuccess, message);
                });
            }
        }

    }

}
