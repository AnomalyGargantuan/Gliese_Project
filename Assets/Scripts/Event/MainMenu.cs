﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	// Starts listening on the OnEnable
	void OnEnable()
	{
		EventManager.StartListening("NewGame", ButtonNewGame);
		EventManager.StartListening("Exit", ButtonExit);
	}

	// Stops listening on the OnDisable to avoid memory leaks
	void OnDisable()
	{
		EventManager.StopListening("NewGame", ButtonNewGame);
		EventManager.StopListening("Exit", ButtonExit);
	}

	//Handles the code from the New game Menu Button
	public void ButtonNewGame()
	{
		Debug.Log("New Game");
		Application.LoadLevel("Demo_0.0.0001");
	}

	//Handles the code from the Exit game Menu Button
	public void ButtonExit()
	{
		Debug.Log("Exit game");
	}
}