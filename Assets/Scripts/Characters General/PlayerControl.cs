using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TopDown
{
    public class PlayerControl : MonoBehaviour
    {
        Animator anim;
        NavMeshAgent agent;
        CharacterStats charStats;

        public float stopDistance = 1;
        public bool moveToPosition;
        public Vector3 destPosition;

        public bool run;
        public bool crouch;

        public float walkSpeed = 1;
        public float runSpeed = 2;
        public float crouchSpeed = .8f;

        public float maxStance = .9f;
        public float minStance = .1f;
        float targetStance;

        float stance;

        List<Rigidbody> ragdollBones = new List<Rigidbody>();


        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            SetupAnimator();
            agent = GetComponent<NavMeshAgent>();
            charStats = GetComponent<CharacterStats>();
            agent.stoppingDistance = stopDistance - .1f;

            agent.updateRotation = true;
            agent.angularSpeed = 500;
            agent.autoBraking = false;
            InitRagdoll();

            if (GetComponentInChildren<EnemySightSphere>())
            {
                GetComponentInChildren<EnemySightSphere>().gameObject.layer = 2;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!charStats.dead)
            {
                run = charStats.run;

                if (moveToPosition)
                {
                    agent.isStopped = false;
                    agent.updateRotation = true;
                    agent.SetDestination(destPosition);

                    float distanceToTarget = Vector3.Distance(transform.position, destPosition);

                    if (distanceToTarget <= stopDistance)
                    {
                        moveToPosition = false;
                        charStats.run = false;
                    }
                }
                else
                {
                    agent.isStopped = true;
                    agent.updateRotation = false;
                }

                HandleSpeed();
                HandleAiming();
                HandleAnimation();
                HandleStates();
            }
        }

        public void HandleAiming()
        {
            anim.SetBool("Aim", charStats.aim);

            if (charStats.shooting)
            {
                anim.SetTrigger("Shoot");
                charStats.shooting = false;
            }
        }

        void HandleStates()
        {
            if (charStats.run)
            {
                targetStance = minStance;
            }
            else
            {
                if (charStats.crouch)
                {
                    targetStance = maxStance;
                }
                else
                {
                    targetStance = minStance;
                }
            }

            stance = Mathf.Lerp(stance, targetStance, Time.deltaTime * 3);
            anim.SetFloat("Stance", stance);

            anim.SetBool("Alert", charStats.alert);
        }

        void HandleSpeed()
        {
            if (!run)
            {
                if (!crouch)
                {
                    agent.speed = walkSpeed;
                }
                else
                {
                    agent.speed = crouchSpeed;
                }
            }
            else
            {
                agent.speed = runSpeed;
            }
        }

        void HandleAnimation()
        {
            Vector3 relativeDirection = (transform.InverseTransformDirection(agent.desiredVelocity)).normalized;
            float animValue = relativeDirection.z;

            if (!run)
            {
                animValue = Mathf.Clamp(animValue, 0, .5f);
            }

            anim.SetFloat("Forward", animValue, .3f, Time.deltaTime);
        }

        void SetupAnimator()
        {
            // reference to the animator component on root.
            anim = GetComponent<Animator>();

            foreach (var childAnimator in GetComponentsInChildren<Animator>())
            {
                if (childAnimator != anim)
                {
                    anim.avatar = childAnimator.avatar;
                    Destroy(childAnimator);
                    break; // stop searching when finding first animator
                }
            }
        }

        void InitRagdoll()
        {
            Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();
            Collider[] cols = GetComponentsInChildren<Collider>();

            for (int i = 0; i < rb.Length; i++)
            {
                rb[i].isKinematic = true;
                ragdollBones.Add(rb[i]);
            }

            for (int i = 0; i < cols.Length; i++)
            {
                if (i != 0)
                {
                    cols[i].gameObject.layer = 8;
                }
                cols[i].isTrigger = true;
            }
        }
    }
}