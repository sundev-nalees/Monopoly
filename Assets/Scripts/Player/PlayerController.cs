using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static GameEvent<PlayerController> MoneyChanged=new GameEvent<PlayerController>();

    


    public string PlayerName;
    public int Money = 1500;
    public int CurrentTileIndex = 0;
    public int JailTurnsRemaining = 0;


    public List<Tile> OwnedProperties=new List<Tile>();


    private void Start()
    {
        MoneyChanged.Raise(this);
    }

    /// <summary>
    /// Moves the player character to the specified tile and updates the current tile index.
    /// Applies a slight horizontal offset based on whether the player is Player 1 or Player 2,
    /// to visually separate player tokens on the same tile.
    /// </summary>
    /// <param name="tile">The target tile to move the player to.</param>
    /// <param name="tileIndex">The index of the target tile on the board.</param>
    public void MoveToTile(Tile tile,int tileIndex) 
    {

        CurrentTileIndex = tileIndex;

        Vector3 basePosition = tile.transform.position;

        
        Vector3 offset = (this == GameManager.Instance.Player1)
            ? new Vector3(-0.2f, 0f, 0)
            : new Vector3(0.2f, 0f, 0); 

        transform.position = basePosition + offset;
    }

    /// <summary>
    /// Attempts to purchase a property tile. The purchase is completed if:
    /// the tile is a property, it has no current owner, and the player has enough money.
    /// Updates ownership, deducts cost, raises a money changed event, and checks for win conditions.
    /// </summary>
    /// <param name="tile">The tile representing the property to be purchased.</param>
    public void BuyProperty(Tile tile)
    {
        if (tile.tileType == TileType.Property && tile.owner == null && Money >= tile.cost)
        {
            Money-= tile.cost;
            tile.owner=this;
            OwnedProperties.Add(tile);
            MoneyChanged.Raise(this);
            Debug.Log($"{GameManager.Instance.CurrentPlayer.PlayerName} bought {tile.tileName}");
            GameManager.Instance.CheckWinconditions();
        }
    }

    /// <summary>
    /// Deducts the specified rent amount from the player's money,
    /// triggers the money changed event, and checks for game win conditions.
    /// </summary>
    /// <param name="rent">The amount of rent to be paid.</param>

    public void PayRent(int rent)
    {
        Money -= rent;
        MoneyChanged.Raise(this);
        GameManager.Instance.CheckWinconditions();
    }

    /// <summary>
    /// Increases the player's money by the specified rent amount,
    /// triggers the money changed event, and checks for game win conditions.
    /// </summary>
    /// <param name="rent">The amount of rent to be received.</param>

    public void  ReceiveRent(int rent)
    {
        Money += rent;
        MoneyChanged.Raise(this);
        GameManager.Instance.CheckWinconditions();
    }

}
