using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType
{
    Property,
    Chance,
    Jail,
    Start,
    GoToJail,
    Utility,
    Special
}

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite image;
    [SerializeField] private SpriteRenderer sRenderer;


    public TileType tileType;
    public string tileName;
    public int cost;
    public int rent;
    
    [HideInInspector] public PlayerController owner;

    private void Start()
    {
        
    }


    private void UpdateSprite(Sprite sprite)
    {
        sRenderer.sprite=sprite;
    }
}
