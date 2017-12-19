using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerDropFruit : FSMNode
{
    private VillagerFSM mymanager;

    public VillagerDropFruit(FSM fsm)
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
            return new FSMNode[1] { new VillagerIdle(manager) };
        }
    }

    public override void Entry()
    {
        mymanager.villager.target = GameObject.FindGameObjectWithTag("village") ; //make sure we have no target
        mymanager.villager.DropAnim();
    }

    public override bool EntryCondition()
    {
        return (mymanager.bagFull && mymanager.atVillage);
    }

    public override void Exit()
    {
        mymanager.villager.target = null;
        return;
    }

    public override void UpdateFSMNode()
    {
        if (!mymanager.villager.animPlaying())
        {
            mymanager.apples = 0;
        }
    }
}
