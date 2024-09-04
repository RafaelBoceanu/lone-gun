using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    [RequireComponent(typeof(AlertBehaviorMain))]
    [RequireComponent(typeof(AttackBehavior))]
    [RequireComponent(typeof(ChaseBehavior))]
    [RequireComponent(typeof(CommonBehavior))]
    [RequireComponent(typeof(SearchBehavior))]
    [RequireComponent(typeof(AlliesBehavior))]
    [RequireComponent(typeof(RetreatBehavior))]
    [RequireComponent(typeof(PointsOfInterestBehavior))]
    public class EnemyAI : MonoBehaviour
    {
        public CharacterStats target;
        public float sightDistance = 20;
        public bool goToPos;
        public Vector3 lastKnownPosition;
        public Vector3 pointOfInterest;
        public float alertTimer;
        public int alertTimerIncrement;
        public float alertMultiplier;
        public int healthDecreaseOnAttack;

        public bool onPatrol;
        public bool canChase;
        public List<EnemyAI> AlliesNear = new List<EnemyAI>();
        public List<POI_Base> PointsOfInterestList = new List<POI_Base>();

        bool updateAllies;

        // state
        public AIstates aiStates;

        public enum AIstates
        {
            patrol,
            chase,
            alert,
            onAlertBehaviours,
            hasTarget,
            search,
            deciding,
            cover,
            attack,
            retreat,
            dead
        }

        // components
        public PlayerControl playerControl;
        //NavMeshAgent agent;
        public CharacterStats charStats;
        EnemiesManager enManager;

        [HideInInspector]
        public AlertBehaviorMain alertBehavior;
        [HideInInspector]
        public AttackBehavior attackBehavior;
        [HideInInspector]
        public ChaseBehavior chaseBehavior;
        [HideInInspector]
        public CommonBehavior commonBehavior;
        [HideInInspector]
        public SearchBehavior searchBehavior;
        [HideInInspector]
        public AlliesBehavior alliesBehavior;
        [HideInInspector]
        public RetreatBehavior retreatBehavior;
        [HideInInspector]
        public PointsOfInterestBehavior poiBehavior;


        // Start is called before the first frame update
        void Start()
        {
            playerControl = GetComponent<PlayerControl>();
            charStats = GetComponent<CharacterStats>();
            charStats.alert = false;

            enManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemiesManager>();
            enManager.AllEnemies.Add(charStats);

            alertBehavior = GetComponent<AlertBehaviorMain>();
            attackBehavior = GetComponent<AttackBehavior>();
            chaseBehavior = GetComponent<ChaseBehavior>();
            commonBehavior = GetComponent<CommonBehavior>();
            searchBehavior = GetComponent<SearchBehavior>();
            alliesBehavior = GetComponent<AlliesBehavior>();
            retreatBehavior = GetComponent<RetreatBehavior>();
            poiBehavior = GetComponent<PointsOfInterestBehavior>();

            if (onPatrol)
            {
                canChase = true;
                enManager.EnemiesOnPatrol.Add(charStats);
            }

            if (canChase)
            {
                enManager.EnemiesAvailableToChase.Add(charStats);
            }

            sightDistance = GetComponentInChildren<EnemySightSphere>().GetComponent<SphereCollider>().radius;
        }

        // Update is called once per frame
        void Update()
        {
            switch (aiStates)
            {
                case AIstates.patrol:
                    alertMultiplier = 1;
                    commonBehavior.DecreaseAlertLevels();
                    commonBehavior.PatrolBehaviour();
                    TargetAvailable();
                    poiBehavior.POIBehavior();
                    break;
                case AIstates.alert:
                    TargetAvailable();
                    alertBehavior.AlertBehaviourMain();
                    break;
                case AIstates.onAlertBehaviours:
                    TargetAvailable();
                    alertBehavior.OnAlertExtraBehaviours();
                    break;
                case AIstates.chase:
                    TargetAvailable();
                    chaseBehavior.ChaseBehaviour();
                    break;
                case AIstates.search:
                    alertMultiplier = .3f;
                    TargetAvailable();
                    commonBehavior.DecreaseAlertLevels();
                    searchBehavior.SearchBehaviour();
                    break;
                case AIstates.hasTarget:
                    attackBehavior.HasTargetBehaviour();
                    break;
                case AIstates.cover:
                    attackBehavior.CoverBehaviour();
                    break;
                case AIstates.deciding:
                    attackBehavior.DecideAttackBehaviorByStats();
                    break;
                case AIstates.attack:
                    if (!charStats.hasCover)                  
                        attackBehavior.AttackBehaviour();
                    else 
                        attackBehavior.AttackFromCover();
                    break;
                case AIstates.retreat:
                    retreatBehavior.RetreatAction();
                    charStats.run = true;
                    charStats.aim = false;
                    break;
            }

            if (aiStates != AIstates.cover && aiStates != AIstates.attack)
            {
                if (attackBehavior.currentCoverPosition != null)
                    attackBehavior.currentCoverPosition.occupied = false;
            }
        }

        public void ChangeAIBehaviour(string behaviour, float delay)
        {
            Invoke(behaviour, delay);
        }

        public void AI_State_Normal()
        {
            aiStates = AIstates.patrol;
            target = null;
            charStats.alert = false;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
            charStats.hasCover = false;
        }

        public void AI_State_Chase()
        {
            aiStates = AIstates.chase;
            charStats.hasCover = false;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
        }

        public void AI_State_Search()
        {
            aiStates = AIstates.search;
            target = null;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
            charStats.hasCover = false;
        }

        public void AI_State_HasTarget()
        {
            aiStates = AIstates.hasTarget;
            charStats.alert = true;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
            charStats.hasCover = false;
        }

        public void AI_State_Attack()
        {
            alliesBehavior.AlertAllies();
            aiStates = AIstates.attack;
        }

        public void AI_State_OnAlert_RunListOfBehaviours()
        {
            aiStates = AIstates.onAlertBehaviours;
            charStats.run = true;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
            charStats.hasCover = false;
        }

        public void AI_State_Cover()
        {
            aiStates = AIstates.cover;
            charStats.run = true;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
            attackBehavior.findCoverPositions = false;
        }

        public void AI_State_Retreat()
        {
            aiStates = AIstates.retreat;
            charStats.crouch = false;
            charStats.aim = false;
            charStats.hasCover = false;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
            attackBehavior.findCoverPositions = false;
        }

        public void AI_State_DecideByStats()
        {
            aiStates = AIstates.deciding;
            charStats.run = false;
            goToPos = false;
            alertBehavior.lookAtPOI = false;
            commonBehavior._initCheck = false;
            charStats.hasCover = false;
        }

        public void GoOnAlert(Vector3 poi)
        {
            pointOfInterest = poi;
            aiStates = AIstates.alert;
            alertBehavior.lookAtPOI = false;
        }

        void TargetAvailable()
        {
            if (target)
            {
                if (sightRaycasts())
                {
                    ChangeAIBehaviour("AI_State_HasTarget", 0);
                }
            }
        }

        Quaternion targetRot;

        public void LookAtTarget(Vector3 positionToLook)
        {
            Vector3 directionToLookTo = positionToLook - transform.position;
            directionToLookTo.y = 0;

            float angle = Vector3.Angle(transform.forward, directionToLookTo);

            if (angle > .1f)
            {
                targetRot = Quaternion.LookRotation(directionToLookTo);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 3);
            }
        }

        public bool sightRaycasts()
        {
            bool retVal = false;
            RaycastHit hitTowardsLowerBody;
            RaycastHit hitTowardsUpperBody;
            float raycastDistance = sightDistance + (sightDistance * .5f);
            Vector3 targetPosition = lastKnownPosition;

            if (target)
            {
                targetPosition = target.transform.position;
            }

            Vector3 raycastStart = transform.position + new Vector3(0, 1.6f, 0);
            Vector3 direction = targetPosition - raycastStart;

            LayerMask excludeLayers = ~((1 << 7) | (1 << 8) | (1 << 2));

            Debug.DrawRay(raycastStart, direction + new Vector3(0, 1, 0));
            if (Physics.Raycast(raycastStart, direction + new Vector3(0, 1, 0), out hitTowardsLowerBody, raycastDistance, excludeLayers))
            {
                if (hitTowardsLowerBody.transform.GetComponent<CharacterStats>())
                {
                    if (target)
                    {
                        if (hitTowardsLowerBody.transform.GetComponent<CharacterStats>() == target)
                        {
                            retVal = true;
                        }
                    }
                }
            }

            if (retVal == false)
            {
                direction += new Vector3(0, 1.6f, 0);

                if (Physics.Raycast(raycastStart, direction, out hitTowardsUpperBody, raycastDistance, excludeLayers))
                {
                    if (target)
                    {
                        if (hitTowardsUpperBody.transform == target.transform)
                        {
                            if (!target.crouch)
                            {
                                retVal = true;
                            }
                        }
                    }
                }
            }

            if (retVal)
            {
                lastKnownPosition = target.transform.position;
            }

            return retVal;
        }
    }

    [System.Serializable]
    public struct WaypointsBase
    {
        public Transform targetDestination;
        public float waitTime;
        public bool lookTowards;
        public Transform targetToLookTo;
        public float speedToLook;
        public bool overrideAnimation;
        public string[] animationRoutines;
        public bool stopList;
    }
}
