using System.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;
    [SerializeField] private TextMeshProUGUI player1Details;
    [SerializeField] private TextMeshProUGUI player2Details;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject buyUI;

    public Tile CurrentBuyTarget;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        
    }
    private void OnEnable()
    {
        PlayerController.MoneyChanged.Subscribe(UpdatePlayerMoney);
        GameManager.MoneyChanged.Subscribe(UpdatePlayerMoney);
        GameManager.MoneyChanged.Subscribe(PlayerUpdateStatus);
        GameManager.TurnChanged.Subscribe(UpdateTurnUI);
    }

    private void OnDisable()
    {
        PlayerController.MoneyChanged.Unsubscribe(UpdatePlayerMoney);
        GameManager.MoneyChanged.Unsubscribe(UpdatePlayerMoney);
        GameManager.TurnChanged.Unsubscribe(UpdateTurnUI);
        GameManager.MoneyChanged.Unsubscribe(PlayerUpdateStatus);
    }

    private void UpdatePlayerMoney(PlayerController player)
    {
        if (player == GameManager.Instance.Player1)
            player1Text.text = $"{player.PlayerName}: ${player.Money}";
        else
            player2Text.text = $"{player.PlayerName}: ${player.Money}";
    }

    private void UpdateTurnUI(PlayerController player)
    {
        turnText.text = $"Turn: {player.PlayerName}";
        

    }

    public void OnBuyButtonClicked()
    {
        if(CurrentBuyTarget!=null)
        {
            GameManager.Instance.CurrentPlayer.BuyProperty(CurrentBuyTarget);
            

            ShowBuyButton(false);
            CurrentBuyTarget = null;

            GameManager.Instance.NextTurn();
        }
    }


    public void ShowBuyButton(bool show)
    {
        buyUI.SetActive(show);
    }
    public void OnCancelButtonClciked()
    {
        ShowBuyButton(false );
        GameManager.Instance.NextTurn();
    }

    public void PlayerUpdateStatus(PlayerController player)
    {
        player1Details.text = $"{GameManager.Instance.Player1.PlayerName}: ${GameManager.Instance.Player1.Money}\nProperties: {string.Join(", ", GameManager.Instance.Player1.OwnedProperties.Select(t => t.tileName))}";
        player2Details.text = $"{GameManager.Instance.Player2.PlayerName}: ${GameManager.Instance.Player2.Money}\nProperties: {string.Join(", ", GameManager.Instance.Player2.OwnedProperties.Select(t => t.tileName))}";
        
    }
}
