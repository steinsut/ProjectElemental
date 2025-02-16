using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    void OnEnable(){

        Time.timeScale = 0f;
    }
    public void RestartGame(){

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
