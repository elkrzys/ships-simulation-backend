using ShipsSimulationBackend.Models;

namespace ShipsSimulationTests;

public class PlayerTests
{
    [Test]
    public void MustReturnRandomAvailableFieldPositionToFire()
    {
        // given
        var player = new Player();
        var opponentFields = player.OpponentBoard.Fields;
        opponentFields.ForEach(field => field.State = FieldState.Miss);
        opponentFields.First().State = FieldState.Empty;
        opponentFields.Last().State = FieldState.Empty;
        var expectedFieldsPositions = new List<Position> { opponentFields.First().Position, opponentFields.Last().Position };

        //when
        var actualPosition = player.Fire();
        
        //then
        Assert.Contains(actualPosition, expectedFieldsPositions);
    }
    
    [Test]
    public void MustReturnNeighbourPositionToFire()
    {
        // given
        var player = new Player();
        player.OpponentBoard.Fields[1].State = FieldState.Miss;
        player.ChangeOpponentFieldState(new Position(1, 1), FieldState.Hit);
        var expectedNeighbourPosition = new Position(2, 1);

        //when
        var actualPosition = player.Fire();
        
        //then
        Assert.AreEqual(expectedNeighbourPosition, actualPosition);
    }

    [Test]
    public void MustSetMissState()
    {
        //given
        var player = new Player();
        
        //when
        player.ProcessOwnFieldAndShipHit(new Position(1, 1));
        
        //then
        Assert.AreEqual(FieldState.Miss, player.OwnBoard.Fields.First().State);
    }

    [Test]
    public void MustSetHitState()
    {
        //given
        var player = new Player();
        var ship = new Ship(ShipType.Submarine);

        player.OwnBoard.Fields[0].OccupyingShip = ship;
        player.OwnBoard.Fields[0].State = FieldState.Occupied;
        player.OwnBoard.Fields[1].OccupyingShip = ship;
        player.OwnBoard.Fields[1].State = FieldState.Occupied;
        player.OwnBoard.Fields[2].OccupyingShip = ship;
        player.OwnBoard.Fields[2].State = FieldState.Occupied;
        
        //when
        var newState = player.ProcessOwnFieldAndShipHit(new Position(1, 1));
        
        //then
        Assert.AreEqual(FieldState.Hit, newState);
        Assert.AreEqual(FieldState.Hit, player.OwnBoard.Fields[0].State);
    }
    
    [Test]
    public void MustSetSunkState()
    {
        //given
        var player = new Player();
        var ship = PrepareSubmarineOnOwnBoard(player);

        player.ProcessOwnFieldAndShipHit(new Position(1, 1));
        player.ProcessOwnFieldAndShipHit(new Position(1, 2));

        //when
        var newState = player.ProcessOwnFieldAndShipHit(new Position(1, 3));
        
        //then
        Assert.AreEqual(FieldState.Sunk, newState);
        Assert.AreEqual(FieldState.Sunk, player.OwnBoard.Fields[0].State);
        Assert.AreEqual(FieldState.Sunk, player.OwnBoard.Fields[1].State);
        Assert.AreEqual(FieldState.Sunk, player.OwnBoard.Fields[2].State);
        Assert.True(ship.IsSunk);
    }

    [Test]
    public void MustReturnAllSunkPositionsFromFirstOne()
    {
        //given
        var player = new Player();
        PrepareSubmarineOnOwnBoard(player);
        
        var expectedSunkPositions = new List<Position> { new(1, 1), new(1, 2), new(1, 3) };
        player.ProcessOwnFieldAndShipHit(expectedSunkPositions[0]);
        player.ProcessOwnFieldAndShipHit(expectedSunkPositions[1]);
        player.ProcessOwnFieldAndShipHit(expectedSunkPositions[2]);
        
        //when
        var actualSunkPositions = player.GetAllSunkShipPositionsFromFirstSunkField(expectedSunkPositions[0]);
        
        //then
        Assert.AreEqual(expectedSunkPositions, actualSunkPositions);
    }
    
    [Test]
    public void MustSetOpponentSunkPositions()
    {
        //given
        var player = new Player();
        var expectedSunkPositions = new List<Position> { new(1, 1), new(1, 2), new(1, 3) };
       
        //when
        player.MarkOpponentSunkFieldsFromPositions(expectedSunkPositions);
        var actualSunkPositions = player.OpponentBoard.Fields
            .Where(field => field.State is FieldState.Sunk)
            .Select(field => field.Position);
        
        //then
        Assert.AreEqual(expectedSunkPositions, actualSunkPositions);
    }

    [Test]
    public void MustPlaceAllShipsOnBoard()
    {
        //given
        var player = new Player();
        
        //when
        player.PlaceShipsRandomly();
        var patrolBoatFields = player.OwnBoard.Fields.Where(field => field.State is FieldState.Occupied && field.OccupyingShip.Type is ShipType.PatrolBoat);
        var submarineFields = player.OwnBoard.Fields.Where(field => field.State is FieldState.Occupied && field.OccupyingShip.Type is ShipType.Submarine);
        var destroyerFields = player.OwnBoard.Fields.Where(field => field.State is FieldState.Occupied && field.OccupyingShip.Type is ShipType.Destroyer);
        var battleshipFields = player.OwnBoard.Fields.Where(field => field.State is FieldState.Occupied && field.OccupyingShip.Type is ShipType.Battleship);
        var carrierFields = player.OwnBoard.Fields.Where(field => field.State is FieldState.Occupied && field.OccupyingShip.Type is ShipType.Carrier);

        //then
        Assert.That(2, Is.EqualTo(patrolBoatFields.Count()));
        Assert.That(3, Is.EqualTo(submarineFields.Count()));
        Assert.That(3, Is.EqualTo(destroyerFields.Count()));
        Assert.That(4, Is.EqualTo(battleshipFields.Count()));
        Assert.That(5, Is.EqualTo(carrierFields.Count()));
    }

    private static Ship PrepareSubmarineOnOwnBoard(Player player)
    {
        var ship = new Ship(ShipType.Submarine);
        player.OwnBoard.Fields[0].OccupyingShip = ship;
        player.OwnBoard.Fields[0].State = FieldState.Occupied;
        player.OwnBoard.Fields[1].OccupyingShip = ship;
        player.OwnBoard.Fields[1].State = FieldState.Occupied;
        player.OwnBoard.Fields[2].OccupyingShip = ship;
        player.OwnBoard.Fields[2].State = FieldState.Occupied;
        return ship;
    }
}