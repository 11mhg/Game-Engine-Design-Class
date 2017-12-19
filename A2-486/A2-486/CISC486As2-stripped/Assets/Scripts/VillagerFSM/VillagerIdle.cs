using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerIdle : FSMNode
{

    private VillagerFSM mymanager;

    public VillagerIdle(FSM fsm)
    {
        manager = fsm;
    }

    public override FSM manager
    {
        get
        {
            return mymanager;
        }
        set
        {
            mymanager = (VillagerFSM)value;
        }
    }

    public override FSMNode[] transitionNodes
    {
        get
        {
            return new FSMNode[1] { new VillagerWalkToTree(manager) };
        }
    }

    public override void Entry()
    {

        mymanager.villager.target = null; //make sure we aren't moving anywhere.
        mymanager.villager.IdleAnim();
    }

    public override bool EntryCondition()
    {
        if (mymanager.atVillage && !mymanager.bagFull)
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
        return; //no need to do anything on exit
    }

    public override void UpdateFSMNode()
    {
        return; //no need to update (nothing to update)
    }
}
