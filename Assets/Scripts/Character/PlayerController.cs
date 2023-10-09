using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace MultiplayerGame
{

    public class PlayerController : MonoBehaviourPun
    {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private int damage;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackDelay;
        [SerializeField] private float lastAttackTime;

        [HideInInspector]
        [SerializeField] private int id;
        [SerializeField] private Animator playerAnim;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Player photonPlayer;
        [SerializeField] private SpriteRenderer sr;

        [SerializeField] private float moveSpeed = 1;
        [SerializeField] private int gold;
        [SerializeField] private int currentHP;
        [SerializeField] private int maxHP;
        [SerializeField] private bool isDead;

        [SerializeField] private static PlayerController currentPlayer;

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine)
                return;
            Move();

            if (Input.GetMouseButton(0) && Time.time - lastAttackTime > attackDelay)
                Attack();
        }

        private void Move()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            rb.velocity = new Vector2(x, y) * moveSpeed;
            if(x != 0 || y != 0)
            {
                playerAnim.SetBool("isMoving", true);
                if (x > 0)
                    photonView.RPC("FlipRight", RpcTarget.All);
                else
                    photonView.RPC("FlipLeft", RpcTarget.All);

            }
            else
            {
                playerAnim.SetBool("isMoving", false);

            }


        }

        private void Attack()
        {

        }

        [PunRPC]
        void FlipLeft()
        {
            sr.flipX = true;
        }

        void FlipRight()
        {
            sr.flipX = false;
        }
    }

}