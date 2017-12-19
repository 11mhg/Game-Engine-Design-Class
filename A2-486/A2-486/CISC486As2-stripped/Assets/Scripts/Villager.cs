using UnityEngine;
using System.Collections;

/***
 * 
 * CISC 486 Assignment 2 sample code. Copyright (C) Nicholas Graham.
 * 
 * This example code is provided to show how to interact with the animation controller
 * and how to move the avatar. This code should be replaced with your own FSM controller
 * code.
 * 
 */

public class Villager : MonoBehaviour {

	public float speed = 5.0f;
	public float pickingDistance = 5.0f;

	public Animator _anim;
	
	// Triggers used to change state
	private int walkHash = Animator.StringToHash ("Walk");
	private int idleHash = Animator.StringToHash ("Idle");
	private int dropHash = Animator.StringToHash ("Drop");
	private int pickHash = Animator.StringToHash ("Pickup");

	// Names of animation states
	private int idleStateHash = Animator.StringToHash ("idle");
	private int walkingStateHash = Animator.StringToHash ("walking");
	private int dropStateHash = Animator.StringToHash ("dropping");
	private int pickupStateHash = Animator.StringToHash ("pickingFruit");

    public int animState;
    private bool isPlaying = false;


	public bool atTree = false;
    public bool atVillage = true;
	public GameObject target = null;

    public bool animPlaying()
    {
        return isPlaying;
    }

	// Use this for initialization
	void Awake () {
		_anim = GetComponent<Animator>();
		_anim.SetTrigger(idleHash);
	}

    IEnumerator AnimationDoneFlag(float yieldTime)
    {
        yield return new WaitForSecondsRealtime(yieldTime);
        isPlaying = false;
    }

    public void WalkAnim()
    {
        _anim.SetTrigger(walkHash);
        animState = walkHash;
        isPlaying = true;
    }

    public void PickAnim()
    {
        _anim.SetTrigger(pickHash);
        animState = pickHash;
        StartCoroutine("AnimationDoneFlag", 8.0f);
    }

    public void DropAnim()
    {
        _anim.SetTrigger(dropHash);
        animState = dropHash;
        StartCoroutine("AnimationDoneFlag", 8.767f);
    }

    public void IdleAnim()
    {
        _anim.SetTrigger(idleHash);
        animState = idleHash;
    }

    public void MoveTo()
    {
        transform.LookAt(target.transform.position);
        if ((transform.position-target.transform.position).magnitude <= pickingDistance)
        {
            if (target.CompareTag("fruitTree"))
            {
                atTree = true;
                atVillage = false;
            }
            if (target.CompareTag("village"))
            {
                atTree = false;
                atVillage = true;
            }
            target = null;
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.position =
                new Vector3(
                    transform.position.x,
                    Terrain.activeTerrain.SampleHeight(transform.position),
                    transform.position.z);
        }
    }


	// Update is called once per frame
	void Update () {
		if(target != null)
        {
            MoveTo();
        }
	}
}
