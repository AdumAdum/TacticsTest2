using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    // There will only ever be one Map Manager per scene, this code insures this
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    [SerializeField] private int OverLayer = 5;

    public Dictionary<Vector2Int, OverlayTile> map;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;

        // Loop through all tiles in tilemap
        //Debug.Log(bounds);

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    //Debug.Log($"{x}, {y}, {z}");
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);

                    if(tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        overlayTile.name = $"{tileMap.GetTile(tileLocation).name}";
                        overlayTile.TerrainSetup();
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z+1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = OverLayer;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);

                        //CHECK FOR BLACK TILES, EXPAND INTO TERRAIN SCRIPT / SYSTEM LATER
                        //string test = tileMap.GetTile(tileLocation).ToString();
                        //Debug.Log($"{test}");}
                    }
                }
            }
        }
    }

    public List<OverlayTile> getAdjNodes(OverlayTile currentTile)
    {
        List<OverlayTile> adjNodes = new List<OverlayTile>();

        Vector2Int[] Neighbors = {
            checkCardinal(currentTile, -1, 0), //left
            checkCardinal(currentTile, 0, 1),  //up
            checkCardinal(currentTile, 1, 0),  //right
            checkCardinal(currentTile, 0, -1)  //down
        };

        foreach (Vector2Int nodePos in Neighbors)
        {
            if (map.ContainsKey(nodePos))
            {
                adjNodes.Add(map[nodePos]);
            }
        }
        return adjNodes;
    }

    private Vector2Int checkCardinal(OverlayTile tile, int xDisplace=0, int yDisplace=0)
    {
        Vector2Int locationToCheck = new Vector2Int(
            tile.gridLocation.x + xDisplace,
            tile.gridLocation.y + yDisplace
        );
        return locationToCheck;
    }
}
