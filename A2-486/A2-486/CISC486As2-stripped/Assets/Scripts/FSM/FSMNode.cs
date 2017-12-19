using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class FSMNode {
    public abstract FSM manager { get; set; }
    public abstract FSMNode[] transitionNodes { get;}
    public abstract void Entry();
    public abstract void Exit();
    public abstract void UpdateFSMNode();
    public abstract bool EntryCondition();
    public FSMNode CheckTransitions()
    {
        if (transitionNodes != null)
        {
            foreach (FSMNode f in transitionNodes)
            {
                if (f.EntryCondition())
                {
                    return f;
                }
            }
        }
        return null;
    }	
}
