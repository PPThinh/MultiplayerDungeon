using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace MultiplayerGame
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] public int maxPlayers;

        public static NetworkManager instance;

        private void Awake()
        {
            if(instance != null && instance != this)
                gameObject.SetActive(false);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Connecting to the master server 
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public void CreateRoom(string roomName)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)maxPlayers;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        [PunRPC]
        public void ChangeScene(string sceneName)
        {
            PhotonNetwork.LoadLevel(sceneName);
        }
    }

}