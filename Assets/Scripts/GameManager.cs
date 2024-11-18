using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject parentObject;
    public PlayerController pm;
    public GameObject victoryUI;
    public GameObject gameOverUI;

    public GameObject player;

    void Update()
    {
        if (parentObject != null)
        {
            int childCount = parentObject.transform.childCount;

            if (childCount == 0)
            {
                victoryUI.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;  // Unlock the cursor
                Cursor.visible = true;  // Make the cursor visible when unlocked
                pm.enabled = false;

                PlayerInput playerInput = player.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    playerInput.enabled = false;
                }
            }
            else if (pm.health < 0)
            {
                gameOverUI.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;  // Unlock the cursor
                Cursor.visible = true;  // Make the cursor visible when unlocked
                pm.enabled = false;

                PlayerInput playerInput = player.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    playerInput.enabled = false;
                }
            }
        }
        else
        {
            Debug.LogWarning("No parent object assigned.");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
