using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Player 1 UI")]
    public Slider player1HealthSlider;
    public Slider player1ManaSlider;
    public TextMeshProUGUI player1HealthText;
    public TextMeshProUGUI player1ManaText;

    [Header("Player 2 UI")]
    public Slider player2HealthSlider;
    public Slider player2ManaSlider;
    public TextMeshProUGUI player2HealthText;
    public TextMeshProUGUI player2ManaText;

    private void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.player1Side == null || gm.player2Side == null) return;

        UpdateUI(gm.player1Side, player1HealthSlider, player1ManaSlider, player1HealthText, player1ManaText);
        UpdateUI(gm.player2Side, player2HealthSlider, player2ManaSlider, player2HealthText, player2ManaText);
    }

    void UpdateUI(PlayerSide player, Slider healthSlider, Slider manaSlider, TextMeshProUGUI healthText, TextMeshProUGUI manaText)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = 20;
            healthSlider.value = player.health;
            if (healthText != null)
                healthText.text = $"HP: {player.health}";
        }

        if (manaSlider != null)
        {
            manaSlider.maxValue = GameManager.Instance.turn / 2 + 3; // You can dynamically set this based on your game logic
            manaSlider.value = player.power;
            if (manaText != null)
                manaText.text = $"MP: {player.power}";
        }
    }

    public void endTurnButtonClicked()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null) gm.EndTurn();
    }

    public void debugCardClicked()
    {

        var card = CardDatabase.GetRandomCard();
        Debug.Log($"Debug Card: {card.cardName}");
        GameManager.Instance.playingCard = card;
        Debug.Log($"Debug Card Clicked: {card.cardName}");

    }

}
