﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
    public bool moveLeft = false;
    public float speed = 3f;
    public float knockbackForce;

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 movement;

	    if (moveLeft)
        {
            movement = new Vector3(-Time.deltaTime*speed, 0.0f, 0.0f);
        }
        else
        {
            movement = new Vector3(Time.deltaTime*speed, 0.0f, 0.0f);
        }

        transform.position += movement;
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyBorder" || other.tag == "InvisibleBorder")
        {
            moveLeft = !moveLeft;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); //flip game object
        }
        else if (other.tag == "Head")
        {
            Head head = other.GetComponent<Head>();
            Segment segment = head.successor != null ? head.successor : null;

            if (segment == null)
            {
				CheckpointManager checkpointManager = other.GetComponent<CheckpointManager>();
				GameObject currentCheckpointObject = checkpointManager.currentCheckpoint;
				Checkpoint currentCheckpoint = currentCheckpointObject.GetComponent<Checkpoint>();
				
				Destroy(head.gameObject);
                currentCheckpoint.respawn();
                return;
            }

            while (segment.successor != null)
            {
                segment = segment.successor;
            }

            Destroy(segment.gameObject);
            
            //Little Knockback on impact
            Vector3 knockbackDirection = head.currentFacingRight ? -(other.transform.right) : other.transform.right; 
			other.gameObject.rigidbody2D.velocity = knockbackDirection * knockbackForce + new Vector3(0f, 10f, 0);
			//

        }
        else if (other.tag == "Segment")
        {
            Segment segment = other.GetComponent<Segment>();
            
            while (segment.successor != null)
            {
                segment = segment.successor;
            }

            Destroy(segment.gameObject);
            
			//Little Knockback on impact
			Head head = segment.head;
			Vector3 knockbackDirection = head.currentFacingRight ? other.transform.right : -other.transform.right; 
			head.gameObject.rigidbody2D.velocity = knockbackDirection * knockbackForce + new Vector3(0f, 10f, 0);
			//
        }
    }
}
