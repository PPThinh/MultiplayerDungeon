using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace MultiplayerGame 
{

    public class MenuManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        [Header("Screens")]
        public GameObject mainScreen;
        public GameObject createRoomScreen;
        public GameObject lobbyScreen;
        public GameObject lobbyBrowserScreen;

        [Header("Main Screen")]
        public Button btnCreateRoom;
        public Button btnFindRoom;

        [Header("Lobby Screen")]
        public TextMeshProUGUI txtPlayerList;
        public TextMeshProUGUI txtRoomInfo;
        public Button btnStart;

        [Header("Lobby Browser Screen")]
        public RectTransform lstRoomContainer;
        public GameObject btnRoomPrefabs;

        private List<GameObject> _btnRooms = new List<GameObject>();
        private List<RoomInfo> _lstRoom = new List<RoomInfo>();
        // Start is called before the first frame update
        void Start()
        {
            // disable menu buttons at the start game
            btnCreateRoom.interactable = false;
            btnFindRoom.interactable = false;

            // enable the cursor
            Cursor.lockState = CursorLockMode.None;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.CurrentRoom.IsVisible = true;
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
        }

        public void SetScreen(GameObject screen)
        {
            mainScreen.SetActive(false);
            createRoomScreen.SetActive(false);
            lobbyScreen.SetActive(false);
            lobbyBrowserScreen.SetActive(false);

            // active the requested screen
            screen.SetActive(true);

            if(screen == lobbyBrowserScreen)
            {
                UpdateLobbyBrowserUI();
            }
        }


        public void OnPlayerNameChanged(TMP_InputField inputPlayerName) => 
                                                        PhotonNetwork.NickName = inputPlayerName.text;

        public override void OnConnectedToMaster()
        {
            btnCreateRoom.interactable = true;
            btnFindRoom.interactable = true;
        }

        public void BtnOnBackScreen() => SetScreen(mainScreen);
        public void BtnOnCreateRoom() => SetScreen(createRoomScreen);
        public void BtnOnFindRoom() => SetScreen(lobbyBrowserScreen);
        public void BtnOnCreate(TMP_InputField inputRoomName) 
                                => NetworkManager.instance.CreateRoom(inputRoomName.text);

        public override void OnJoinedRoom()
        {
            SetScreen(lobbyScreen);
            photonView.RPC("UpdateLobbyUI", RpcTarget.All);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer) => UpdateLobbyUI();

        [PunRPC]
        void UpdateLobbyUI()
        {
            btnStart.interactable = PhotonNetwork.IsMasterClient;

            txtPlayerList.text = "";
            foreach (Player player in PhotonNetwork.PlayerList)
                txtPlayerList.text += player.NickName + "\n";
            txtRoomInfo.text = "<b>Room Name </b> \n" + PhotonNetwork.CurrentRoom.Name;
        }

        public void BtnOnStartGame()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
        }
        public void BtnLeaveLobby()
        {
            PhotonNetwork.LeaveRoom();
            SetScreen(mainScreen);
        }

        GameObject CreateCurrentRooms()
        {
            GameObject buttonObj = Instantiate(btnRoomPrefabs, lstRoomContainer.transform);
            _btnRooms.Add(buttonObj);
            return buttonObj;
        }

        void UpdateLobbyBrowserUI()
        {
            foreach(GameObject button in _btnRooms)
            {
                button.SetActive(false);
            }

            for(int x = 0; x < _lstRoom.Count; x++)
            {
                GameObject btn = x >= _btnRooms.Count ? CreateCurrentRooms() : _btnRooms[x];
                btn.SetActive(false);
                // set room name & players
                btn.transform.Find("txtRoomName").GetComponent<TextMeshProUGUI>().text = _lstRoom[x].Name;
                btn.transform.Find("txtCounterPlayer").GetComponent<TextMeshProUGUI>().text = _lstRoom[x].PlayerCount + " / " + _lstRoom[x].MaxPlayers;

                // set button when click
                Button btnComp = btn.GetComponent<Button>();
                string roomName = _lstRoom[x].Name;

                btnComp.onClick.RemoveAllListeners();
                btnComp.onClick.AddListener(() => { BtnOnJoinRoom(roomName); });
            }

        }

        public void BtnOnRefresh()
        {
            UpdateLobbyBrowserUI();
        }

        public void BtnOnJoinRoom(string roomName)
        {
            NetworkManager.instance.JoinRoom(roomName);
        }

        public override void OnRoomListUpdate(List<RoomInfo> allRooms)
        {
            _lstRoom = allRooms;
        }


        // Update is called once per frame
        void Update()
        {
        
        }
    }

}