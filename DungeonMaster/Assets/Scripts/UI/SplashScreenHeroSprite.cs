using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreenHeroSprite : MonoBehaviour {

	float timeToWalk = 2.35f;
	Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (timeToWalk > 0) {
			transform.Translate (22 * Time.deltaTime * Vector3.right);
			timeToWalk -= Time.deltaTime;
		} else {
			animator.SetTrigger("onArrival");
		}
	}
}
