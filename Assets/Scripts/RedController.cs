using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedController : MonoBehaviour
{
    GameObject player;
    Vector3 playerLocation;
    UnityEngine.AI.NavMeshAgent navAgent;
    [SerializeField] float speed;

    void Start()
    {
        //player = GameObject.FindWithTag("Player");
        player = GameObject.Find("Green");
        navAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        playerLocation = player.transform.position;
    }
    
    void FixedUpdate()
    {
        if (player != null)
        {
            GetPlayer();
        }
    }

    void GetPlayer()
    {
        navAgent.speed = speed;
        playerLocation = player.transform.position;
        navAgent.destination = playerLocation;
    }
}
