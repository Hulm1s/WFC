using System;
using System.Collections.Generic;
using System.Linq;


namespace WFC.Core
{
    public class WfcGrid
    {
        public int Width {get; private set;}
        public int Height {get; private set;}

        public Cell[,] Grid {get; private set;}

        private Random rng = new Random();

        public WfcGrid(int width, int height, List<TileData> allTiles)
        {
            Width = width;
            Height = height;
            Grid = new Cell[width, height];

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Grid[x,y] = new Cell(x,y,allTiles);
                }
            }
        }
// Nalezne pole s nejnizsi Entropii (>1)
        public Cell GetLowestEntropyCell()
        {
            Cell lowest = null;
            int minEntropy = int.MaxValue;

            foreach(var cell in Grid)
            {
                if(!cell.IsCollapsed && !cell.IsContradiction)
                {
                    if(cell.Entropy < minEntropy)
                    {
                        minEntropy = cell.Entropy;
                        lowest = cell;
                    }
                }
            }
            return lowest;
        }
//Krok algoritmu (Kolaps + Propagace)
        public bool Step()
        {
            Cell cellToCollapse = GetLowestEntropyCell();
            if(cellToCollapse == null) return false; //HOTOVO nebo slepa 

            cellToCollapse.Collapse(rng);
            Propagate(cellToCollapse);
            return true;

        }

        private void Propagate(Cell startCell)
        {
            Stack<Cell> stack = new Stack<Cell>();
            stack.Push(startCell);

            while (stack.Count > 0)
            {
                Cell current = stack.Pop();

                foreach(Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Cell neighbor = GetNeighbor(current, dir);
                    if(neighbor == null || neighbor.IsCollapsed) continue;

                    var validNeighborTiles = neighbor.PossibleTiles.Where(neighborTile =>
                    current.PossibleTiles.Any(myTile => myTile.CanConnectTo(neighborTile, dir))).ToList();

                    if(validNeighborTiles.Count < neighbor.PossibleTiles.Count){
                        neighbor.PossibleTiles.Clear();
                        neighbor.PossibleTiles.AddRange(validNeighborTiles);
                        stack.Push(neighbor);
                    }
                }
            }
        }

        private Cell GetNeighbor(Cell cell, Direction dir)
        {
            int nx = cell.X + (dir == Direction.East ? 1 : dir == Direction.West ? -1 : 0);
            int ny = cell.Y + (dir == Direction.North ? 1 : dir == Direction.South ? -1 : 0);

            if (nx>= 0 && nx<Width && ny >= 0 && ny < Height)
                return Grid[nx,ny];
            return null;    
        }


    }

}