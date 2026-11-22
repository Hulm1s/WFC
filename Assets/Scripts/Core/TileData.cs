namespace WFC.Core
{
    public enum Direction
    {
        North = 0, East = 1, South = 2, West = 3
    }
    public class TileData
    {
        public string Id {get; private set; }
        public char AsciiSymbol {get; private set;}
        public int[] Sockets {get; private set;}
    
        public TileData(string id, char symbol, int north, int east, int south, int west)
        {
            Id = id;
            AsciiSymbol = symbol;
            Sockets = new int[] { north, east, south, west };
        }
        public bool CanConnectTo(TileData other, Direction dir)
        {
            int mySocket = Sockets[(int)dir];
            int oppositeDir = ((int)dir + 2) % 4;
            int otherSocket = other.Sockets[oppositeDir];

            return mySocket == otherSocket;

        }
    }
}

