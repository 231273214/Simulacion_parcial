using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject pauseUI;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnGameResumePress()
    {
        isPaused = false;
        pauseUI.SetActive(false);
        Time.timeScale = 1f;

    }

    public void OnGameExitPress()
    {
        Application.Quit();
    }

    public void OnGameOptionsPress()
    {

    }
}
