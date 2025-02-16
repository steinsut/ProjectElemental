using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    void OnEnable(){
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
    public void RestartGame(){
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
