﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

abstract public class Monster : MonoBehaviour {

    [SerializeField]
    protected float health = 20;

    [SerializeField]
    protected bool isSeeking = false;

    [SerializeField]
    protected float movementSpeed = 2;
    [SerializeField]
    protected float attackPower = 5;
    protected GameObject hero;
    [SerializeField]
    protected float stunDuration = .25f;
    [SerializeField]
    protected bool isStunned = false;

    [SerializeField]
    protected Vector2 direction;
    protected Vector2 stundirection;

    [SerializeField] 
    private GameObject[] dropableItems;

    [SerializeField]
    protected AudioClip spawnSound;

    [SerializeField]
    protected AudioClip deathSound;

    [SerializeField]
    protected AudioClip hitSound1;

    [SerializeField]
    protected AudioClip hitSound2;

    private Vector3 lastHitDirection;

    private SpriteRenderer sr1;

    

    // Update is called once per frame
    public void Damage(float attack, Vector2 hitdirection)
    {
        //Debug.Log("DANK MEMES");
        if(isSeeking)
            gameObject.GetComponent<AIPath>().canMove = false;
        stundirection = hitdirection;
        lastHitDirection = hitdirection;
        isStunned = true;
        health -= attack;
        if (Random.Range(0.0f, 1.0f) <= 0.5)
        {
            GetComponent<AudioSource>().PlayOneShot(hitSound1);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(hitSound2);
        }
        GetComponent<AudioSource>().PlayOneShot(hitSound1);
        
    }

    protected IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    public void Debuff(string field, float modifier ,float duration)
    {
        if (field.Equals("Slow"))
        {
            StartCoroutine(Restore(field, modifier, duration));
            movementSpeed *= modifier;
        }
    }

    protected IEnumerator Restore(string field, float modifier, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (field.Equals("Slow"))
        {
            movementSpeed /= modifier;
        }
    }

    protected void DropItem()
    {
        System.Random random = new System.Random();
        float index = random.Next(0, 11);
        var location = transform.position + lastHitDirection;
        if (index == 3)
        {
            Instantiate(dropableItems[0], location, Quaternion.identity);
            return;
        }
        if (index == 6)
        {
            Instantiate(dropableItems[1], location, Quaternion.identity);
            return;
        }
    }
    
    protected virtual void Move()
    {
        transform.Translate(direction.normalized * Time.deltaTime * movementSpeed);
    }

    protected virtual void Update()
    {

        if (sr1 == null)
            sr1 = GetComponentInChildren<SpriteRenderer>();
        if (health <= 0)
        {
            DropItem();
            GetComponent<AudioSource>().PlayOneShot(deathSound);
            StartCoroutine(DestroyThis());
            health = 100;
        }
        if (isStunned)// stunned knockback
        {
            transform.Translate(stundirection * Time.deltaTime * (movementSpeed + 3));
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0)
            {
                stunDuration = .25f;
                isStunned = false;
                if(isSeeking)
                    gameObject.GetComponent<AIPath>().canMove = true;
            }
        }
        else if (!isSeeking)// regular movement
        {
            Move();
        }

        if (isSeeking)
        {
            Vector2 movement = GetComponent<Rigidbody2D>().velocity;
            Debug.Log(movement.x);
            if (movement.x >= 0)
            {
                sr1.flipX = false;
            }
            else
            {
                sr1.flipX = true;
            }
        }
        
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Hero"))
        {
            Vector2 pushdirection = other.gameObject.transform.position - transform.position;
            pushdirection.Normalize();
            other.gameObject.GetComponent<HeroController>().damage(attackPower, pushdirection);
        }
    }
}
