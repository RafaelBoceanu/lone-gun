using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class ActivateDeactivateRenderers : MonoBehaviour
    {
        public bool activate;
        public bool deactivate;

        public MeshRenderer[] meshRens;

        // Update is called once per frame
        void Update()
        {
            if (activate)
            {
                foreach (MeshRenderer ren in meshRens)
                {
                    ren.enabled = true;
                }

                activate = false;
            }

            if (deactivate)
            {
                foreach (MeshRenderer ren in meshRens)
                {
                    ren.enabled = false;
                }

                deactivate = false;
            }
        }
    }
}