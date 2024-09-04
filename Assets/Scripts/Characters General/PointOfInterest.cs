using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class PointOfInterest : MonoBehaviour
    {
        public bool createPointOfInterest;

        public List<CharacterStats> affectedChars = new List<CharacterStats>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (createPointOfInterest)
            {
                for (int i = 0; i < affectedChars.Count; i++)
                {
                    affectedChars[i].ChangeToAlert(transform.position);
                }

                createPointOfInterest = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CharacterStats>())
            {
                if (!affectedChars.Contains(other.GetComponent<CharacterStats>()))
                {
                    affectedChars.Add(other.GetComponent<CharacterStats>());
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<CharacterStats>())
            {
                if (affectedChars.Contains(other.GetComponent<CharacterStats>()))
                {
                    affectedChars.Remove(other.GetComponent<CharacterStats>());
                }
            }
        }
    }
}