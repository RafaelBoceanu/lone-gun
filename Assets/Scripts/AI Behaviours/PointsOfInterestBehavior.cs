using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class PointsOfInterestBehavior : MonoBehaviour
    {
        EnemyAI enAI_main;

        void Start()
        {
            enAI_main = GetComponent<EnemyAI>();
        }

        public void POIBehavior()
        {
            if (enAI_main.PointsOfInterestList.Count > 0)
            {
                for (int i = 0; i < enAI_main.PointsOfInterestList.Count; i++)
                {
                    if (enAI_main.pointOfInterest[i] != null)
                    {
                        POIBehavior(enAI_main.PointsOfInterestList[i].poiType,
                            enAI_main.PointsOfInterestList[i]);
                    }
                }
            }
        }

        void POIBehavior(POI_Base.POIType type, POI_Base poi)
        {
            switch (type)
            {
                case POI_Base.POIType.deadbody: // there is a poi nearby

                    POI_Deadbody body = poi.transform.GetComponent<POI_Deadbody>(); // find the type of the dead body

                    if (body.isActiveAndEnabled)
                    {
                        Vector3 directionTowardsPOI = body.transform.position - transform.position;
                        float angleTowardsTarget = Vector3.Angle(transform.forward, directionTowardsPOI.normalized);

                        if (angleTowardsTarget < enAI_main.charStats.viewAngleLimit)
                        {
                            Vector3 origin = transform.position + new Vector3(0, 1.8f, 0);
                            Vector3 rayDirection = body.transform.position - origin;

                            RaycastHit hit;

                            if (Physics.Raycast(origin, rayDirection, out hit, enAI_main.sightDistance))
                            {
                                if (hit.transform.Equals(body.transform) || hit.transform.GetComponentInParent<CharacterStats>())
                                {
                                    if (hit.transform.GetComponentInParent<CharacterStats>().dead)
                                    {
                                        enAI_main.lastKnownPosition = poi.transform.position;
                                        enAI_main.charStats.alert = true;
                                        enAI_main.charStats.alertLevel = 10;

                                        enAI_main.AI_State_Chase();


                                        enAI_main.PointsOfInterestList.Remove(poi);
                                        Destroy(poi);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        enAI_main.PointsOfInterestList.Remove(poi);
                    }
                    break;
                case POI_Base.POIType.other:
                    //reserved
                    break;
            }
        }
    }
}