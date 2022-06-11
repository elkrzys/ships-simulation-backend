using System.Collections;
using ShipsSimulationBackend.Models;

namespace ShipsSimulationTests;

public class BoardTests
{
    [Test]
    public void MustInitializeBoardWithHundredEmptyFields()
    {
        //when
        var board = new Board();

        //expect
        Assert.NotNull(board.Fields);
        Assert.That(board.Fields.Select(field => field.State), Has.Exactly(100).EqualTo(FieldState.Empty));
    }

    [Test]
    public void MustReturnCorrectFieldOnPosition()
    {
        //given
        var board = new Board();
        var position = new Position(1, 1);

        //when
        var field = board.GetFieldOnPosition(position);

        //then
        Assert.AreEqual(position, field.Position);
    }

    [Test]
    public void MustReturnPositionNeighboursWhenNoBoundaries()
    {
        //given
        var board = new Board();
        var position = new Position(4, 4);
        var expectedNeighboursPositions = new List<Position>
        {
            new(3, 4),
            new(4, 3),
            new(4, 5),
            new(5, 4)
        };

        //when
        var actualNeighboursPositions =
            board.GetAvailableNeighboursFieldsForPosition(position).Select(field => field.Position);

        //then
        Assert.AreEqual(expectedNeighboursPositions, actualNeighboursPositions);
    }

    [Test, TestCaseSource(nameof(NeighboursTestCases))]
    public IEnumerable<Position> MustReturnPositionNeighboursWhenPositionIsNearBoundaries(Position position)
    {
        //given
        var board = new Board();

        //expect
        return board.GetAvailableNeighboursFieldsForPosition(position).Select(field => field.Position);
    }

    [Test]
    public void MustGetOnlyAvailableNeighbours()
    {
        //given
        var board = new Board();
        var position = new Position(1, 2);
        // set field (1, 1) as Hit
        board.Fields[0].State = FieldState.Hit;
        var expectedPositions = new List<Position> { new(1, 3), new(2, 2) };
        
        //when
        var actualPositions = board.GetAvailableNeighboursFieldsForPosition(position).Select(field => field.Position);
        
        //then
        Assert.AreEqual(expectedPositions, actualPositions);
    }

    [Test]
    public void MustGetFieldsBetweenPositions()
    {
        //given
        var board = new Board();
        var begin = new Position(1, 1);
        var end = new Position(1, 3);
        var expectedFieldsPositions = new List<Position> { new(1, 1), new(1, 2), new(1, 3) };

        //when
        var actualFieldsPosistions = board.GetFieldsBetweenPositions(begin, end).Select(field => field.Position);
        
        //then
        Assert.AreEqual(expectedFieldsPositions, actualFieldsPosistions);
    }
    
    [Test]
    public void MustGetFieldsOccupiedByShip()
    {
        //given
        var board = new Board();
        var ship = new Ship(ShipType.Submarine);
        var begin = new Position(1, 1);
        var end = new Position(1, 3);
        var expectedFields = board.GetFieldsBetweenPositions(begin, end);
        var anotherFields = board.GetFieldsBetweenPositions(new Position(5, 5), new Position(8, 5));
        expectedFields.ForEach(field => field.OccupyingShip = ship);
        anotherFields.ForEach(field => field.OccupyingShip = new Ship(ShipType.Destroyer));
        
        //when
        var actualFields = board.GetFieldsOccupiedByShip(ship);
        
        //then
        Assert.AreEqual(expectedFields.Select(field => field.Position), actualFields.Select(field => field.Position));
    }

    private static IEnumerable<TestCaseData> NeighboursTestCases
    {
        get
        {
            yield return new TestCaseData(new Position(1, 1)).Returns(new List<Position> { new(1, 2), new(2, 1) });
            yield return new TestCaseData(new Position(1, 10)).Returns(new List<Position> { new(1, 9), new(2, 10) });
            yield return new TestCaseData(new Position(10, 1)).Returns(new List<Position> { new(9, 1), new(10, 2) });
            yield return new TestCaseData(new Position(10, 10)).Returns(new List<Position> { new(9, 10), new(10, 9) });
            yield return new TestCaseData(new Position(1, 5)).Returns(
                new List<Position> { new(1, 4), new(1, 6), new(2, 5) });
        }
    }
}