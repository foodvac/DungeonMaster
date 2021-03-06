﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DmController : MonoBehaviour {

    private gamecontroller.DMAbilities currentabilities;
    [SerializeField]
    private GameObject monsterspawns;
    [SerializeField]
    private GameObject itemspawns;

    private gamecontroller gc;

	private float manaCount;

	public float GetManaCount () {
		return manaCount;
	}

	[SerializeField]
	private float maxMana;

	public float GetMaxMana () {
		return maxMana;
	}

	[SerializeField]
	private List<GameObject> dmTraps;

	public void SetMaxMana (int newMax) {
		maxMana = newMax;
	}

	[SerializeField]
	private float manaRate;

	public float GetManaRate () {
		return manaRate;
	}

	public void SetManaRate (float newRate) {
		manaRate = newRate;
	}


    private float manapercentage;

	private int monsterToSummon;
	private string zoneToSummon;
	private bool toBeSummoned;
	private bool trapToBeActivated;
	private bool manaLocked;
	private bool isInfiniteMana;
    
	private Vector3 trapLoc;
	private int trapType;
	private InputManager inputManager;
	private Transform heroTransform;
    [SerializeField]
	private List<Transform> monsterSpawnTransforms;

	public static float ITEM_DESPAWN_TIME = 5f;
	public static float MANA_LOCK_TIME = 3f;
	public const float INFINITE_MANA_TIME = 5f;
	public const float INFINITE_MANA_COOLDOWN = 30f;

	private float infiniteManaCounter;
	

    public bool getisinfinitemana()
    {
        return isInfiniteMana;
    }

    public float getmanapercentage()
    {
        return manapercentage;
    }




    // Use this for initialization
    void Start () {
		manaCount = 50;
		maxMana = 100;
		//every two seconds
		//InvokeRepeating ("IncrementMana", 0.2f, 0.2f);
		monsterToSummon = 0;
		toBeSummoned = false;
		trapToBeActivated = false;
		zoneToSummon = "";
		trapType = -1;
		manaLocked = false;
		infiniteManaCounter = 120;
		heroTransform = GameObject.Find("Hero").GetComponent<Transform>();
		monsterSpawnTransforms = monsterspawns.GetComponentsInChildren<Transform>().ToList();
        Debug.Log(monsterSpawnTransforms.Count);
	}
	
	// Update is called once per frame
	void Update () {

        if (gc == null)
        {
            gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<gamecontroller>();
            currentabilities = gc.getAbilities();
        }

        if (manapercentage > 0)
        {
            manapercentage -= Time.deltaTime;
        }


        float recharge_factor = 1;
        if (GameObject.FindGameObjectsWithTag("Monster").Length == 0)
        {
            recharge_factor = 3;
        }

		ChangeMana (Time.deltaTime * manaRate * recharge_factor);

		//make sure we have the input manager
		if (inputManager == null) {
			inputManager = GameObject.FindGameObjectWithTag ("InputManager").GetComponent<InputManager> ();
		}

//		// Check for monster summon by number OR for trap activation
//		if (Input.GetKeyDown (KeyCode.Alpha1)) {
//			// Queue monster 1
//			toBeSummoned = true;
//			monsterToSummon = 1;
//		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
//			// Queue monster 2
//			toBeSummoned = true;
//			monsterToSummon = 2;
//		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
//			// Queue monster 3
//			toBeSummoned = true;
//			monsterToSummon = 3;
//		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
//			// Queue monster 4
//			toBeSummoned = true;
//			monsterToSummon = 4;
//		}
//
//		// Check for summon zone
//		else if (Input.GetKeyDown ("q") && toBeSummoned) {
//			zoneToSummon = "q";
//			SummonMonster ();
//		} else if (Input.GetKeyDown ("w") && toBeSummoned) {
//			zoneToSummon = "w";
//			SummonMonster ();
//		} else if (Input.GetKeyDown ("e") && toBeSummoned) {
//			zoneToSummon = "e";
//			SummonMonster ();
//		} else if (Input.GetKeyDown ("a") && toBeSummoned) {
//			zoneToSummon = "a";
//			SummonMonster ();
//		} else if (Input.GetKeyDown ("s") && toBeSummoned) {
//			zoneToSummon = "s";
//			SummonMonster ();
//		} else if (Input.GetKeyDown ("d") && toBeSummoned) {
//			zoneToSummon = "d";
//			SummonMonster ();
//		}
//
//		// Check for trap
//		else if (Input.GetKeyDown ("t")) {
//			trapToBeActivated = true;
//		}

		monsterSpawnTransforms.Sort((p1,p2)=> Vector3.Distance(p1.position, heroTransform.position).CompareTo(Vector3.Distance(p2.position, heroTransform.position)));
		
		if (inputManager.GetDmNum () != -1) {
			toBeSummoned = true;
			monsterToSummon = inputManager.GetDmNum ();
	
			if (( (monsterToSummon < 3) && (gamecontroller.HasFlag(currentabilities, gamecontroller.DMAbilities.meleemonsters))) || ((monsterToSummon >= 3) && (gamecontroller.HasFlag(currentabilities,gamecontroller.DMAbilities.specialmonsters))))
            {
                SummonMonster();
            }
				
			
		} else if (inputManager.GetDmSpell() != -1) {
			trapToBeActivated = true;
			trapType = inputManager.GetDmSpell ();
            if (gamecontroller.HasFlag(currentabilities, gamecontroller.DMAbilities.spells))
            {
                ActivateTrap();
            }
            
		}

		// ESC key to cancel any queued actions
		else if (Input.GetKeyDown (KeyCode.Escape)) {
			CleanInput ();
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
            if (gamecontroller.HasFlag(currentabilities, gamecontroller.DMAbilities.infintemana))
            {
                infiniteMana();
            }
			
		}


        infiniteManaCounter += Time.deltaTime;
    }

	void SummonMonster () {
		int manaCost = monsterToSummon * 10;

		// Not enough mana!
		if (manaCost > manaCount && !isInfiniteMana) {
			// UI warning
			CleanInput ();
			return;
		}
		
		toBeSummoned = false;

		transform.Find("SpawnPointQ").GetComponent<MonsterSpawner>()
			.SpawnMonster(monsterToSummon, monsterSpawnTransforms[0].position);
        Debug.Log(monsterSpawnTransforms[0].position);
        if (monsterToSummon == 2)
            monsterToSummon = 0;

            transform.Find("SpawnPointQ").GetComponent<MonsterSpawner>()
    .SpawnMonster(monsterToSummon, monsterSpawnTransforms[1].position);
        Debug.Log(monsterSpawnTransforms[1].position);

        // Deduct mana
        if (isInfiniteMana) {return;}
		ChangeMana(0 - manaCost);
		CleanInput();

        gc.summontwo();
		
	}

	void ActivateTrap () {
        //Debug.Log ("Activating trap " + trapType + " " + trapLoc);
        trapLoc = heroTransform.transform.position;
		int manaCost = 55;
        

		if (manaCost > manaCount) {
			CleanInput ();
			return;
		}

		GameObject.Instantiate (dmTraps [trapType - 1], trapLoc, Quaternion.identity);
		ChangeMana (0 - manaCost);
		CleanInput ();
	}

	void CleanInput() {
		monsterToSummon = 0;
		trapType = -1;
		toBeSummoned = false;
		trapToBeActivated = false;
		zoneToSummon = "";
	}

	public void activateManaLock()
	{
		manaLocked = true;
		Invoke("deactivateManaLock", MANA_LOCK_TIME);
	}

    public bool checkInfinteMana()
    {
        if (INFINITE_MANA_COOLDOWN > infiniteManaCounter)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

	private void infiniteMana()
	{
		if (checkInfinteMana())
        {
            ChangeMana(100);
            manaLocked = true;
            isInfiniteMana = true;
            infiniteManaCounter = 0;
            Invoke("deactivateManaLock", INFINITE_MANA_TIME);
            manapercentage = INFINITE_MANA_TIME;
        }
		
	}
	
	private void deactivateManaLock()
	{
		manaLocked = false;
		isInfiniteMana = false;
	}
	
	public void ChangeMana (float amount)
	{
		if (manaLocked && amount > 0){return;}
        
		manaCount += amount;
		if (manaCount > maxMana)
			manaCount = maxMana;

	//	manaText.text = "Mana: " + manaCount.ToString() + "/" + maxMana.ToString();
	}
}
