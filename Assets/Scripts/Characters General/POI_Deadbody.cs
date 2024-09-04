using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class POI_Deadbody : POI_Base
    {
        public CharacterStats owner;

        // Start is called before the first frame update
        void Start()
        {
            owner.GetComponentInParent<CharacterStats>();
            owner.enableOnDeath = this;
            this.enabled = false;
        }
    }
}