using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

    public GameObject ToSpawn;

    public GameObject target1;
    public GameObject target2;
    private GameObject curTarget;
    public float speed = 1.0f;
    private float acceptablerange = 0.5f;

	// Use this for initialization
	void Start () {
        StartCoroutine("Shoot", 1.0f);
        curTarget = target1;
	}

    void Update()
    {
        if (Vector3.Distance(curTarget.transform.position,transform.position) <= acceptablerange){
            if (curTarget == target1)
            {
                curTarget = target2;
            }
            else
            {
                curTarget = target1;
            }
        }
        transform.position = transform.position + (curTarget.transform.position - transform.position) * Time.deltaTime/2;
        
    }
	
	private IEnumerator Shoot(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            GameObject g = Instantiate(ToSpawn, transform.position + new Vector3(0,-1f,0), Quaternion.identity);
            g.GetComponent<PPhysicsBody>().Velocity = new Vector2(0, 100);
            yield return new WaitForSeconds(waitTime);
            
        }
    }
}
