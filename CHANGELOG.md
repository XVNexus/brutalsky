## *Nightly 2024.05.13*

#### ADDED
- controls for player 2

#### CHANGED
- updated car on car map to have larger wheels and more ground clearance
- decreased maximum movement speed of players
- increased speed required for player boost particles to show
- made map background affected by player lights again
- simplified tags for lcs signed integer types

#### FIXED
- input system shut down when the game was paused
- player input on one keyboard got shut off for one frame when a button was pressed on another keyboard


## *Nightly 2024.05.12*

#### CHANGED
- limited size of followcam view to size of map
- camera no longer follows targets outside of map bounds
- map background no longer reacts to player lights
- reformatted stringified lcs data to be more readable
- logic links are now serialized as 6 digit hexadecimal codes instead of two integers


## *Nightly 2024.05.11*

#### ADDED
- new default map with a working car that has a driver and passenger mount
- checkerboard background with 1x1 and 10x10 meter squares for all maps to visualize movement better

#### CHANGED
- increased map size limit to 2.5 kilometers
- decreased size of void map to 250x250 meters
- slightly changed ui pane title border colors
- logic nodes for joints no longer continually output the joint's properties

#### FIXED
- binary lcs parser did not allow strings and shape forms longer than 255 bytes


## *Nightly 2024.05.08*

#### ADDED
- particle effect for goals, previously known as redirects
- options for player sensor to select whether or not to output a logic signal for enter, stay, and exit events
- target leading for followcam so fast moving targets do not leave the frame

#### CHANGED
- increased size of void map to one square kilometer
- improved followcam leading algorithm so it doesn't overshoot very fast targets
- limited map size to one square kilometer
- made goals their own object instead of a standalone logic node

#### FIXED
- camera did not adapt to screen aspect ratio automatically on maps without followcam
- followcam jolted when a player died close to another player
- logic links did not get saved with the map if there were no logic nodes


## *Nightly 2024.05.07*

#### ADDED
- procedurally generated platformer maps that increase in length with each level
- ejection force option for player mounts
- load in animation on game start

#### CHANGED
- player mounts now use orthodox physics joints instead of rigid walls and tween animations
- the camera will now automatically zoom and pan on players on large maps
- player mounts now allow the player to dismount and stay dismounted without first leaving the mount's hitbox
- map picker menu now sorts maps by default > custom > generated
- map system now generates maps on the spot instead of generating files and then loading those files back again

#### FIXED
- players were immortal while on a player mount
- smooth cam jolted when changing aspect ratio from horizontal to vertical or vice versa relative to the screen's aspect ratio
- data folders did not get automatically created on game start


## *Nightly 2024.05.06*

#### ADDED
- ui buttons to restart the current round or refresh the map list from the level selector
- logic redirect node that switches to a different map when triggered
- player mounts that grab any nearby player and send their joystick input to the logic system
- ability to make objects children of other objects which causes them to be welded to their parent

#### CHANGED
- map loading gracefully fails when encountering bad map data

#### FIXED
- the active map was reloaded from disk instead of reloaded from memory on map restart


## *Nightly 2024.05.05*

#### ADDED
- compressed binary lcs format

#### CHANGED
- fight automatically restarts if there is one player remaining
- maps are now lazy loaded
- improved level transition to cover up lag spikes caused by loading a new map
- improved ui
- improved logic serialization
- lcs files now use .lcs and .lcb extensions in place of .txt and .bin


## *Nightly 2024.05.04*

#### ADDED
- level transition animations
- logic player sensor

#### CHANGED
- ui automatically closes when a map is selected from the pause menu or level selector


## *Nightly 2024.05.03*

#### ADDED
- new default map
- delay logic node

#### CHANGED
- improved default maps
- random generator logic nodes now only generate on input high
- shortened lcs object and addon tags

#### FIXED
- logic was serialized even if no logic parts were present
- jointed adhesive shapes haphazardly teleported around when hit by the player


## *Nightly 2024.05.02*

#### ADDED
- node-based logic programming system


## *Nightly 2024.04.30*

#### CHANGED
- improved ui theme

#### FIXED
- player death particles were invisible


## *Nightly 2024.04.29*

#### CHANGED
- improved map background
- improved default maps
- improved player heal/hurt particles


## *Nightly 2024.04.28*

#### ADDED
- custom cursor

#### CHANGED
- overhauled textures and map background

#### FIXED
- objects were visible outside of map bounds


## *Nightly 2024.04.27*

#### ADDED
- bezier shape form node

#### CHANGED
- improved lcs file header
- map files are now named by numeric id instead of title

#### FIXED
- one player pushing another caused glitchy movement


## *Nightly 2024.04.21*

#### CHANGED
- improved default maps
- improved joint handling
- improved shape adhesion handling


## *Nightly 2024.04.20*

#### ADDED
- map gravity setting

#### CHANGED
- improved default maps
- improved map previews

#### FIXED
- players did not respawn correctly


## *Nightly 2024.04.19*

#### ADDED
- more map settings


## *Nightly 2024.04.14*

#### ADDED
- new experimental default map

#### CHANGED
- improved map previews

#### FIXED
- map previews did not properly sort objects by their layer


## *Nightly 2024.04.13*

#### ADDED
- map previews in the level selector menu
- players can unstick from glue by attempting to move away

#### CHANGED
- improved pool wave animations

#### FIXED
- lcs parser did not properly handle child lines


## *Nightly 2024.04.12*

#### ADDED
- map background texture

#### CHANGED
- reduced map file size


## *Nightly 2024.04.11*

#### CHANGED
- reduced map file size
- increased player movement speed when on ground


## *Nightly 2024.04.07*

#### CHANGED
- improved color serialization


## *Nightly 2024.04.06*

#### CHANGED
- reformatted map files to line-based format to reduce file size


## *Nightly 2024.04.05*

#### FIXED
- map ids did not generate correctly


## *Nightly 2024.04.02*

#### CHANGED
- reduced map file size


## *Nightly 2024.04.01*

#### CHANGED
- improved touch and slide particles


## Dev 0.1 (2024.03.07)

#### ADDED
- map picker ui

#### CHANGED
- improved map loading and generation


## *Nightly 2024.03.02*

#### ADDED
- simple autogenerated maps

#### CHANGED
- improved particles
- improved camera shake
- improved map saving and loading


## *Nightly 2024.03.01*

#### CHANGED
- improved particles


## *Nightly 2024.02.29*

#### CHANGED
- improved physics and wave effects


## *Nightly 2024.02.28*

#### ADDED
- gravity
- player jumping

#### FIXED
- player death particles repeated every frame


## *Nightly 2024.02.24*

#### ADDED
- basic pause menu

#### CHANGED
- improved map and player handling
- improved ui style
- improved player damage particles


## *Nightly 2024.02.23*

#### ADDED
- map saving and loading


## *Nightly 2024.02.22*

#### ADDED
- advanced joint system
- more map and shape lighting options


## *Nightly 2024.02.21*

#### ADDED
- reactive animated waves for pools


## *Nightly 2024.02.20*

#### ADDED
- adhesion material property
- heal/hurt material property


## *Nightly 2024.02.16*

#### ADDED
- map save file system


## *Nightly 2024.02.15*

#### CHANGED
- improved damage system


## *Nightly 2024.02.14*

#### ADDED
- shape joints

#### CHANGED
- player speed is reduced on collision with another player
- improved default map


## *Nightly 2024.02.13*

#### ADDED
- default map
- pools


## *Nightly 2024.02.12*

#### ADDED
- map and shape system


## *Nightly 2024.02.11*

#### ADDED
- basic player particle effects
- camera shake


## *Nightly 2024.02.10*

#### ADDED
- player with indicator ring
