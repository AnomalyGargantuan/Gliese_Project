﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerPhysics))]
public class PlayerController : MonoBehaviour 
{
	//Player Handling
	public float walkSpeed = 8;
	public float runSpeed = 12;
	public float acceleration = 30;
	public float gravity = 20;
	public float jumpHeight = 12;

	private float animationSpeed;
	private float currentSpeed;
	private float targetSpeed;
	private Vector2 amountToMove;

	private PlayerPhysics playerPhysics;
	private Animator animator;

	void Start()
	{
		playerPhysics = GetComponent<PlayerPhysics> ();
		animator = GetComponent<Animator> ();
	}

	void Update()
	{

		if(playerPhysics.movementStopped)
		{
			targetSpeed = 0;
			currentSpeed = 0;
		}

		if(playerPhysics.grounded)
		{
			amountToMove.y = 0;
			//Jump
			if (Input.GetButtonDown("Jump"))
			{
				amountToMove.y = jumpHeight;
			}
		}

		animationSpeed = IncrementTowards(animationSpeed, Mathf.Abs(targetSpeed), acceleration);
		animator.SetFloat("Speed", animationSpeed);

		float speed = (Input.GetButton("Run")) ? runSpeed : walkSpeed; //TODO Bug, aumenta velocidade no ar
		targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
		currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);

		amountToMove.x = currentSpeed;
		amountToMove.y -= gravity * Time.deltaTime;

		playerPhysics.Move(amountToMove * Time.deltaTime);

		float moveDir = Input.GetAxisRaw("Horizontal");
		if (moveDir != 0)
		{
			transform.eulerAngles = (moveDir > 0) ? Vector3.up * 180 : Vector3.zero;
		}

	}

	private float IncrementTowards(float n, float target, float a)
	{
		if(n == target)	
		{
			return n;
		}
		else
		{
			float dir = Mathf.Sign(target - n); //Must N be increased or decreased to get closer to the target
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign(target-n)) ? n : target; //If N has passed target then return target, otherwise return N.
		}
	}
}
