using System.Collections.Generic;
using NUnit.Framework;
using WFC.Core;

public class WfcGridTests
{
    // A copy of the full tile set, so the tests don't depend on the Visualiser
    private List<TileData> FullSet() => new List<TileData>
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

    // Helper: run the grid to the end and return how it ended
    private StepResult RunToEnd(WfcGrid grid)
    {
        StepResult result = StepResult.Running;
        while (result == StepResult.Running) result = grid.Step();
        return result;
    }

    [Test]
    public void SameSeed_GivesSameGrid()
    {
        var tiles = FullSet();
        var a = new WfcGrid(10, 10, tiles, 42);
        var b = new WfcGrid(10, 10, tiles, 42);
        RunToEnd(a);
        RunToEnd(b);

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                Assert.AreEqual(a.Grid[x, y].PossibleTiles[0].Id, 
                                b.Grid[x, y].PossibleTiles[0].Id);
    }


    [Test]
    public void FinishedGrid_BreaksNoRule()
    {
        var tiles = FullSet();
        var a = new WfcGrid(10, 10, tiles, 42);
        var result = RunToEnd(a); 
        Assert.AreEqual(StepResult.Finished, result);
        for(int x = 0; x < a.Width; x++ ){
            for(int y = 0; y < a.Height; y++){
                TileData current = a.Grid[x,y].PossibleTiles[0];
                if(x < a.Width-1)
                    Assert.AreEqual(a.Grid[x+1,y].PossibleTiles[0].Sockets[3],current.Sockets[1], $"east mismatch at {x}, {y}");
                if(y < a.Height-1)
                    Assert.AreEqual(a.Grid[x,y+1].PossibleTiles[0].Sockets[2], current.Sockets[0], $"north mismatch at {x}, {y}");
            }
        }



    }


    [Test]
    public void OneByOne_FinishesInOneStep()
    {
        var tiles = FullSet();
        var a = new WfcGrid(1, 1, tiles, 42);
        
        StepResult result = a.Step();
        Assert.AreEqual(StepResult.Finished, result);
    }
}