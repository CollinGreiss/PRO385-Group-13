using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject defaultPanel;
    public GameObject joinPanel;

    public void ShowDefaultPanel()
    {
        defaultPanel.SetActive(true);
        joinPanel.SetActive(false);
    }

    public void ShowJoinPanel()
    {
        defaultPanel.SetActive(false);
        joinPanel.SetActive(true);
        
    }

    public void StartGame()
    {
        GameManager.Instance.networkManager.StartHost();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void JoinGame()
    {
        GameManager.Instance.networkManager.StartClient();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game has been quit.");
    }

}
