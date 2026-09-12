# TO DO
- [x] Player base script
- [x] Maze base script
- [x] Copy example mazes
- [x] Implement Game visualization and Management
    - GameManager must be a GameObject in the scene that is tasked with creating [Game] objects and the rest of dependencies, assign them a chosen [Maze] and [Player] and spawn a visualization of them in the game world.
    - **Idea**: After every step, record previous and current states of the board and chosen Action. This way an animation can be played even on invalid actions.
- [x] HumanPlayer script using Unity inputs instead of console commands.
- [x] Swap the hardcoded Spacebar input for Stepping the game for an Input System approach
- [ ] Add list of players and mazes to GameManager for a [test_all] functionality.
- [x] Visualization has animation for bumping into a wall with a setting to make it optional
- [ ] Implement Data Gathering into csv files
- [ ] Finish Readme documentation

# C# Scripts
## Game
_Game.cs_ controls the main flow of the game itself. It tells the other scripts when to participate in Step().
It also saves the path taken by the player of the current game.

Although the flow is controlled by Game, the decision to take a step or keep the flow paused is taken by the [GameManager], as opposed to the loop in the original Python version.
The flow is:
1. First create a game with SetupGame(). Then, for every step:
2. [Observe] the state of the game.
3. Record the *observed* position of the player to the path history.
4. Ask the [Player] to choose an action.
    - The Player is tasked with the testing of actions and such.
    - The chosen Action the one that will be played for sure, no a test.  
5. Tell [ForwardModel] to apply the action to the [GameState].
6. Ask the [GameState] if the new state is terminal and record it in a variable.
7. If it is, record the iteration counter in case it's wanted later.

Step() returns the action chosen by the player for use in the Unity side of things.
> A special case has been allowed for a HumanPlayer. In Python, the simulation could be paused while waiting for a player action thanks to input(), but in Unity, we need Update() to keep running while checking for inputs. [Game] allows for a null action **only if the player is HumanPlayer**, which would make the character stay in place, without altering the [GameState], until the [Player] inputs an [Action].

## Game State
_GameState.cs_ contains the **true, real** state of the game. The [Game State] is what is actually happening.

When asked to update the [Player] position, it always ensures the new position is valid (Isn't out of the mazes bounds and isn't inside of a wall).

## Observation
_Observation.cs_ is an auxiliary class. It helps by creating a copy of the game state that is used for gathering information about it and testing out different possible actions. An [Observation] is a simulation of the [Game State]. Observations are expected to be discarded and recreated anew when the [Game State] changes.

It also can provide a list of available [Actions], but since actions are dictated by inputs, which are always available, and avoiding incorrect actions is required behaviour for a functioning agent, the list will always be all 4 directions of movement.

> Ideally [GameState] and [Observation] should inherit from a shared class, but alas, it wasn't done.

## Forward Model
_ForwardModel.cs_ is a tool for making changes in both the [Game State] and an [Observation].
It only holds 2 functions: Play() and Test(). They effectively do the same: apply a given [Action] to the [Game State] or an [Observation], respectively.

[ForwardModel] allows for impossible moves (E.q. moving *into* a wall). [GameState] is tasked with rejecting them.
> Since [ForwardModel is a tool for applying [Actions] to [GameState], it shouldn't know whether an action should be allowed or not. Impossible actions are rejected by [GameState] or avoided by the [Player].]

> The approach of having [ForwardModel] take care of applying [Actions] instead of giving [GameState] and [Observation] their own functions is just consolidation of tasks and project organization, not a strict requirement of the implementation.

## Action
_Action.cs_ defines the Action object that is used to represent a move in the game. In this version of the game it will always be 4 directions of movement that could be represented by an Enumerator, but this approach allows for expansion of the game's rules and is just as readable in code. 

# Maze
Holds the map of the playable field and methods to obtain information about it.
As in the original Python version, every [Maze] is its own script that inherits from the original [Maze] script, with the only changes being the information about the map in the *Initialize()* method and the override of the ToString() method with the new name. All of the information about the maze aside from that is automatically updated through methods. 

# Unity implementation
## Philosophy
All scripts have been ported to C# without any dependency for Unity, except for [Maze] and [Player], which need it for a comfortable swapping of different versions In-Editor.
Even in those cases, the changes have been minor to allow the project to easily work without Unity. The reason for this approach is an intention of keeping the Maze Game as similar as possible to its original Python version, using Unity as simply a method to more comfortably visualize the events.


## GameManager
_GameManager.cs_ is the tool for the creation of individual [Games], changing their initial settings within the editor, and creating Game World visualizations of said [Games].

It receives a [Maze] **ScriptableObject** *(that contains the information of the script used for its creation)* and a [Player] **ScriptableObject** *(same as the maze)*.
In the Inspector window you can set the `budget` and `max_iterations` settings for the [Game] logic.

The visualization works by creating a Maze **on the [GameManager] GameObject's location**. The GameObject that holds all of the tiles is a child of the GameObject holding the script for organization.
The created maze simply instatiates the selected asset corresponding each type of tile in order and spawns a player model in the starting tile.

For animation, [GameManager] records the position of the player before and after a Step and moves the model towards that location. The simulation cannot advance until the end of this animation, so a speed setting has been added for comfort.
Making a Step requires input (Spacebar), unless `Autoplay` is enabled, which makes a Step as soon as the option is available.
The animation system also allows for an optional behaviour for the cases where the [Player] tries to move to a wall and would normally stay in place, making it travel towards the wall and coming back after bumping against it.

Inspector fields for the assets used has been added in case you want to change the art.

### Inputs
[GameManager] also allows for inputs during gameplay to control the simulation, this inputs can be changed in the Input System Settings:
- `Spacebar` steps the simulation.
- `R` resets the simulation **only when the game has ended**
- `Shift + R` force resets the simulation at any point
- `A` toggles autoplay on and off
- `+` increases the animation speed
- `-` decreases the animation speed
These inputs affect **all** [GameManager] instances.


## Mazes
Maze layouts are ScriptableObject assets. To create a new maze:

1. Create a C# class that inherits from `Maze`.
2. Initialize its layout from `OnEnable()` with `Initialize(new int[,] { ... })`.
3. Select the `0_MazeAssetGenerator` asset in `Assets\Prefabs\Mazes\` to open its inspector window.
4. Drag the C# [Maze] script into its `Maze Script` field and click `Create Maze Asset`.
5. Set the output folder (default is the same folder the rest of [Maze] assets are in), then click `Create Maze Asset`.
6. Drag the generated `.asset` into the `Maze` field on the `GameManager`.

> **_AI DISCLAIMER_**: The `MazeScriptAssetGenerator.cs` script in `Assets\Editor\` has been written with the help of Generative AI. The decision in favour of this approach with the help of AI as opposed to the initial hand-crafted approach of making new [Maze] scripts and manually making prefabs with them as components has been taken due to in editor comfort and general good Unity development practices. Although the code has been written by Gen AI, it has been done under human supervision.

## Players
AI players use the same ScriptableObject workflow as the Mazes. 
1. Create a concrete class that inherits from `Player`.
2. Select the `0_PlayerAssetGenerator` asset in `Assets\Prefabs\Players\` to open its inspector window.
3. Drag the C# player script into its `Player Script` field and click `Create Player Asset`.
4. Set the output folder (default is the same folder the generator is in), then click `Create Player Asset`.
5. Drag the generated `.asset` into the `Player` field on the `GameManager`.

The visual `player_prefab` remains a separate scene object. It is only the
rendered player model; the generated Player asset contains the decision-making
logic. Override `Reset()` in stateful players when they need per-game cleanup.
> **_AI DISCLAIMER_**: As they use the same approach, the `PlayerScriptAssetGenerator.cs` script in `Assets\Editor\` has been written with the help of Generative AI, same as with the mazes. The decision in favour of this approach with the help of AI as opposed to the initial hand-crafted approach of making new [Player] scripts and manually making prefabs with them as components has been taken due to in editor comfort and general good Unity development practices. Although the code has been written by Gen AI, it has been done under human supervision.