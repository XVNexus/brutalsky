## *Nightly δ24.06.02*

#### UPDATES
- removed logic system


## *Nightly δ24.05.28*

#### UPDATES
- parkour platforms now have slightly lower height variance
- parkour platform ice chance now increases more linearly with difficulty


## *Nightly δ24.05.27*

#### UPDATES
- player on player collisions now deal damage based on relative speed instead of real impact force (which means that being hit at 20 m/s in air will do the same damage as being slammed against the wall at 20 m/s, fixing the op wall slam issue)
- logic system now uses string ids instead of int indices for nodes


## Dev μ0.2 (2024-05-18)

#### BUGFIXES
- round restarted when a player was unregistered but did not actually die


## *Nightly δ24.05.17*

#### UPDATES
- spacing between first platform and spawn and last platform and goal is more consistent with the spacing between other platforms in parkour maps

#### BUGFIXES
- players did not collide with map objects
- parkour finish platform was larger than the start and goal platforms


## *Nightly δ24.05.16*

#### UPDATES
- made parkour platforms more consistently spaced
- parkour platforms now decrease in size linearly with each level down to 2 units at maximum difficulty
- reduced chance of ice platforms spawning on low to mid level parkour maps
- added more visual detail to parkour platforms
- added subtle background coloring to parkour maps from blue to red indicating the difficulty level
- touching the spike under a parkour platform now instantly kills players
- changed shape of sensors and goals to squares to allow for more precise sizing
- parkour map goals now cover the entire height of the beacon for the sake of visual consistency
- reduced width of parkour goal hitboxes so player contact occurs in the middle of the goal platform instead of on the edge
- parkour platforms are now snapped to the grid
- parkour platform beacons are now in the background instead of the foreground
- split shapes into shapes and decals, the latter taking the place of a shape with simulated set to false
- replaced background/midground/foreground layer system with signed byte, allowing up to 128 background layers and 127 foreground layers
- removed simulated setting and some other useless variables for objects
- lcs string parser now uses implicit typing instead of explicit type tags for properties, resulting in cleaner syntax for config file and map files


## *Nightly δ24.05.15*

#### UPDATES
- increased number of parkour levels from 5 to 12
- parkour maps now greatly increase in difficulty with each level
- replaced rectangular parkour platforms with spike platforms
- added beacons to parkour start, platforms, and goal
- added suspension and body shape for car on car map
- added config option to control whether or not to automatically restart the level on last man standing
- added config options to disable player 1 or player 2 if desired
- player mounts now use a less aggressive clamping method because the old method was causing issues with the car
- improved ui style
- reduced performance impact of non-simulated objects

#### BUGFIXES
- round restarted any time a player died due to all players being assumed dead at all times
- multiple copies of the same player could be spawned which caused a lot of second-hand glitches
- spawns were drawn incorrectly in level previews
- some objects did not respect the simulated setting


## *Nightly δ24.05.14*

#### UPDATES
- added config system with basic graphics and gameplay settings
- added ui page for changing settings on the config system
- added in-game guide (copied from readme page on github)
- limited player acceleration due to ground friction to 2x
- added shockwave to death explosion
- increased camera shake on death explosion
- player health indicator now uses exponential interpolation instead of linear interpolation
- lowered speed threshold for boost particles

#### BUGFIXES
- lcs string files contained excessive property separators


## *Nightly δ24.05.13*

#### UPDATES
- added controls for player 2
- updated car on car map to have larger wheels and more ground clearance
- players now accelerate on the ground proportional to the surface friction
- decreased maximum movement speed of players
- increased speed required for player boost particles to show
- made map background affected by player lights again
- simplified tags for lcs signed integer types

#### BUGFIXES
- player input on one keyboard got shut off for one frame when a button was pressed on another keyboard


## *Nightly δ24.05.12*

#### UPDATES
- limited size of followcam view to size of map
- camera no longer follows targets outside of map bounds
- map background no longer reacts to player lights
- reformatted stringified lcs data to be more readable
- logic links are now serialized as 6 digit hexadecimal codes instead of two integers


## *Nightly δ24.05.11*

#### UPDATES
- added new default map with a working car that has a driver and passenger mount
- added checkerboard background with 1x1 and 10x10 meter squares for all maps to visualize movement better
- increased map size limit to 2.5 kilometers
- decreased size of void map to 250x250 meters
- slightly changed ui pane title border colors
- logic nodes for joints no longer continually output the joint's properties

#### BUGFIXES
- binary lcs parser did not allow strings and shape forms longer than 255 bytes


## *Nightly δ24.05.08*

#### UPDATES
- added particle effect for goals, previously known as redirects
- added options for player sensor to select whether or not to output a logic signal for enter, stay, and exit events
- added target leading for followcam so fast moving targets do not leave the frame
- increased size of void map to one square kilometer
- improved followcam leading algorithm so it doesn't overshoot very fast targets
- limited map size to one square kilometer
- made goals their own object instead of a standalone logic node

#### BUGFIXES
- camera did not adapt to screen aspect ratio automatically on maps without followcam
- followcam jolted when a player died close to another player
- logic links did not get saved with the map if there were no logic nodes


## *Nightly δ24.05.07*

#### UPDATES
- added procedurally generated platformer maps that increase in length with each level
- player mounts now use orthodox physics joints instead of rigid walls and tween animations
- the camera will now automatically zoom and pan on players on large maps
- player mounts now allow the player to dismount and stay dismounted without first leaving the mount's hitbox
- added ejection force option for player mounts
- map picker menu now sorts maps by default > custom > generated
- map system now generates maps on the spot instead of generating files and then loading those files back again
- added load in animation on game start

#### BUGFIXES
- players were immortal while on a player mount
- smooth cam jolted when changing aspect ratio from horizontal to vertical or vice versa relative to the screen's aspect ratio
- data folders did not get automatically created on game start


## *Nightly δ24.05.06*

#### UPDATES
- added ui buttons to restart the current round or refresh the map list from the level selector
- logic redirect node that switches to a different map when triggered
- player mounts that grab any nearby player and send their joystick input to the logic system
- ability to make objects children of other objects which causes them to be welded to their parent
- map loading gracefully fails when encountering bad map data

#### BUGFIXES
- the active map was reloaded from disk instead of reloaded from memory on map restart


## *Nightly δ24.05.05*

#### UPDATES
- round automatically restarts if there is one player remaining
- maps are now lazy loaded
- improved level transition to cover up lag spikes caused by loading a new map
- improved ui
- improved logic serialization
- added compressed binary lcs format
- lcs files now use .lcs and .lcb extensions in place of .txt and .bin


## *Nightly δ24.05.04*

#### UPDATES
- added level transition animations
- added logic player sensor
- ui automatically closes when a map is selected from the pause menu or level selector


## *Nightly δ24.05.03*

#### UPDATES
- added new default map
- added delay logic node
- improved default maps
- random generator logic nodes now only generate on input high
- shortened lcs object and addon tags

#### BUGFIXES
- logic was serialized even if no logic parts were present
- jointed adhesive shapes haphazardly teleported around when hit by the player


## *Nightly δ24.05.02*

#### UPDATES
- added node-based logic programming system


## *Nightly δ24.04.30*

#### UPDATES
- improved ui theme

#### BUGFIXES
- player death particles were invisible


## *Nightly δ24.04.29*

#### UPDATES
- improved map background
- improved default maps
- improved player heal/hurt particles


## *Nightly δ24.04.28*

#### UPDATES
- overhauled textures and map background
- added custom cursor

#### BUGFIXES
- objects were visible outside of map bounds


## *Nightly δ24.04.27*

#### UPDATES
- added bezier shape form node
- map files are now named by numeric id instead of title
- improved lcs file header

#### BUGFIXES
- one player pushing another caused glitchy movement


## *Nightly δ24.04.21*

#### UPDATES
- improved default maps
- improved joint handling
- improved shape adhesion handling


## *Nightly δ24.04.20*

#### UPDATES
- added map gravity setting
- improved default maps
- improved map previews

#### BUGFIXES
- players did not respawn correctly


## *Nightly δ24.04.19*

#### UPDATES
- added more map settings


## *Nightly δ24.04.14*

#### UPDATES
- added new experimental default map
- improved map previews

#### BUGFIXES
- map previews did not properly sort objects by their layer


## *Nightly δ24.04.13*

#### UPDATES
- added map previews in the level selector menu
- players can now unstick from glue by attempting to move away
- improved pool wave animations

#### BUGFIXES
- lcs parser did not properly handle child lines


## *Nightly δ24.04.12*

#### UPDATES
- added map background texture
- reduced map file size


## *Nightly δ24.04.11*

#### UPDATES
- reduced map file size
- increased player movement speed when on ground


## *Nightly δ24.04.07*

#### UPDATES
- improved color serialization


## *Nightly δ24.04.06*

#### UPDATES
- reformatted map files to line-based format to reduce file size


## *Nightly δ24.04.05*

#### BUGFIXES
- map ids did not generate correctly


## *Nightly δ24.04.02*

#### UPDATES
- reduced map file size


## *Nightly δ24.04.01*

#### UPDATES
- improved touch and slide particles


## Dev μ0.1 (2024-03-07)

#### UPDATES
- added map picker ui
- improved map loading and generation


## *Nightly δ24.03.02*

#### UPDATES
- added simple autogenerated maps
- improved particles
- improved camera shake
- improved map saving and loading


## *Nightly δ24.03.01*

#### UPDATES
- improved particles


## *Nightly δ24.02.29*

#### UPDATES
- improved physics and wave effects


## *Nightly δ24.02.28*

#### UPDATES
- added gravity
- added player jumping

#### BUGFIXES
- player death particles repeated every frame


## *Nightly δ24.02.24*

#### UPDATES
- added basic pause menu
- improved map and player handling
- improved ui style
- improved player damage particles


## *Nightly δ24.02.23*

#### UPDATES
- added map saving and loading


## *Nightly δ24.02.22*

#### UPDATES
- added advanced joint system
- added more map and shape lighting options


## *Nightly δ24.02.21*

#### UPDATES
- added reactive animated waves for pools


## *Nightly δ24.02.20*

#### UPDATES
- added adhesion material property
- added heal/hurt material property


## *Nightly δ24.02.16*

#### UPDATES
- added map save file system


## *Nightly δ24.02.15*

#### UPDATES
- improved damage system


## *Nightly δ24.02.14*

#### UPDATES
- added shape joints

#### UPDATES
- player speed is reduced on collision with another player
- improved default map


## *Nightly δ24.02.13*

#### UPDATES
- added default map
- added pools


## *Nightly δ24.02.12*

#### UPDATES
- added map and shape system


## *Nightly δ24.02.11*

#### UPDATES
- added basic player particle effects
- added camera shake


## *Nightly δ24.02.10*

#### UPDATES
- added player with indicator ring
