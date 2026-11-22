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
}