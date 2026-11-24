using System;
using System.Collections.Generic;
using System.Linq;


namespace WFC.Core
{
    public enum StepResult
    {
        Finished, Contradicted, Running
    }
    public class WfcGrid
    {
        public int Width {get; private set;}
        public int Height {get; private set;}
        //Properties for measuring. To remain independent on Unity. 
        public int LastCellsPopped {get; private set;}
        public int LastTilesRemoved {get; private set;}

        public Cell ContradictionCell {get; private set;}
        public Cell[,] Grid {get; private set;}

        private Random rng;

        public WfcGrid(int width, int height, List<TileData> allTiles, int seed)
        {
            rng = new Random(seed);
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
            double minEntropy = double.MaxValue;

            foreach(var cell in Grid)
            {
                if(!cell.IsCollapsed && !cell.IsContradiction)
                {
                    //Vytvoření jemného noise, abychom nezačínali vždy od (0,0)
                    double noisy = cell.Entropy + rng.NextDouble() * 0.001; 
                    if( noisy < minEntropy)
                    {
                        minEntropy = noisy;
                        lowest = cell;
                    }
                }
            }
            return lowest;
        }
//Krok algoritmu (Kolaps + Propagace)
        public StepResult Step()
        {
            Cell cellToCollapse = GetLowestEntropyCell();
            if(cellToCollapse == null) return StepResult.Finished ; //Done
            
            cellToCollapse.Collapse(rng);
            Propagate(cellToCollapse);

            if(ContradictionCell != null) return StepResult.Contradicted; //Contradicted
            
            if(!HasUndecidedCell()) return StepResult.Finished; // Done in this step
            return StepResult.Running; 

        }
        private bool HasUndecidedCell()
        {
            foreach (var cell in Grid)
                if(!cell.IsCollapsed && !cell.IsContradiction) return true;
            return false;
        }

        private void Propagate(Cell startCell)
        {
            int poppedCells = 0;
            int removedTiles = 0;
            Stack<Cell> stack = new Stack<Cell>();
            stack.Push(startCell);
            bool contradiction = false;
            while (stack.Count > 0 && !contradiction)
            {
                Cell current = stack.Pop();
                poppedCells +=1;

                foreach(Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Cell neighbor = GetNeighbor(current, dir);
                    if(neighbor == null || neighbor.IsCollapsed) continue;

                    var validNeighborTiles = neighbor.PossibleTiles.Where(neighborTile =>
                    current.PossibleTiles.Any(myTile => myTile.CanConnectTo(neighborTile, dir))).ToList();

                    if(validNeighborTiles.Count < neighbor.PossibleTiles.Count){
                        //check for contradiction
                        if (validNeighborTiles.Count == 0) {
                            contradiction = true;
                            ContradictionCell = neighbor;
                        }
                        removedTiles += neighbor.PossibleTiles.Count - validNeighborTiles.Count;
                        neighbor.PossibleTiles.Clear();
                        neighbor.PossibleTiles.AddRange(validNeighborTiles);
                        stack.Push(neighbor);
                    }
                }
            }
            LastCellsPopped = poppedCells;
            LastTilesRemoved = removedTiles;
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