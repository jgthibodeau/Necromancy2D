using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBoss : Enemy
{
    [System.Serializable]
    public struct AttackWeight
    {
        public Transition attackTransition;
        [Range(0,100)]
        public int weight;
        public bool canRepeat;
    }
    public AttackWeight[] attackWeights;
    private int[] weights;

    [Range(0,1)]
    public float attackChance;
    public int attacksBeforeCooldown;
    [SerializeField]
    private int currentAttackCount;
    private int lastAttack = -1;
    
    BossAllyState bossAllyState;
    BossJumpState bossJumpState;
    BossSummonState bossSummonState;
    BossLaunchState bossLaunchState;
    CoolDownState coolDownState;
    WeakState weakState;

    public GameObject lifeBar;

    private WeakSpotHealth weakSpotHealth;

    public override void MakeFSM()
    {
        if (fsm != null)
        {
            return;
        }

        fsm = new FSMSystem(initialState);

        wanderState = GetComponent<WanderState>();
        wanderState.AddTransition(Transition.LostPlayer, StateID.Wander);
        wanderState.AddTransition(Transition.SawPlayer, StateID.Chase);
        fsm.AddState(wanderState);

        chaseState = GetComponent<ChasePlayerState>();
        chaseState.AddTransition(Transition.SawPlayer, StateID.Chase);
        chaseState.AddTransition(Transition.Cooldown, StateID.CoolDown);

        chaseState.AddTransition(Transition.BossJump, StateID.BossJump);
        chaseState.AddTransition(Transition.BossSummon, StateID.BossSummon);
        chaseState.AddTransition(Transition.BossLaunch, StateID.BossLaunch);
        fsm.AddState(chaseState);

        bossJumpState = GetComponent<BossJumpState>();
        bossJumpState.AddTransition(Transition.SawPlayer, StateID.Chase);
        fsm.AddState(bossJumpState);

        bossSummonState = GetComponent<BossSummonState>();
        bossSummonState.AddTransition(Transition.SawPlayer, StateID.Chase);
        fsm.AddState(bossSummonState);

        bossLaunchState = GetComponent<BossLaunchState>();
        bossLaunchState.AddTransition(Transition.SawPlayer, StateID.Chase);
        fsm.AddState(bossLaunchState);

        coolDownState = GetComponent<CoolDownState>();
        coolDownState.AddTransition(Transition.CooldownFinished, StateID.Chase);
        fsm.AddState(coolDownState);

        weakState = GetComponent<WeakState>();
        weakState.AddTransition(Transition.CooldownFinished, StateID.Chase);
        fsm.AddState(weakState);


        bossAllyState = GetComponent<BossAllyState>();
        bossAllyState.AddTransition(Transition.Launch, StateID.Launched);
        bossAllyState.AddTransition(Transition.Resurrect, StateID.Ally);
        fsm.AddState(bossAllyState);

        deadState = GetComponent<DeadState>();
        deadState.AddTransition(Transition.Resurrect, StateID.Ally);
        fsm.AddState(deadState);

        fsm.AddTransitionToAllStates(Transition.Dead, StateID.Dead);
        fsm.AddTransitionToAllStates(Transition.Weakened, StateID.Weak);

        Debug.Log("current state " + fsm.CurrentStateID);
    }

    public override void Start()
    {
        base.Start();

        weakSpotHealth = GetComponent<WeakSpotHealth>();

        lifeBar.SetActive(false);

        weights = new int[attackWeights.Length];
        for (int i = 0; i < attackWeights.Length; i++)
        {
            weights[i] = attackWeights[i].weight;
        }
    }

    public void Update()
    {
        if (alerted && !lifeBar.activeSelf)
        {
            lifeBar.SetActive(true);
        }
        if (!alerted && lifeBar.activeSelf)
        {
            lifeBar.SetActive(false);
        }

        if (fsm.CurrentStateID == StateID.Chase && currentAttackCount >= attacksBeforeCooldown)
        {
            //TODO switch to cooldown
            currentAttackCount = 0;
        }

        if (fsm.CurrentStateID != StateID.Weak && fsm.CurrentStateID != StateID.Dead && weakSpotHealth.IsWeak())
        {
            SetTransition(Transition.Weakened);
        }
    }

    public override void Attack()
    {
        if (Random.value <= attackChance)
        {
            int attackIndex;
            AttackWeight chosenAttack;
            do
            {
                attackIndex = Util.GetRandomWeightedIndex(weights);
                chosenAttack = attackWeights[attackIndex];
            }
            while (!chosenAttack.canRepeat && attackIndex == lastAttack);

            Debug.Log("Chose attack " + chosenAttack.attackTransition);

            SetTransition(chosenAttack.attackTransition);

            currentAttackCount++;
        }
    }

    public override void PreJump()
    {
        base.PreJump();
        bossJumpState.PreJump();
    }

    public override void Jump()
    {
        base.Jump();
        bossJumpState.Jump();
    }

    public override void Land()
    {
        base.Land();
        bossJumpState.Land();
    }

    public override void StopLand()
    {
        base.Land();
        bossJumpState.StopLand();
    }
}
