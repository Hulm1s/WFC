using System.Collections.Generic;

namespace WFC.Core{
    public class Cell{
        public int X {get; private set;}
        public int Y {get; private set;}
        public List<TileData> PossibleTiles {get; private set;}

        public bool IsCollapsed => PossibleTiles.Count == 1;
        public bool IsContradiction => PossibleTiles.Count == 0;
        public int Entropy => PossibleTiles.Count;

        public Cell(int x, int y, List<TileData> allTiles){
            this.X = x;
            this.Y = y;

            this.PossibleTiles = new List<TileData>(allTiles);
        }

        public void Collapse(System.Random rng)
        {
            int index = rng.Next(PossibleTiles.Count);
            TileData chosen = PossibleTiles[index];
            PossibleTiles.Clear();
            PossibleTiles.Add(chosen);
        }

    }


}
