using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TopDown
{
    public class EnemyUI : MonoBehaviour
    {
        public bool show = true;
        public GameObject enUIprefab;
        public GameObject enUI;
        Text textUI;
        Text morale;
        Text supression;
        EnemyAI enAI;

        // Start is called before the first frame update
        void Start()
        {
            enAI = GetComponent<EnemyAI>();
            enUI = Instantiate(enUIprefab, transform.position, Quaternion.identity) as GameObject;
            enUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);


            Text[] texts = enUI.GetComponentsInChildren<Text>();

            textUI = texts[0];
            morale = texts[1];
            supression = texts[2];
        }

        // Update is called once per frame
        void Update()
        {
            if (show)
            {
                enUI.gameObject.SetActive(true);

                string info = enAI.aiStates.ToString();

                textUI.text = info;
                morale.text = "morale " + enAI.charStats.morale.ToString();
                supression.text = "supression" + enAI.charStats.supressionLevel.ToString();

                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
                enUI.transform.position = screenPoint;
            }
            else
            {
                enUI.gameObject.SetActive(false);
            }
        }

        public void EnableDisableUI()
        {
            show = !show;
        }
    }
}