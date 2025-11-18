using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_PauseHandler : MonoBehaviour
{

    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Animator animator;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f) && hit.collider.CompareTag("PauseButton"))
            {
                PauseToggle();
            }
        }
    }

    public void PauseToggle()
    {
        animator.Play("FrameButtonPress");

        if (pauseCanvas.activeSelf)
        {
            pauseCanvas.SetActive(false);
            pauseScreen.SetActive(false);
        }
        else
        {
            pauseCanvas.SetActive(true);
            pauseScreen.SetActive(true);
        }
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
