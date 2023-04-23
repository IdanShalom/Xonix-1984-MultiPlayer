# Project 3: 80’s

Screen resolution: 16x9
starting scene is "BeforeGame".

## Scope
FULL GAME OF XONIX

## Intervention
Multiplayer:
I made the game a multiplayer game. 
2 players play on the same screen. (each player with different color).
each player can conquer the other's sea. 
first player who conquer 50% of the screen , wins. 
when player hits another player (by body or by trail):
    game pauses.
    another screen is poppin up, and the players have 6 seconds to fight. the winner keeps going from the points 
    they stopped. looser is getting back to start point and looses his trail, as if he was hit by enemy. 
    in the fight, each player has to press his "right" or "left" keys, as much as he can in the 6 seconds. 
    the winner is the plyer who pressed more times of the other. 
    if there is a tie, the screen is resseting to another fight, untill someone wins.  
    game continues.


## Mechanics
- the scene is divided to "land" and "sea".
- Square that is the player character
     - MOVES ONLY UP, DOWN, RIGHT, LEFT (IN STRAIGHT LINES) inside the tiles.
     - WHEN ON "Land", CAN CLOSE AREAS BY CLOSING RECTANGLE WITH ITS TRAIL, AND MAKE IT "Sea" ** if there are enemies in both rectangles closed by player then only the trail becomes Sea.
- Balls that are the enemies - the balls (enemies) are in the land,  moving around freely, except for one that is moving freely in the "sea"
- player keeps moving untill another arrow was hit or a disqualification detected.
-disqualifications:
    1. player itself hit by enemy. 
    2. players trail hit by enemy while drawing. 
    3. player tries to move in the opposite direction while moving in the land area (while drawing).
    4. the player hits its own tail while in land. (while drawing it)
- players color changes in the land and changes back in the sea.





