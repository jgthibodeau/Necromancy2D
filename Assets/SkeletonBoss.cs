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
    }
    public AttackWeight[] attackWeights;
    private Transition[] attackTransitions;
    private int[] weights;

    [Range(0,1)]
    public float attackChance;
    public int attacksBeforeCooldown;
    [SerializeField]
    private int currentAttackCount;
    
    BossAllyState bossAllyState;
    BossJumpState bossJumpState;
    BossSummonState bossSummonState;
    BossLaunchState bossLaunchState;
    CoolDownState coolDownState;
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

        chaseState.AddTransition(Transition.BossJump, StateID.BossJump);
        chaseState.AddTransition(Transition.BossSummon, StateID.BossSummon);
        chaseState.AddTransition(Transition.BossLaunch, StateID.BossLaunch);
        fsm.AddState(chaseState);

        bossJumpState = GetComponent<BossJumpState>();
        bossJumpState.AddTransition(Transition.SawPlayer, StateID.Chase);
        bossJumpState.AddTransition(Transition.Cooldown, StateID.CoolDown);
        fsm.AddState(bossJumpState);

        bossSummonState = GetComponent<BossSummonState>();
        bossSummonState.AddTransition(Transition.SawPlayer, StateID.Chase);
        bossSummonState.AddTransition(Transition.Cooldown, StateID.CoolDown);
        fsm.AddState(bossSummonState);

        bossLaunchState = GetComponent<BossLaunchState>();
        bossLaunchState.AddTransition(Transition.SawPlayer, StateID.Chase);
        bossLaunchState.AddTransition(Transition.Cooldown, StateID.CoolDown);
        fsm.AddState(bossLaunchState);

        coolDownState = GetComponent<CoolDownState>();
        coolDownState.AddTransition(Transition.CooldownFinished, StateID.Chase);
        fsm.AddState(coolDownState);

        
        bossAllyState = GetComponent<BossAllyState>();
        bossAllyState.AddTransition(Transition.Launch, StateID.Launched);
        bossAllyState.AddTransition(Transition.Resurrect, StateID.Ally);
        fsm.AddState(bossAllyState);

        deadState = GetComponent<DeadState>();
        deadState.AddTransition(Transition.Resurrect, StateID.Ally);
        fsm.AddState(deadState);

        fsm.AddTransitionToAllStates(Transition.Dead, StateID.Dead);

        Debug.Log("current state " + fsm.CurrentStateID);
    }

    public override void Start()
    {
        base.Start();

        attackTransitions = new Transition[attackWeights.Length];
        weights = new int[attackWeights.Length];
        for (int i = 0; i < attackWeights.Length; i++)
        {
            attackTransitions[i] = attackWeights[i].attackTransition;
            weights[i] = attackWeights[i].weight;
        }
    }

    public void Update()
    {
        if (fsm.CurrentStateID == StateID.Chase && currentAttackCount >= attacksBeforeCooldown)
        {
            //TODO switch to cooldown
            currentAttackCount = 0;
        }
    }

    public override void Attack()
    {
        if (Random.value <= attackChance)
        {
            int chosenAttack = Util.GetRandomWeightedIndex(weights);
            Transition chosenTransition = attackTransitions[chosenAttack];
            Debug.Log("Chose attack " + chosenTransition);

            SetTransition(chosenTransition);

            currentAttackCount++;
        }
    }
}
