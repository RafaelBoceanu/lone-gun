using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TopDown
{
    public class SearchBehavior : MonoBehaviour
    {
        // searching variables
        public bool decideBehaviour;
        public float decideBehaviourThreshold = 5;
        public List<Transform> possibleHidingPlaces = new List<Transform>();
        public List<Vector3> positionsAroundUnit = new List<Vector3>();
        bool getPossibleHidingPositions;
        bool populateListOfPositions;
        bool searchAtPositions;
        bool createSearchPositions;
        int indexSearchPositions;
        bool searchHidingSpots;
        Transform targetHidingSpot;

        public float delayTillNewBehaviour = 3;
        float _timerTillNewBehaviour;

        EnemyAI enAI_main;

        void Start()
        {
            enAI_main = GetComponent<EnemyAI>();
        }
        public void SearchBehaviour()
        {
            if (!decideBehaviour)
            {
                int ranValue = Random.Range(0, 11);

                if (ranValue < decideBehaviourThreshold)
                {
                    searchAtPositions = true;
                    Debug.Log("Searching in positions around unit");
                }
                else
                {
                    searchHidingSpots = true;
                    Debug.Log("Searching in hiding spots");
                }

                decideBehaviour = true;
            }
            else
            {
                #region Search for HidingSpots
                if (searchHidingSpots)
                {
                    if (!populateListOfPositions)
                    {
                        possibleHidingPlaces.Clear();

                        Collider[] allColliders = Physics.OverlapSphere(transform.position, 20);

                        for (int i = 0; i < allColliders.Length; i++)
                        {
                            if (allColliders[i].GetComponent<HidingSpot>())
                            {
                                possibleHidingPlaces.Add(allColliders[i].transform);
                            }
                        }
                        populateListOfPositions = true;
                    }
                    else if (possibleHidingPlaces.Count > 0)
                    {
                        if (!targetHidingSpot)
                        {
                            int ranValue = Random.Range(0, possibleHidingPlaces.Count);

                            targetHidingSpot = possibleHidingPlaces[ranValue];
                        }
                        else
                        {
                            enAI_main.charStats.MoveToPosition(targetHidingSpot.position);

                            Debug.Log("Going to Hiding Spot");
                            float distanceToTarget = Vector3.Distance(transform.position, targetHidingSpot.position);

                            if (distanceToTarget < 2)
                            {
                                _timerTillNewBehaviour += Time.deltaTime;

                                if (_timerTillNewBehaviour > delayTillNewBehaviour)
                                {
                                    //do things and reset
                                    populateListOfPositions = false;
                                    targetHidingSpot = null;
                                    decideBehaviour = false;
                                    _timerTillNewBehaviour = 0;
                                }
                            }
                        }
                    }
                    else // else if (possibleHidingPlaces.Count > 0)
                    {
                        //no hiding spot found near unit, search at positions instead
                        Debug.Log("No hiding spots found, cancel it and search at positions instead");
                        searchAtPositions = true;
                        populateListOfPositions = false;
                        targetHidingSpot = null;
                    }
                }
                #endregion

                if (searchAtPositions)
                {
                    if (!createSearchPositions)
                    {
                        positionsAroundUnit.Clear();

                        int ranValue = Random.Range(4, 10);

                        for (int i = 0; i < ranValue; i++)
                        {
                            float offsetX = Random.Range(-10, 10);
                            float offsetZ = Random.Range(-10, 10);

                            Vector3 originPos = transform.position;
                            originPos += new Vector3(offsetX, 0, offsetZ);

                            NavMeshHit hit;

                            if (NavMesh.SamplePosition(originPos, out hit, 5, NavMesh.AllAreas))
                            {
                                positionsAroundUnit.Add(hit.position);
                            }
                        }

                        if (positionsAroundUnit.Count > 0)
                        {
                            indexSearchPositions = 0;
                            createSearchPositions = true;
                        }  //else try again until you find one
                    }
                    else
                    {
                        Vector3 targetPosition = positionsAroundUnit[indexSearchPositions];

                        Debug.Log("Going To Position");

                        enAI_main.charStats.MoveToPosition(targetPosition);

                        float distanceToPosition = Vector3.Distance(transform.position, targetPosition);

                        if (distanceToPosition < 2)
                        {
                            int ranVal = Random.Range(0, 11);
                            decideBehaviour = (ranVal < 5);

                            if (indexSearchPositions < positionsAroundUnit.Count - 1)
                            {
                                _timerTillNewBehaviour += Time.deltaTime;

                                if (_timerTillNewBehaviour > delayTillNewBehaviour)
                                {
                                    indexSearchPositions++;
                                    _timerTillNewBehaviour = 0;
                                }
                            }
                            else
                            {
                                _timerTillNewBehaviour += Time.deltaTime;

                                if (_timerTillNewBehaviour > delayTillNewBehaviour)
                                {
                                    indexSearchPositions = 0;
                                    _timerTillNewBehaviour = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
