using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameTxt;
    [SerializeField] private TMP_Dropdown colorDropdown;
    [SerializeField] private TMP_Dropdown teamDropdown; 
    private Color SetColor()
    {
        switch (colorDropdown.value)
        {
            case 0: return Color.white;
            case 1: return Color.black;
            case 2: return Color.red;
            case 3: return Color.orange;
            case 4: return Color.yellow;
            case 5: return Color.green;
            case 6: return Color.blue;
            case 7: return Color.purple;
            default: return Color.cyan;
        }
    }

    private int SetPlayerTeam()
    {
        if (teamDropdown.value == 0)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
    private string SetName()
    {
        return nameTxt.text;
    }

    public void OnReadyClicked()
    {
        NetworkSessionManager.Instance.SavePlayerDataOnSession(
            SetName(),
            SetColor(),
            SetPlayerTeam()
        );
        Debug.Log("Sent Data from UI Manager");
    }
}
