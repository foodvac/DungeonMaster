﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour {

    private InputManager inputmanager;

    private SpriteRenderer sr;
    [SerializeField]
    private Sprite up;
    [SerializeField]
    private Sprite down;
    [SerializeField]
    private Sprite left;


    [SerializeField]
    private float mspeed = 0;
    [SerializeField]
    private float walkspeed = 3;
    [SerializeField]
    private float runspeed = 8;


    [SerializeField]
    private BasicSword weapon;

    private Vector2 inputdirection;

    private bool canmove = true;


    private bool isdashing = false;
    private float dashcount;
    private Vector2 dashdirection = Vector2.down;
    [SerializeField]
    private float dashtime = .25f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }


 
    void Update () {
        //make sure we have the input manager
        if (inputmanager == null)
        {
            inputmanager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        }


        //handle in-process dashing
        if (isdashing)
        {
            mspeed = runspeed;
            transform.Translate(dashdirection * Time.deltaTime * mspeed);
            dashcount -= Time.deltaTime;
            if (dashcount <= 0)
            {
                isdashing = false;
                canmove = true;
            }
        }

        //handle regular movement  + beginning dashing
        if (canmove)
        {
            //get input direction
            inputdirection = inputmanager.GetHeroMovement();
            if (inputdirection != Vector2.zero)
            {
                //change sprite based on direction
                if (Mathf.Abs(inputdirection.x) >= Mathf.Abs(inputdirection.y))
                {
                    //facing right
                    if (inputdirection.x > 0)
                    {
                        sr.sprite = left;
                        sr.flipX = true;
                        dashdirection = Vector2.right;
                    }
                    else //facing left
                    {
                        sr.sprite = left;
                        sr.flipX = false;
                        dashdirection = -Vector2.right;
                    }
                }
                else
                {
                    //facing up
                    if (inputdirection.y > 0)
                    {
                        sr.sprite = up;
                        sr.flipX = false;
                        dashdirection = Vector2.up;
                    }
                    else //facing down
                    {
                        sr.sprite = down;
                        sr.flipX = false;
                        dashdirection = -Vector2.up;
                    }
                }
            }
            
            //handle dash
            if (inputmanager.GetHeroDash())
            {
              
                dashcount = dashtime;
                isdashing = true;
                canmove = false;
            }
            else
            {
                //regular move
                if (inputdirection != Vector2.zero)
                {

                    //move player
                    mspeed = walkspeed;
                    transform.Translate(inputdirection.normalized * Time.deltaTime * mspeed);

                }
            }


            //handle attacking
            if (inputmanager.GetHeroAttack())
            {
                if (!weapon.checkAttacking())
                {
                    weapon.doBasicAttack(dashdirection/2);
                }
            }
            
        }
       





	}
}
