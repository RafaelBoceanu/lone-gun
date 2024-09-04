using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class LevelManager : MonoBehaviour
    {
        public List<Transform> exitPositions = new List<Transform>();
        public List<RetreatActionsBase> retreatPositions = new List<RetreatActionsBase>();

        public static LevelManager instance;

        public static LevelManager GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }

        public RetreatActionsBase ReturnRetreatPosition(Transform posTransform)
        {
            RetreatActionsBase retVal = null;

            for (int i = 0; i < retreatPositions.Count; i++)
            {
                if (posTransform == retreatPositions[i].retreatPositions)
                {
                    retVal = retreatPositions[i];
                    break;
                }
            }

            return retVal;
        }
    }

    [System.Serializable]
    public class RetreatActionsBase
    {
        public bool inUse;
        public bool visited;
        public Transform retreatPositions;
        public List<EnemyAI> reinforcements = new List<EnemyAI>();
    }
}