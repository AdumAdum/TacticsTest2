using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    // A*
    public int G;
    public int H;
    public int F { get { return G + H; } }

    //Djikstra's
    public int movCost;
    public int pathCost = 99;

    //Both
    public OverlayTile previous;
    public bool occupiedPlayer = false;
    public bool occupiedEnemy = false;
    
    public CharacterManager playerOn;
    public CharacterManager enemyOn;

    public Vector3Int gridLocation;
    public Vector2Int gridLocation2D {get {return new Vector2Int( gridLocation.x, gridLocation.y); } }

    SpriteRenderer sprite_renderer;

    void Start()
    {
        sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
        TerrainSetup();
    }

    public void ShowTile()
    {
        sprite_renderer.color = new Color(1,1,1,1f);
    }

    public void ShowAtkTile()
    {
        sprite_renderer.color = new Color(1f,0f,0f,1f);
    }

    public void HideTile()
    {
        sprite_renderer.color = new Color(1,1,1,0);
    }

    /*if (tileMap.GetTile(tileLocation).name == "Black")
    {
        overlayTile.isBlocked = true;
    }*/

    //There has to be a better way to do this using grids and tilemaps, but I dunno how
    public void TerrainSetup()
    {
        switch (name)
        {
            case "Dirt":
                movCost = 1;
                break;

            case "Mud":
                movCost = 2;
                break;

            case "Black":
                movCost = 99;
                break;

            default:
                break;
        }
    }
}
