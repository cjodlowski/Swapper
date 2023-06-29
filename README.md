# Swapper

## About
Gotta Update an About Here :P

---

## Update Log
### 06/29/23
 - Buzzsaw now rotates with slowed timescale 
 - Laser is now reworked to work with pause menu
 - UIProgress pauses when the games pause
### 06/28/23
 - Reworked some issue with Pausing before the round starts
 - Trying to make sure all coroutines are not running while the pause menu or game over sequence is occuring
 - Attempting to avoid timeScale but might still need to rework some things
 - Hopefully without breaking everything in the process
 - Updated the HandleEnd() method to pause and slow down time on the final blow

### 06/02/23
 - Workaround for InputSystem bug where WASD could not be used for UI and Player Movement at the same time
 - Created and hooked up PauseMenu Options and buttons
 - Created UI ActionMap
 - Made sure only the player who paused is allowed to control the Pause Menu
 - Set Navigation within the pause menu
 - Created an AudioMixer that controls Master, Music, and SFX Sounds separately
 - Added to PlayerPrefs / Configuration settings so that options are saved upon resuming the game
 

### 05/08/23
 - Fixed Players Respawning in the wrong area
 - Implemented some pause menu functionality
 - added paused gamestate to GameManager
 - added actions to inputactions
 - deleted controls file b/c i didnt see it getting used

### 05/03/23

- Organized Scene Objects 
- Swapped spawnpoints 1 and 4 on Map 1
- Moved spawnpoints on Map 2 to be closer to the points given on the Miro Board
- Continued to move assets into folders to be a bit more organized

