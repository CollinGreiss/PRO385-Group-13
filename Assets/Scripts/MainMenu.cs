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
        NetworkManager.Instance.StartHost();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void JoinGame()
    {
        NetworkManager.Instance.StartClient();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game has been quit.");
    }

}
