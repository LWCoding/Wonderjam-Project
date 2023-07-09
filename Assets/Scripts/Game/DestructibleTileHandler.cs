using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTileHandler : MonoBehaviour
{

    public static DestructibleTileHandler Instance;
    public Tilemap destructableTilemap;
    public Dictionary<Vector3Int, int> tileHealth = new Dictionary<Vector3Int, int>();
    private const int DefaultTileHealth = 2;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        InitializeTiles();
    }

    private void InitializeTiles()
    {
        foreach (var pos in destructableTilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = destructableTilemap.CellToWorld(localPlace);
            if (destructableTilemap.HasTile(localPlace))
            {
                tileHealth.Add(localPlace, DefaultTileHealth);
                destructableTilemap.SetTileFlags(localPlace, TileFlags.None);
            }
        }
    }

    public void ShootTileAt(Vector3Int pos, int damage = 1)
    {
        if (destructableTilemap.HasTile(pos))
        {
            tileHealth[pos] -= damage;
            GameController.Instance.GetPooledExplosionObject(pos, 0.7f);
            if (tileHealth[pos] <= 0)
            {
                destructableTilemap.SetTile(pos, null);
                SoundManager.Instance.PlayOneShot(AudioType.X_BLOCK_BREAK, 0.35f);
            }
            else
            {
                destructableTilemap.SetColor(pos, new Color(1, 1, 1, 0.15f + (float)tileHealth[pos] / DefaultTileHealth));
                SoundManager.Instance.PlayOneShot(AudioType.X_BLOCK_HIT, 0.5f);
            }
        }
    }

}
