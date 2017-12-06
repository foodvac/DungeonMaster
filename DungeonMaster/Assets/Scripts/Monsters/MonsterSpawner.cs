﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

	// List of different monster types. Monster1 is in monsters[0], etc.
    [SerializeField]
    private List<GameObject> monsters;
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnMonster (int monster, Vector3 position) {
		GameObject.Instantiate(monsters[monster - 1], position, Quaternion.identity);
		Debug.Log("Spawned monster" + (monster) + " at zone " + name);
       
    }
}
