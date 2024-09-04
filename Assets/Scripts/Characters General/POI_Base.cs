using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class POI_Base : MonoBehaviour
    {
        public POIType poiType;

        public enum POIType
        {
            deadbody,
            other
        }
    }
}