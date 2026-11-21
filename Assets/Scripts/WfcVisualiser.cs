using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFC.Core;
public class WfcVisualiser : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    private WfcGrid wfcGrid;
    private List<TileData> tiles;


    // Start is called before the first frame update
    void Start()
    {
        InitWfc();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool stepped = wfcGrid.Step();
            if(!stepped) Debug.Log("Generovani ukonceno nebo slepa ulicka");
            else Debug.Log($"Count of popped cells: {wfcGrid.LastCellsPopped} Count of removed tiles: {wfcGrid.LastTilesRemoved}");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            InitWfc();
        }
    }


    void InitWfc()
    {
        //zakladni sada dilku
        tiles = new List<TileData>
        {
            new TileData("Prazdno", ' ', 0, 0, 0, 0),
            new TileData("Rovna_SJ", '║', 1, 0, 1, 0),
            new TileData("Rovna_VZ", '═', 0, 1, 0, 1),
            new TileData("Krizovatka", '╬', 1, 1, 1, 1),
            new TileData("Roh_SV", '╚', 1, 1, 0, 0),
            new TileData("Roh_VJ", '╔', 0, 1, 1, 0),
            new TileData("Roh_JZ", '╗', 0, 0, 1, 1),
            new TileData("Roh_ZS", '╝', 1, 0, 0, 1)
        };
        wfcGrid = new WfcGrid(width, height, tiles);
        Debug.Log("Mřížka připravena! Mačkej MEZERNÍK pro skládání krok za krokem.");
    }

    void OnDrawGizmos()
    {
        if (wfcGrid == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = wfcGrid.Grid[x, y];
                Vector3 pos = new Vector3(x, y, 0);

                if (cell.IsContradiction)
                {
                    // Červená = Slepá ulička (0 možností)
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(pos, Vector3.one * 0.8f);
                }
                else if (cell.IsCollapsed)
                {
                    // Zelená = Hotové políčko + vykreslení čar podle toho, kam vede cesta
                    TileData tile = cell.PossibleTiles[0];
                    Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.4f);
                    Gizmos.DrawCube(pos, Vector3.one * 0.9f);

                    Gizmos.color = Color.white;
                    if (tile.Sockets[0] == 1) Gizmos.DrawLine(pos, pos + Vector3.up * 0.5f);    // Sever
                    if (tile.Sockets[1] == 1) Gizmos.DrawLine(pos, pos + Vector3.right * 0.5f); // Východ
                    if (tile.Sockets[2] == 1) Gizmos.DrawLine(pos, pos + Vector3.down * 0.5f);  // Jih
                    if (tile.Sockets[3] == 1) Gizmos.DrawLine(pos, pos + Vector3.left * 0.5f);  // Západ
                }
                else
                {
                    // Šedá = Ještě nerozhodnuto (čím menší čtvereček, tím nižší entropie!)
                    Gizmos.color = Color.gray;
                    float scale = (float)cell.Entropy / tiles.Count;
                    Gizmos.DrawWireCube(pos, Vector3.one * (scale * 0.8f));
                }
            }
        }
    }
}
