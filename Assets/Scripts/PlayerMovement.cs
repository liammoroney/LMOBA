﻿using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class PlayerMovement : NetworkBehaviour {

    private CharacterController controller;
    public float speed = 1;
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }
    [HideInInspector]
    public Vector3 waypoint;

    private Vector3 lastPosition;
    public bool isMoving = false;

    [HideInInspector]
    public Vector3 rotationTarget;
    public float rotationSpeed = 500;
    private Quaternion targetRotation;

    private void Start ()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        waypoint = transform.position;
        
        rotationTarget = Vector3.zero;
    }
	
	void Update ()
    {
        agent.speed = GetComponent<NetworkedPlayerScript>().speed;
        // Smooth rotation
        agent.updateRotation = false;
        Vector3 updatedTarget = new Vector3(rotationTarget.x, transform.position.y, rotationTarget.z);
        if (updatedTarget != transform.position)
            targetRotation = Quaternion.LookRotation(updatedTarget - transform.position);
        if((Vector3.Angle(transform.forward, (updatedTarget - transform.position))) > 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }


        if (transform.position == lastPosition)
        {
            if (isMoving && GetComponent<NetworkedPlayerScript>().anim)
            {
                isMoving = false;
                GetComponent<NetworkedPlayerScript>().anim.SetTrigger("JogToIdle");
            }
        }
        else
        {
            if (!isMoving && GetComponent<NetworkedPlayerScript>().anim)
            {
                isMoving = true;
                GetComponent<NetworkedPlayerScript>().anim.SetTrigger("IdleToJog");
            }
        }


        if (GetComponent<ShootingScript>().isCastingSpell)
        {
            //agent.Stop();
            //agent.speed = 0;
        }

        if(GetComponent<ShootingScript>().target)
        {
            if(Vector3.Distance(transform.position, GetComponent<ShootingScript>().target.position) >= GetComponent<ShootingScript>().autoRange)
            {
                //Debug.Log("too far away to auto, moving closer");
                agent.SetDestination(GetComponent<ShootingScript>().target.position);
            }
        }
        else if (GetComponent<ShootingScript>().isCasting || GetComponent<ShootingScript>().isCastingSpell)
        {
            agent.SetDestination(transform.position);
            agent.updateRotation = false;
            
            // Lerp rotation to face target
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetComponent<ShootingScript>().autoTarget - transform.position), Time.deltaTime * 10);
        }
        else if(waypoint != null && waypoint != transform.position && !GetComponent<NetworkedPlayerScript>().isAI)
        {
            //if(!agent.updateRotation)
                //agent.updateRotation = true;

            agent.SetDestination(waypoint);
        }

        //if (!isLocalPlayer)
            //return;

        if (isLocalPlayer && Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(GetComponent<NetworkedPlayerScript>().myCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if(hit.transform.tag == "Floor")
                {   
                    waypoint = hit.point;
                    rotationTarget = hit.point;
                    // If we click somewhere else on the map, we want to remove our target
                    if(GetComponent<ShootingScript>().target)
                    {
                        GetComponent<ShootingScript>().target = null;
                    }
                }
            }
        }

        lastPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(waypoint, 0.5f);
        //Gizmos.DrawSphere(rotationTarget, 1);
        if (GetComponent<NetworkedPlayerScript>().myCamera)
            Gizmos.DrawLine(GetComponent<NetworkedPlayerScript>().myCamera.transform.position, waypoint);
    }
}
