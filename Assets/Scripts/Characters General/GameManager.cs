using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TopDown
{
    public class GameManager : MonoBehaviour
    {
        public CharacterStats selectedPlayer;
        public int playerTeam;
        public GameObject playerControls;
        public bool doubleClick;
        public bool overUIElement;
        public GameObject cameraMover;
        public float cameraSpeed = .3f;
        public AudioSource gunShot;
        public ParticleSystem muzzle;
        public bool ignoreManager;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!ignoreManager)
            {
                if (!overUIElement)
                    HandleSelection();

                bool hasPlayer = selectedPlayer;
                playerControls.SetActive(hasPlayer);

                HandleCameraMovement();
            }


            if (selectedPlayer.health <= 0)
            {
                RestartGame();
            }
        }

        void HandleSelection()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    CheckHit(hit);
                }
            }
        }

        void HandleCameraMovement()
        {
            float hor = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");

            Vector3 newPos = new Vector3(-hor, 0, -vert) * cameraSpeed;
            cameraMover.transform.position += newPos;
        }

        void CheckHit(RaycastHit hit)
        {
            if (hit.transform.GetComponent<CharacterStats>())
            {
                CharacterStats hitStats = hit.transform.GetComponent<CharacterStats>();

                if (hitStats.team == playerTeam)
                {
                    if (selectedPlayer == null)
                    {
                        selectedPlayer = hitStats;
                        selectedPlayer.selected = true;

                    }
                    else
                    {
                        selectedPlayer.selected = false;
                        selectedPlayer = hitStats;
                        selectedPlayer.selected = true;
                    }
                }
                
                
                if(selectedPlayer)
                {
                    if (hitStats.team != playerTeam)
                    {
                        selectedPlayer.run = false;
                        selectedPlayer.lookPosition = hitStats.transform.position;
                        selectedPlayer.anim.Play("Fire");
                        gunShot.Play();
                        muzzle.Emit(1);
                        hitStats.health -= 20;
                        hitStats.morale -= 25;
                        hitStats.supressionLevel += 15;
                    }
                     
                }
            }
            else
            {
                if (selectedPlayer)
                {
                    if (doubleClick)
                    {
                        selectedPlayer.run = true;
                    }
                    else
                    {
                        doubleClick = true;
                        StartCoroutine("closeDoubleClick");
                    }

                    selectedPlayer.MoveToPosition(hit.point);
                }
            }
        }

        void RestartGame()
        {
            new WaitForSeconds(10);
            SceneManager.LoadScene("Demo");
        }

        IEnumerator closeDoubleClick()
        {
            yield return new WaitForSeconds(1);
            doubleClick = false;
        }

        public void EnterUIElement()
        {
            overUIElement = true;
        }

        public void ExitUIElement()
        {
            overUIElement = false;
        }

        public void ChangeStance()
        {
            if (selectedPlayer)
            {
                selectedPlayer.run = false;
                selectedPlayer.crouch = !selectedPlayer.crouch;
            }
        }
    }
}