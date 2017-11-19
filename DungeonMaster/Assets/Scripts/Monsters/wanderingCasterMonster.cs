﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wanderingCasterMonster : wanderingMonster {
    
    private new float movementSpeed = 1.25f;

    private float castingCD = 4;
    private bool canCast = false;
    private float castingTime = 2;
	// Use this for initialization
	void Start () {
        hero = GameObject.FindWithTag("Hero");
        direction = hero.transform.position - transform.position;
        Invoke("ChangeDirection", Random.Range(2, 5));
    }

    // Update is called once per frame

    protected override void Move()
    {
        transform.Translate(direction.normalized * Time.deltaTime * movementSpeed);
    }

    protected void CastSpell()
    {
        // instantiate the aoe spell
    }

    protected override void Update()
    {
        if (isStunned)// stunned knockback
        {
            transform.Translate(stundirection * Time.deltaTime * (movementSpeed + 3));
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0)
            {
                stunDuration = .25f;
                isStunned = false;
            }
        }
        else // regular movement
        {
            if (castingCD <= 0) // Can cast a spell
            {
                if (castingTime <= 0) // CAST THE SPELL
                {
                    CastSpell();
                    castingTime = 2;
                    castingCD = 4;
                }
                else
                {
                    castingTime -= Time.deltaTime; // casting the spell
                }
            }
            else
            {
                castingCD -= Time.deltaTime;
                Move();
            }
            
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hero"))
        {
            Vector2 pushdirection = other.gameObject.transform.position - transform.position;
            pushdirection.Normalize();
            other.gameObject.GetComponent<HeroController>().damage(attackPower, pushdirection * (5/4));
        }
    }
}