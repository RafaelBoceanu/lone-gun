using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TopDown
{
    public class CharacterStats : MonoBehaviour
    {
        public float health = 100;
        public int morale = 100;
        public int supressionLevel = 20;
        public int unitRank = 0;

        public float viewAngleLimit = 50;
        public int alertLevel;
        public bool selected;
        public bool dead;
        public int team;
        public bool crouch;
        public bool run;
        public bool alert = true;
        public bool attack;
        public bool aim;
        public bool shooting;
        public bool hasCover;
        public GameObject selectedCube;
        
        [HideInInspector]
        public PlayerControl playerControl;
        [HideInInspector]
        public EnemyAI enAI;
        [HideInInspector]
        public Animator anim;

        public bool reload;
        public Vector3 lookPosition;

        [HideInInspector]
        public POI_Deadbody enableOnDeath;

        public Transform alertDebugCube;

        // Start is called before the first frame update
        void Start()
        {
            health = 100;

            playerControl = GetComponent<PlayerControl>();

            if (GetComponent<EnemyAI>())
            {
                enAI = GetComponent<EnemyAI>();
            }
            if (GetComponent<Animator>())
            {
                anim = GetComponent<Animator>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            selectedCube.SetActive(selected);

            if (run)
            {
                crouch = false;
            }

            if (alertDebugCube)
            {
                float scale = alertLevel * .05f;
                alertDebugCube.localScale = new Vector3(scale, scale, scale);
            }

            if (morale < 0)
                morale = 0;

            if (morale < 50 && supressionLevel > 25)
            {
                enAI.aiStates = EnemyAI.AIstates.retreat;
            }

            if (health <= 0)
            {
                health = 0;

                if (!dead)
                {
                    dead = true;

                    if (enAI)
                    {
                        enAI.alliesBehavior.DecreaseAlliesMorale(30);
                        enAI.alliesBehavior.IncreaseAlliesSupression(15);
                    }

                    KillCharacter();
                }
            }
        }

        public void StopMoving()
        {
            playerControl.moveToPosition = false;
        }

        public void MoveToPosition(Vector3 position)
        {
            playerControl.moveToPosition = true;
            playerControl.destPosition = position;
        }

        public void CallFunctionWithString(string functionIdentifier, float delay)
        {
            Invoke(functionIdentifier, delay);
        }

        public void ChangeToNormal()
        {
            enAI.ChangeAIBehaviour("AI_State_Normal", 0);
            alert = false;
            crouch = false;
            run = false;
        }

        public void ChangeToAlert(Vector3 poi)
        {
            alert = true;
            playerControl.moveToPosition = false;

            enAI.GoOnAlert(poi);
        }

        void ChangeStance()
        {
            crouch = !crouch;
        }

        void AlertPhase()
        {
            alert = !alert;
        }

        void ChangeRunState()
        {
            run = !run;
        }

        void AttackPhase()
        {
            attack = !attack;
        }    

        public void KillCharacter()
        {

            anim.SetBool("Dead", true);

            MonoBehaviour[] comp = GetComponents<MonoBehaviour>();
            
            for (int i = 0; i < comp.Length; i++)
            {
                comp[i].enabled = false;
            }

            this.enabled = true;

            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}
