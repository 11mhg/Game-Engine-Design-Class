using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerPickFruit : FSMNode
{
    private VillagerFSM mymanager;

    public VillagerPickFruit(FSM fsm)
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
            return new FSMNode[1] {new VillagerWalkToVillage(manager) };
        }
    }

    public override void Entry()
    {
        mymanager.villager.PickAnim();
    }

    public override bool EntryCondition()
    {
        if (mymanager.atTree && mymanager.currentTree.apples > 0 && !mymanager.bagFull)
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
        mymanager.currentTree.apples = 0;
        mymanager.currentTree = null;
        mymanager.villager.target = null;
        return;
    }

    public override void UpdateFSMNode()
    {
        if (!mymanager.villager.animPlaying())
        {
            mymanager.apples += 3;
        }
        return; //playing anim
    }
}
