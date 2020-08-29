using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalKnowledgeSensor : Sensor
{
	GameMode gameMode;

	protected override void Awake()
	{
		base.Awake();
		gameMode = GameObject.Find("GameMode").GetComponent<GameMode>();
	}

	public MsPacMan GetMsPacMan()
	{
		return gameMode.MsPacMan;
	}

	public Ghost GetGhost(GhostName ghost)
	{
		return gameMode.GetGhost(ghost);
	}
}
