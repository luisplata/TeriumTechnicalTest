using TMPro;
using UnityEngine;

internal class PlayerUiCustom : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lifeText, nickText;
    public void UpdatePlayerUi(string playerNick, string life)
    {
        lifeText.text = life;
        nickText.text = playerNick;
    }
}