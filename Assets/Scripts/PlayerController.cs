﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerPhysics))]
public class PlayerController : Entity 
{
	//Player Handling
	public float walkSpeed = 8;
	public float runSpeed = 12;
	public float acceleration = 30;
	public float gravity = 20;
	public float jumpHeight = 12;
	public float slideDecelaration = 10;

	//System
	private float animationSpeed;
	private float currentSpeed;
	private float targetSpeed;
	private Vector2 amountToMove;
	private float moveDirX;

	//states
	private bool jumping;
	private bool sliding;
	//private bool wallHolding;
	private bool _wallHolding;

	private bool wallHolding //Geter and Seter for variables //TODO Acertar para todos os Bools
	{
		get {return _wallHolding;}

		set
		{
			_wallHolding = value;
			animator.SetBool("WallHold", _wallHolding);
		}
	}

	//Components
	private PlayerPhysics playerPhysics;
	private Animator animator;

	void Start()
	{
		playerPhysics = GetComponent<PlayerPhysics> ();
		animator = GetComponent<Animator> ();

		animator.SetLayerWeight(1, 1);
	}

	void Update()
	{
		//Reset Accelaration uppon colision
		if(playerPhysics.movementStopped)
		{
			targetSpeed = 0;
			currentSpeed = 0;
		}

		//If player is touching the ground
		if(playerPhysics.grounded)
		{
			amountToMove.y = 0;

			if(wallHolding)
			{
				wallHolding = false;
				//animator.SetBool("WallHold", false);
			}

			if(jumping)
			{
				jumping = false;
				animator.SetBool("Jumping", false);
			}

			if(sliding)
			{
				if(Mathf.Abs(currentSpeed) < .25f)
				{
					sliding = false;
					animator.SetBool("Sliding", false);
					playerPhysics.ResetCollider();
				}
			}



			//Slide Input
			if(Input.GetButtonDown("Slide"))
			{
				sliding = true;
				animator.SetBool("Sliding", true);
				targetSpeed = 0;

				playerPhysics.SetCollider(new Vector3(10.3f, 1.5f, 3), new Vector3(.35f, .75f, 0));
			}
		}
		else
		{
			if(!wallHolding)
			{
				if(playerPhysics.canWallHold)
				{
					wallHolding = true;
					//animator.SetBool("WallHold", true);
				}
			}
		}

		//Jump
		if (Input.GetButtonDown("Jump"))
		{
			if(playerPhysics.grounded || wallHolding)
			{
				amountToMove.y = jumpHeight;
				jumping = true;
				animator.SetBool("Jumping", true);
				
				if(wallHolding)
				{
					wallHolding = false;
					//animator.SetBool("WallHold", false);
				}
			}
		}

		animationSpeed = IncrementTowards(animationSpeed, Mathf.Abs(targetSpeed), acceleration);
		animator.SetFloat("Speed", animationSpeed);

		moveDirX = Input.GetAxisRaw("Horizontal");
		if(!sliding)
		{
			//Input
			float speed = (Input.GetButton("Run")) ? runSpeed : walkSpeed; //TODO Bug, aumenta velocidade no ar
			targetSpeed = moveDirX * speed;
			currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);

			//Face direction
			if (moveDirX != 0 && !wallHolding)
			{
				transform.eulerAngles = (moveDirX > 0) ? Vector3.up * 180 : Vector3.zero;
			}
		}
		else
		{
			currentSpeed = IncrementTowards(currentSpeed, targetSpeed, slideDecelaration);
		}

		//Set amount to move
		amountToMove.x = currentSpeed;

		if(wallHolding)
		{
			amountToMove.x = 0;
			if(Input.GetAxisRaw("Vertical") != -1)
			{
				amountToMove.y = 0;
			}
		}

		amountToMove.y -= gravity * Time.deltaTime;
		playerPhysics.Move(amountToMove * Time.deltaTime, moveDirX);
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
