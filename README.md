# ships-simulation-backend

## C# Backend application for Battleship board game simulation

The simulation is an implementation of a web application in .NET 6.0.

It consists of one solution with simulation project and unit tests project.

Tests are implemented with NUnit and cover main Player and Boards functionalities.

## Game structure

The game is designed following rules from https://en.wikipedia.org/wiki/Battleship_(game).

### Ships

Ships are represented by class `Ship` containing type of ship (Enum ShipType) and Size. Ship has also properties Hits representing the sum of aimed shots and IsSunk which returns true when Hits is equal to the ship size.

### Boards

The board consists of 100 Fields with Positions `(row, column)` and one of 5 States: `Empty, Miss, Occupied, Hit, Sunk`. Structure used to store Fields is `List`.

Each field contains information about Ship that is occupying that field. If the field isn't occupied, its `State` is `Empty` and `OccupingShip` is `null`.

There are two players having two boards each:

- OwnBoard stores information about player's own ships and field states.
- OpponentBoard stores information about hits, misses and sunk fields on fields and ships.

### Ships placement

Each player places ships randomly. For each ship there are generated beginPositon and endPosition which represent where the ship should be placed on the board. If fields between (including begin and end) positions are available, the ship is placed and fields are updated. This step is retried for each ship until all ships are placed.

In v1.0 simulation placed ships without any space between them - it caused visual effect of huge ships on the board. The issue was fixed in v1.1 - ships are placed randomly, but one ship can be placed nearby another only if between them is space of at least width one empty field. It also aplies to the corner neighbours of the ship.

### Firing strategy

Each player plays a round. Player who starts is randomly selected with a 50% chance. Round consists of a single shot, when players misses, round goes to the opponent. If player hits a ship, he fires again until the result is `Miss`.
All the shots in series are counted to one round.

When player hits a ship, simulation gets all existing hits from the OpponentBoard and randomly gets one neighbour (from avaiable fields). When there are no hits available (e.g after sunk), the shot position is generated randomly.

Field defined as available is a field, that is not `Hit` or `Sunk`. Opponent must react to fired position by marking new field state. Eventually when ship is sunk, all related fields are marked as sunk. Response of the opponent reaction is the new state of field shot by player. Then, player marks the response on his OpponentBoard.

Also, finding all remaining hits was a solution to the problem, where there was a need to remember last hit position and after that there was a miss or there was no available neighbour. In this situation, the player would have to guess the position again. With all remaining hits there is no need to remember the sequence of hits which simplifies firing algorithm, because we always know where is a hit that has available neighbours.

### Game result

The game checks if Player1 or Player2 has all the ships sunk. If player returns true when `IsLost` is called, the game loop is broken and game finishes by setting the winner as the player who didn't lose.
The endpoint `/game/simulate` is a POST method which starts the simulation and returns the result containing the Beginner, Winner and both Player objects with their Boards.
