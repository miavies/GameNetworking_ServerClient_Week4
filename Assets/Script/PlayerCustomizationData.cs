using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCustomizationData : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameTxt;
    [SerializeField] private TMP_Dropdown colorDropdown;
    [SerializeField] private Button playBtn;
    [SerializeField] private GameObject UICanvas;

    //private void Start()
    //{
    //    playBtn.onClick.AddListener(OnPlayBtnClicked);  
    //}

    //private void OnPlayBtnClicked()
    //{
    //    string playerName = nameTxt.text; 
    //    Color playerColor = SetColor();

    //    NetworkManager.Instance.RPC_SpawnPlayer(Runner.LocalPlayer, playerName, playerColor);
        
    //    UICanvas.SetActive(false);
    //}

    //private Color SetColor()
    //{
    //    switch (colorDropdown.value)
    //    {
    //        case 0: return Color.white;
    //        case 1: return Color.black;
    //        case 2: return Color.red;
    //        case 3: return Color.orange;
    //        case 4: return Color.yellow;
    //        case 5: return Color.green;
    //        case 6: return Color.blue;
    //        case 7: return Color.purple;
    //        default: return Color.cyan;
    //    }
    //}
}
