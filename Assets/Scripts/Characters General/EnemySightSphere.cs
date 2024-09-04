using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class EnemySightSphere : MonoBehaviour
    {
        EnemyAI enAI;
        CharacterStats charStats;

        List<CharacterStats> trackingTargets = new List<CharacterStats>();

        // Start is called before the first frame update
        void Start()
        {
            enAI = GetComponentInParent<EnemyAI>();
            charStats = GetComponentInParent<CharacterStats>();
        }

        // Update is called once per frame
        void Update()
        {
            //if(enAI.target==null)
            //{
            for (int i = 0; i < trackingTargets.Count; i++)
            {
                if (trackingTargets[i] != enAI.target)
                {
                    Vector3 direction = trackingTargets[i].transform.position - transform.position;
                    float angleTowardsTarget = Vector3.Angle(transform.parent.forward, direction.normalized);

                    if (angleTowardsTarget < charStats.viewAngleLimit)
                    {
                        enAI.target = trackingTargets[i];
                    }
                }
                else
                {
                    continue;
                }
            }
            // } ///if(enAI.target == null)
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CharacterStats>())
            {
                CharacterStats otherStats = other.GetComponent<CharacterStats>();

                if (otherStats.team != charStats.team)
                {
                    if (!trackingTargets.Contains(otherStats))
                    {
                        trackingTargets.Add(otherStats);
                    }
                }
                else
                {
                    EnemyAI otherAI = otherStats.transform.GetComponent<EnemyAI>();

                    if (otherAI != enAI)
                    {
                        if (!otherAI.charStats.dead)
                        {
                            if (!enAI.AlliesNear.Contains(otherAI))
                            {
                                enAI.AlliesNear.Add(otherAI);
                            }
                        }
                    }
                }
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<POI_Base>())
            {
                POI_Base poi = other.GetComponent<POI_Base>();

                if (!enAI.PointsOfInterestList.Contains(poi))
                {
                    enAI.PointsOfInterestList.Add(poi);
                }
                else
                {
                    enAI.PointsOfInterestList.Remove(poi);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<CharacterStats>())
            {
                CharacterStats leavingTarget = other.GetComponent<CharacterStats>();

                if (trackingTargets.Contains(leavingTarget))
                {
                    trackingTargets.Remove(leavingTarget);
                }

                if (leavingTarget.transform.GetComponent<EnemyAI>())
                {
                    EnemyAI otherAI = leavingTarget.transform.GetComponent<EnemyAI>();

                    if (otherAI != enAI)
                    {
                        if (!enAI.AlliesNear.Contains(otherAI))
                        {
                            enAI.AlliesNear.Remove(otherAI);
                        }
                    }
                }

            }
        }
    }
}