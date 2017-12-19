using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerWalkToTree : FSMNode
{
    private VillagerFSM mymanager;

    public VillagerWalkToTree(FSM fsm)
    {
        manager = fsm;
    }

    public override FSMNode[] transitionNodes
    {
        get
        {
            return new FSMNode[1] { new VillagerPickFruit(manager) };
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
        int randomTree = (int)(Random.Range(0, mymanager.trees.Length));
        mymanager.villager.target = mymanager.trees[randomTree];
        SimpleTree t = mymanager.villager.target.GetComponent<SimpleTree>();
        if (t == null)
        {
            t = mymanager.villager.target.AddComponent<SimpleTree>();
        }
        mymanager.currentTree = t;
        mymanager.villager.WalkAnim();
    }

    public override bool EntryCondition()
    {
        if ((mymanager.trees.Length > 0) && !mymanager.bagFull && mymanager.villager.target == null)
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
        mymanager.villager.target = null;
        return;
    }

    public override void UpdateFSMNode()
    {
        return;
    }
}
