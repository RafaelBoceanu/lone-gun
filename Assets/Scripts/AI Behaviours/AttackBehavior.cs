using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class AttackBehavior : MonoBehaviour
    {
        // attacking variables
        public Animator modelAnim;
        public bool startShooting;
        public float attackRate = 1.5f;
        public float shootRate = 1.0f;
        float shootG;
        float attackG;
        public ParticleSystem muzzleFX;
        public AudioSource audioSource;
        EnemyAI enAI_main;

        public float delayTillNewBehaviour = 3;
        float _timerTillNewBehaviour;

        // Start is called before the first frame update
        void Start()
        {
            enAI_main = GetComponent<EnemyAI>();
            audioSource = GetComponentInChildren<AudioSource>();
        }

        public void HasTargetBehaviour()
        {
            enAI_main.charStats.StopMoving();

            if (enAI_main.sightRaycasts())
            {
                if (enAI_main.charStats.alertLevel < 10)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, enAI_main.target.transform.position);
                    float multiplier = 1 + (distanceToTarget * .1f);
                    //How fast it recognises it's an enemy is based on distance

                    enAI_main.alertTimer += Time.deltaTime * multiplier;

                    if (enAI_main.alertTimer > enAI_main.alertTimerIncrement)
                    {
                        enAI_main.charStats.alertLevel++;
                        enAI_main.alertTimer = 0;
                    }
                }
                else
                {
                    enAI_main.AI_State_DecideByStats();
                }

                enAI_main.LookAtTarget(enAI_main.lastKnownPosition);
            }
            else
            {
                if (enAI_main.charStats.alertLevel > 5)
                {
                    enAI_main.AI_State_Chase();
                    enAI_main.pointOfInterest = enAI_main.lastKnownPosition;
                }
                else
                {
                    _timerTillNewBehaviour += Time.deltaTime;

                    if (_timerTillNewBehaviour > delayTillNewBehaviour)
                    {
                        enAI_main.AI_State_Normal();
                        _timerTillNewBehaviour = 0;
                    }
                }
            }
        }

        public void DecideAttackBehaviorByStats()
        {
            bool supPass = supressionPass();
            bool morPass = moralePass();

            Debug.Log("supression " + supPass + "morale " + morPass);

            if (supPass && morPass)
            {
                enAI_main.AI_State_Attack();
            }
            else
            {
                if (!supPass)
                {
                    enAI_main.AI_State_Cover();
                }
            }
        }

        bool moralePass()
        {
            int ranValue = Random.Range(0, 101);

            //TODO: add modifiers
            int health = Mathf.RoundToInt(enAI_main.charStats.health / 10);

            int friendlies = 0;

            if (enAI_main.AlliesNear.Count > 0)
            {
                friendlies = 10;

                for (int i = 0; i < enAI_main.AlliesNear.Count; i++)
                {
                    if (enAI_main.AlliesNear[i].charStats.unitRank > enAI_main.charStats.unitRank)
                    {
                        friendlies += 10;
                    }
                }
            }

            int modifiers = health + friendlies;

            ranValue -= modifiers;

            if (ranValue > enAI_main.charStats.morale)
                return false;
            else
                return true;
        }

        bool supressionPass()
        {
            int ranValue = Random.Range(0, 101);

            int enemyAiming = 0;

            if (enAI_main.target.aim)
            {
                enemyAiming = 10;
            }

            int modifiers = enemyAiming;

            ranValue += modifiers;

            if (ranValue < enAI_main.charStats.supressionLevel)
                return false;
            else
                return true;
        }

        public void AttackBehaviour()
        {
            if (!startShooting)
            {
                if (enAI_main.sightRaycasts())
                {
                    enAI_main.LookAtTarget(enAI_main.lastKnownPosition);

                    enAI_main.charStats.aim = true;

                    attackG += Time.deltaTime;

                    if (attackG > attackRate)
                    {
                        startShooting = true;
                        timesShot = 0;
                        timesToShoot = 1;
                        attackG = 0;
                    }
                }
                else
                {
                    enAI_main.charStats.aim = false;
                    enAI_main.AI_State_Chase();
                }
            }
            else
            {
                ShootingBehaviour();
            }
        }

        public void AttackFromCover()
        {
            if (!startShooting)
            {
                enAI_main.LookAtTarget(enAI_main.lastKnownPosition);
                enAI_main.charStats.run = false;
                enAI_main.charStats.crouch = true;

                float attackRatePenalty = 0;

                attackRatePenalty = enAI_main.charStats.supressionLevel * .01f;

                attackG += Time.deltaTime;

                if (attackG > attackRate + attackRatePenalty)
                {
                    ReEvaluateCover();

                    if (validCover)
                    {
                        enAI_main.charStats.supressionLevel -= 10;

                        if (enAI_main.charStats.supressionLevel < 0)
                            enAI_main.charStats.supressionLevel = 0;

                        if (supressionPass())
                        {
                            enAI_main.charStats.crouch = false;
                            startShooting = true;
                            timesShot = 0;
                            timesToShoot = Random.Range(1, 5);
                            _delayAnim = 0;
                        }
                    }
                    else
                    {
                        enAI_main.charStats.aim = false;
                        findCoverPositions = false;
                        _curTries = 0;
                        currentCoverPosition.occupied = false;
                        enAI_main.AI_State_Cover();
                    }

                    attackG = 0;
                }
            }
            else
            {
                enAI_main.LookAtTarget(enAI_main.lastKnownPosition);
                enAI_main.charStats.aim = true;

                _delayAnim += Time.deltaTime;

                if (_delayAnim > 1)
                {
                    if (enAI_main.sightRaycasts())
                    {
                        ShootingBehaviour();
                    }
                    else
                    {
                        startShooting = false;
                        enAI_main.charStats.aim = false;
                        attackG = 0;
                        timesShot = 0;
                        enAI_main.AI_State_Chase();
                    }
                }
            }
        }

        void ReEvaluateCover()
        {
            Vector3 targetPosition = enAI_main.lastKnownPosition;
            Transform validatePosition = currentCoverPosition.positionObject.parent.parent.transform;

            Vector3 directionOfTarget = targetPosition - validatePosition.position;
            Vector3 coverForward = validatePosition.TransformDirection(Vector3.forward);

            if (Vector3.Dot(coverForward, directionOfTarget) > 0)
            {
                if (currentCoverPosition.backPos)
                    validCover = false;
                else
                    validCover = true;
            }
            else
            {
                if (currentCoverPosition.backPos)
                    validCover = true;
                else
                    validCover = false;
            }
        }

        float _delayAnim;
        bool validCover;
        int timesShot;
        int timesToShoot;
        bool onAimingAnimation;

        public void ShootingBehaviour()
        {
            if (timesShot < timesToShoot)
            {
                shootG += Time.deltaTime;

                if (shootG > shootRate)
                {
                    muzzleFX.Emit(1);
                    audioSource.Play();
                    enAI_main.target.health -= 10;
                    enAI_main.charStats.shooting = true;
                    timesShot++;
                    shootG = 0;

                    if (timesShot == timesToShoot - 1)
                    {
                        enAI_main.alliesBehavior.AlertEveryoneInsideRange(5);
                    }
                }
            }
            else
            {
                startShooting = false;
            }
        }

        public bool findCoverPositions;
        public List<Transform> coverPositions = new List<Transform>();
        public List<Transform> ignorePositions = new List<Transform>();
        private ObjectDistanceComparer objectDistanceComparer;
        public CoverBase currentCoverPosition;
        public int maxTries = 3;
        public int _curTries;

        public void CoverBehaviour()
        {
            if (!findCoverPositions)
            {
                FindCover();
            }
            else
            {
                enAI_main.charStats.MoveToPosition(currentCoverPosition.positionObject.position);
                enAI_main.charStats.run = true;

                float distance = Vector3.Distance(transform.position, currentCoverPosition.positionObject.position);

                if (distance < 1)
                {
                    enAI_main.charStats.hasCover = true;
                    enAI_main.charStats.StopMoving();
                    enAI_main.AI_State_Attack();
                }
            }
        }

        void FindCover()
        {
            if (_curTries <= maxTries)
            {
                if (!findCoverPositions)
                {
                    findCoverPositions = true;
                    _curTries++;

                    CoverBase targetCoverPosition = null;
                    float distanceToTarget = Vector3.Distance(transform.position, enAI_main.target.transform.position);

                    coverPositions.Clear();

                    Vector3 targetPosition = enAI_main.target.transform.position;

                    Collider[] colliders = Physics.OverlapSphere(transform.position, 20);

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].GetComponent<CoverPositions>())
                        {
                            if (!ignorePositions.Contains(colliders[i].transform))
                            {
                                float distanceToCandidate = Vector3.Distance(transform.position, colliders[i].transform.position);

                                Debug.Log(distanceToCandidate + " " + distanceToTarget);

                                if (distanceToCandidate < distanceToTarget)
                                {
                                    coverPositions.Add(colliders[i].transform);
                                }
                            }
                        }
                    }

                    if (coverPositions.Count > 0)
                    {
                        SortPosition(coverPositions);

                        CoverPositions validatePosition = coverPositions[0].GetComponent<CoverPositions>();

                        Vector3 directionOfTarget = targetPosition - validatePosition.transform.position;
                        Vector3 coverForward = validatePosition.transform.TransformDirection(Vector3.forward);

                        if (Vector3.Dot(coverForward, directionOfTarget) < 0)
                        {
                            for (int i = 0; i < validatePosition.BackPositions.Count; i++)
                            {
                                if (!validatePosition.BackPositions[i].occupied)
                                {
                                    targetCoverPosition = validatePosition.BackPositions[i];
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < validatePosition.FrontPositions.Count; i++)
                            {
                                if (!validatePosition.FrontPositions[i].occupied)
                                {
                                    targetCoverPosition = validatePosition.FrontPositions[i];
                                }
                            }
                        }
                    }

                    if (targetCoverPosition == null)
                    {
                        findCoverPositions = false;

                        if (coverPositions.Count > 0)
                        {
                            ignorePositions.Add(coverPositions[0]);
                        }
                    }
                    else
                    {
                        targetCoverPosition.occupied = true;
                        currentCoverPosition = targetCoverPosition;
                    }
                }
            }
            else
            {
                Debug.Log("Max tries reached! Changing behavior!");
                enAI_main.AI_State_Attack();
            }
        }

        public void SortPosition(List<Transform> positions)
        {
            objectDistanceComparer = new ObjectDistanceComparer(this.transform);
            positions.Sort(objectDistanceComparer);
        }

        private class ObjectDistanceComparer : IComparer<Transform>
        {
            private Transform referenceObject;

            public ObjectDistanceComparer(Transform reference)
            {
                referenceObject = reference;
            }

            public int Compare(Transform x, Transform y)
            {
                float distX = Vector3.Distance(x.position, referenceObject.position);

                float distY = Vector3.Distance(y.position, referenceObject.position);

                int retVal = 0;

                if (distX < distY)
                {
                    retVal = -1;
                }
                else if (distX > distY)
                {
                    retVal = 1;
                }

                return retVal;
            }
        }
    }
}
