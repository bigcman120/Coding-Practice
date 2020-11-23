using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedController : MonoBehaviour
{
    GameObject player;
    GameObject nearestWall;
    Vector3 playerLocation;
    Vector3 avoidDestination;
    float playerDistance;
    UnityEngine.AI.NavMeshAgent navAgent;
    [SerializeField] float avoidSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float avoidDistance;
    [SerializeField] bool chase;
    [SerializeField] bool avoid;

    void Start()
    {
        //player = GameObject.FindWithTag("Player");
        player = GameObject.Find("Green");
        navAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        playerLocation = player.transform.position;
        nearestWall = GameObject.FindWithTag("Wall");
    }
    
    void FixedUpdate()
    {
        if (player != null)
        {
            playerLocation = player.transform.position;
            if (chase)
            {
                GetPlayer();
            }
            else if (avoid)
            {
                AvoidPlayer();
            }
            else if (!chase && !avoid)
            {
                Idle();
            }
        }
    }

    void GetPlayer()
    {
        //Set the Agent to begin moving
        //Target the player as the goal destination
        navAgent.speed = chaseSpeed;
        navAgent.destination = playerLocation;
    }

    void AvoidPlayer()
    {
        this.transform.LookAt(playerLocation);
        playerDistance = Vector3.Distance(this.transform.position, playerLocation);
        if(playerDistance < avoidDistance)
        {
            //Cast Ray Directly behind the enemy
            //Give Raycast a distance equal to the avoidDistance - playerDistance + some buffer value
            //Mark the endpoint of the ray as the new avoidDestination
            //Set the NavAgent Destination to the avoidDestination
            Ray distancingRay = new Ray(this.transform.position, -transform.forward);
            //avoidDestination = new Vector3(-playerLocation.x, -playerLocation.y, playerLocation.z);
            avoidDestination = distancingRay.GetPoint((avoidDistance - playerDistance) + 2);
            navAgent.speed = avoidSpeed;
            navAgent.destination = avoidDestination;
        }

        if(Vector3.Distance(this.transform.position, avoidDestination) < 0.5f)
        {
            //Set the NavAgent to walk when close to their destination
            navAgent.speed = walkSpeed;
        }
    }

    void Idle()
    {
        this.transform.Rotate(0, 0.1f, 0);
    }
}