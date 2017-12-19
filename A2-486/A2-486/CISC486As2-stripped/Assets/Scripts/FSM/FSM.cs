using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour {
    public abstract FSMNode activeState { get; set; }



	// Use this for initialization
	public void StartFSM (float updateTime) {
        StartCoroutine("StateUpdate", updateTime);
	}
	
	IEnumerator StateUpdate(float yieldTime)
    {
        while (true)
        {
            activeState.UpdateFSMNode();
            FSMNode possibleTransition = activeState.CheckTransitions();
            if (possibleTransition != null)
            {
                activeState.Exit();
                activeState = possibleTransition;
                activeState.Entry();
            }
            yield return new WaitForSeconds(yieldTime);
        }
    }
}
