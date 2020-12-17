using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedController : MonoBehaviour
{
    GameObject player;
    GameObject nearestWall;
    GameObject target;
    Vector3 playerLocation;
    Vector3 avoidDestination;
    float playerDistance;
    float slowFallForce;
    
    UnityEngine.AI.NavMeshAgent navAgent;
    float fallBegin;
    float fallEnd;
    
    bool falling;
    Rigidbody targetRigidbody;
    [SerializeField] float slowFallForceBase;
    [SerializeField] float slowFallIncrease;
    [SerializeField] float fallTimer;
    [SerializeField] float avoidSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float avoidDistance;
    [SerializeField] float chaseOffset;
    [SerializeField] bool chase;
    [SerializeField] bool avoid;

    void Start()
    {
        //player = GameObject.FindWithTag("Player");
        player = GameObject.Find("Green");
        navAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        playerLocation = player.transform.position;
        nearestWall = GameObject.FindWithTag("Wall");
        MarkTarget();
        targetRigidbody = target.GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        if (player != null)
        {
            //Updates the Location of and Distance from the Player
            playerLocation = player.transform.position;
            playerDistance = Vector3.Distance(this.transform.position, playerLocation);

            //Chase
            if (chase && !avoid)
            {
                GetPlayer();
            }
            //Avoid
            else if (avoid && !chase)
            {
                AvoidPlayer();
            }
            //Idle
            else if (!chase && !avoid)
            {
                Idle();
            }
            //Resolve Chase and Avoid Paradox
            else if (chase && avoid)
            {
                //Randomly choose to Avoid or Chase
                int r1 = new int();
                r1 = Random.Range(1, 3);
                if (r1 == 1)
                    avoid = false;
                else
                    chase = false;
            }

            //Slowed Fall Speed
            if (falling == true)
            {
                //Trigger SlowFall
                SlowFall();
                if (Time.time > fallEnd)
                {
                    //Return to Normal Falling Speed
                    targetRigidbody.useGravity = true;
                    falling = false;
                }
            }
        }
    }

    void GetPlayer()
    {
        //Set the Agent to begin moving
        navAgent.speed = chaseSpeed;
        //If the Target isn't too far above the Agent
        if (Mathf.Abs(playerLocation.y - this.transform.position.y) < 1)
        {
            //Face the Target
            //Cast a Ray at the Target
            //Sets destination to a point slightly offset from the Target
            this.transform.LookAt(playerLocation);
            Ray pursuitRay = new Ray(this.transform.position, transform.forward);
            navAgent.destination = pursuitRay.GetPoint(chaseOffset + playerDistance);
        }

        //If Close enough to the Target to attack
        if (playerDistance < 1.0f)
        {
            Uppercut(400f);
            //Shove(100f);
        }
    }

    void AvoidPlayer()
    {
        //Face the Target
        this.transform.LookAt(playerLocation);

        //If the Agent isn't far enough away from the target
        if(playerDistance < avoidDistance)
        {
            //Cast Ray Directly behind the enemy
            //Give Raycast a distance equal to the avoidDistance - playerDistance + some buffer value
            //Mark the endpoint of the ray as the new avoidDestination
            //Set the NavAgent Destination to the avoidDestination
            Ray distancingRay = new Ray(this.transform.position, -transform.forward);
            avoidDestination = distancingRay.GetPoint((avoidDistance - playerDistance));
            navAgent.speed = avoidSpeed;
            navAgent.destination = avoidDestination;
        }

        //If the target is getting close to far enough away
        if(Vector3.Distance(this.transform.position, avoidDestination) < 0.5f)
        {
            //Set the NavAgent to walk when close to their destination
            navAgent.speed = walkSpeed;
        }
    }

    void Idle()
    {
        //Agent spins in a circle when idle
        navAgent.destination = this.transform.position;
        this.transform.Rotate(0, 0.1f, 0);
    }

    void Uppercut(float launchForce)
    {
        //If the target has a Rigidbody component
        if(targetRigidbody != null)
        {
            //Knock the Target into the air and cause it to slow fall
            targetRigidbody.AddForce(target.transform.up * launchForce);
            FallTracker();
        }
    }

    void FallTracker()
    {
        //Resets all important trackers fro the slow fall
        falling = true;
        slowFallForce = slowFallForceBase;
        fallBegin = Time.time;
        fallEnd = fallBegin + fallTimer;
        targetRigidbody.velocity = Vector3.zero;
        targetRigidbody.angularVelocity = Vector3.zero;
    }

    void SlowFall()
    {
        //Disable Gravity on the target rigid body
        //Slowly shift the target upwards
        targetRigidbody.useGravity = false;
        targetRigidbody.AddForce(target.transform.up * slowFallForce);
        if (slowFallForce > 0)
        {
            slowFallForce -= slowFallIncrease;
        }
    }

    void Shove(float shoveForce)
    {
        //Gets the rigidbody component of the target
        //Shoves them away from red by giving the target a force in the opposite direction
        targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody != null)
        {
            targetRigidbody.AddForce(player.transform.forward * shoveForce);
        }
    }

    void MarkTarget()
    {
        target = player;
    }
}