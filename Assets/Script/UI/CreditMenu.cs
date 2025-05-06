using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleCredits : MonoBehaviour
{
    public Button backButton;
    
    void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(ReturnToMainMenu);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}