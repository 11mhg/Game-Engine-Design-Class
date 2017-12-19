using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerWalkToVillage : FSMNode
{
    private VillagerFSM mymanager;

    public VillagerWalkToVillage(FSM fsm)
    {
        manager = fsm;
    }

    public override FSMNode[] transitionNodes
    {
        get
        {
            return new FSMNode[1] { new VillagerDropFruit(manager) };
        }
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

    public override void Entry()
    {
        if (mymanager.TargetVillage != null)
        {
            mymanager.villager.target = mymanager.TargetVillage;
        }
        else
        {
            mymanager.villager.target = GameObject.FindGameObjectWithTag("village");
        }
        mymanager.villager.WalkAnim();
    }

    public override bool EntryCondition()
    {
        //if our bag is full (because we have 3 apples, we can keep moving
        if (mymanager.bagFull)
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
        //stops us from having a target to move to.
        return;
    }

    public override void UpdateFSMNode()
    {
        return; //skip cause moveTo in villager will take care of it.
    }
}
