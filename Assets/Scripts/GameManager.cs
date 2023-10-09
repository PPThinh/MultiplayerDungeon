using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace MultiplayerGame
{

    public class GameManager : MonoBehaviourPun
    {
        [Header("Players")]
        public string playerPrefabPath;
        public Transform[] spawnPoint;
        public float respawnTime;
        private int _playersInGame;

        public static GameManager instance;

        private void Awake()
        {
            instance = this;
        }


        // Start is called before the first frame update
        void Start()
        {
            photonView.RPC("ImInGame", RpcTarget.AllBuffered);        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        [PunRPC]
        void ImInGame()
        {
            _playersInGame++;
            if (_playersInGame == PhotonNetwork.PlayerList.Length)
                SpawnPlayer();
        }

        void SpawnPlayer()
        {
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);

        }
    }

}