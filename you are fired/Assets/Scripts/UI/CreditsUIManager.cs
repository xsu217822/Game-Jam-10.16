using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CreditsUIManager : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button exitButton;
    [SerializeField] private bool autoReturnToMenu = true;
    [SerializeField] private float autoReturnDelay = 2f;

    private bool isInitialized = false;

    private void OnEnable()
    {
        if (creditsPanel == null || exitButton == null)
        {
            FindUIComponents();
        }
        if (!isInitialized)
        {
            ConnectButton();
            HideCreditsPanel();
        }
    }

    private void FindUIComponents()
    {
        if (creditsPanel == null)
        {
            creditsPanel = GameObject.Find("Canvas/credit");
        }
        
        if (exitButton == null)
        {
            GameObject creditObj = GameObject.Find("Canvas/credit");
            if (creditObj != null)
            {
                Button[] buttons = creditObj.GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    if (btn.name.Contains("Exit") || btn.name.Contains("exit") || btn.name.Contains("apply"))
                    {
                        exitButton = btn;
                        break;
                    }
                }
            }
        }

        if (creditsPanel == null || exitButton == null)
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            
            if (canvas != null)
            {
                if (creditsPanel == null)
                {
                    Transform[] allTransforms = canvas.GetComponentsInChildren<Transform>(true);
                    foreach (Transform t in allTransforms)
                    {
                        if (t.name == "credit" || t.name.Contains("Credit"))
                        {
                            creditsPanel = t.gameObject;
                            break;
                        }
                    }
                }
                
                if (exitButton == null)
                {
                    Button[] allButtons = canvas.GetComponentsInChildren<Button>(true);
                    foreach (Button btn in allButtons)
                    {
                        if (btn.name.Contains("Exit") || btn.name.Contains("exit") || btn.name.Contains("apply"))
                        {
                            exitButton = btn;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void ConnectButton()
    {
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitCredits);
            exitButton.onClick.AddListener(ExitCredits);
        }
    }

    private void HideCreditsPanel()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (!isInitialized)
        {
            if (creditsPanel == null || exitButton == null)
            {
                FindUIComponents();
            }
            
            if (creditsPanel == null || exitButton == null)
            {
                return;
            }
            
            ConnectButton();
            HideCreditsPanel();
            isInitialized = true;
        }
    }

    public void EnterCredits()
    {
        if (creditsPanel == null || exitButton == null)
        {
            FindUIComponents();
        }
        
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }

        if (exitButton != null && !isInitialized)
        {
            ConnectButton();
        }

        if (AudioManager.I != null)
        {
            AudioManager.I.PlayCredits();
        }
    }

    public void ExitCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }

        if (AudioManager.I != null)
        {
            AudioManager.I.EndCredits();
        }

        ReturnToMenu();
    }

    public void OnCreditsScrollEnd()
    {
        if (autoReturnToMenu)
        {
            Invoke(nameof(ExitCredits), autoReturnDelay);
        }
    }

    private void ReturnToMenu()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    public void StartCredits()
    {
        EnterCredits();
    }

    public void EndCredits()
    {
        ExitCredits();
    }
}
