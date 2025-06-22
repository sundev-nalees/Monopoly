using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static GameEvent<PlayerController> TurnChanged= new GameEvent<PlayerController>();

    public static GameEvent<PlayerController> MoneyChanged = new GameEvent<PlayerController>();


    public List<Tile> BoardTiles;
    public PlayerController Player1;
    public PlayerController Player2;

    public PlayerController CurrentPlayer;
    private int currentPlayerIndex = 0;
    private bool isMoving = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    
    private void Start()
    {
        CurrentPlayer = Player1;
        TurnChanged.Raise(CurrentPlayer);
        MoneyChanged.Raise(CurrentPlayer);
        Player1.MoveToTile(BoardTiles[0], 0);
        Player2.MoveToTile(BoardTiles[0], 0);
    }

    public void RollDiceAndMove()
    {
        if (isMoving) return;

        int roll = Random.Range(1, 7);
        Debug.Log($"{CurrentPlayer.PlayerName} rolled a {roll}");
        StartCoroutine(MovePlayer(roll));
    }

    private IEnumerator MovePlayer(int steps)
    {
        isMoving = true;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(0.4f);

            int nextIndex = (CurrentPlayer.CurrentTileIndex + 1) % BoardTiles.Count;
            CurrentPlayer.MoveToTile(BoardTiles[nextIndex], nextIndex);
        }

        ResolveTileEffect(CurrentPlayer);
        isMoving = false;
    }

    private void ResolveTileEffect(PlayerController player)
    {
        Tile landedTile = BoardTiles[player.CurrentTileIndex];

        switch (landedTile.tileType)
        {
            case TileType.Property:
                if (landedTile.owner == null)
                {
                    UIManager.Instance.CurrentBuyTarget = landedTile;
                    UIManager.Instance.ShowBuyButton(true);
                    return;
                    
                }
                else if (landedTile.owner != player)
                {
                    player.PayRent(landedTile.rent);
                    landedTile.owner.ReceiveRent(landedTile.rent);
                    Debug.Log($"{player.PlayerName} paid rent to {landedTile.owner.PlayerName}");
                }
                break;
            case TileType.Chance:
                TriggerChance(player);
                break;
            case TileType.GoToJail:
                SendPlayerToJail(player);
                break;
        }

        NextTurn();
    }

    private void TriggerChance(PlayerController player)
    {
        float roll = Random.value;
        if (roll < 0.9f)
        {
            int amount = Mathf.RoundToInt(player.Money * Random.Range(0.1f, 0.3f));
            player.Money -= amount;
            MoneyChanged.Raise(CurrentPlayer);
            Debug.Log($"{player.PlayerName} lost ${amount} on a chance card.");
        }
        else
        {
            int bonus = Mathf.RoundToInt(player.Money * 0.5f);
            player.Money += bonus;
            MoneyChanged.Raise(CurrentPlayer);
            Debug.Log($"{player.PlayerName} gained ${bonus} on a lucky chance card!");
        }
        CheckWinconditions();
    }

    private void SendPlayerToJail(PlayerController player)
    {
        for (int i = 0; i < BoardTiles.Count; i++)
        {
            if (BoardTiles[i].tileType == TileType.Jail)
            {
                player.MoveToTile(BoardTiles[i], i);
                player.JailTurnsRemaining = 3;
                Debug.Log($"{player.PlayerName} has been sent to jail.");
                return;
            }
        }
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % 2;
        CurrentPlayer = (currentPlayerIndex == 0) ? Player1 : Player2;
        Debug.Log($"It's now {CurrentPlayer.PlayerName}'s turn.");
        TurnChanged.Raise(CurrentPlayer);
        MoneyChanged.Raise(CurrentPlayer);
    }

    public void CheckWinconditions()
    {
        if (Player1.Money <= 0)
        {
            EndGame(Player2);
        }
        else if (Player2.Money <= 0)
        {
            EndGame(Player1);
        }
    }

    private void EndGame(PlayerController Winner)
    {
        Debug.LogWarning("{Winner.playerName} Wins!");
    }
}
