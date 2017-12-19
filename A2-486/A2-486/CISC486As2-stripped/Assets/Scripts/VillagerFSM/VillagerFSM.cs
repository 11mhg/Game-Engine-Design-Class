using UnityEngine;

public class VillagerFSM : FSM
{
    private FSMNode currentState;
    public bool highPriority = true;
    public int apples = 0;
    public bool bagFull { get { return apples == 3; } }
    public bool atTree { get { return villager.atTree; } }
    public bool atVillage {  get { return villager.atVillage; } }
    public Villager villager;
    public SimpleTree currentTree = null;
    public GameObject[] trees = null;
    public GameObject TargetVillage = null;

    public override FSMNode activeState {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
        }
    }

    public void Start()
    {
        currentState = new VillagerIdle(this);
        trees = GameObject.FindGameObjectsWithTag("fruitTree");
        villager = GetComponent<Villager>();
        if (highPriority)
        {
            StartFSM(Time.deltaTime);
        }
        else
        {
            StartFSM(1.0f);
        }
    }
}
