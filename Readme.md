# Summary
> This Summary has been written by Generative AI

MazeGame is a Unity-based framework for simulating, testing, and visualizing maze-navigation games. A game consists of a maze, a player, a current game state, and a sequence of actions. The project separates these concepts so that the core simulation can remain mostly independent of Unity, while Unity provides convenient asset management, scene visualization, animation, and interactive controls.

The simulation is organized around the following components:

- **Maze** defines the playable grid, including its dimensions, walls, and traversable cells.
- **Player** chooses actions based on an observation of the current state. Players may be human-controlled or implemented as reusable AI strategies.
- **Action** represents a requested movement, currently one of the four cardinal directions.
- **GameState** stores the authoritative state and validates updates such as player movement.
- **Observation** provides a disposable copy of the state that players can inspect and use to test possible actions without changing the real game.
- **ForwardModel** applies actions to either the real state or an observation.
- **Game** coordinates setup, observation, action selection, state updates, terminal-state detection, and path recording.

Within Unity, `GameManager` creates and visualizes an individual game. It can advance the simulation one step at a time, run it automatically, animate player movement, reset the game, and optionally show a wall-bump animation when an invalid move is attempted. `MultiTester` runs repeated combinations of configured players and mazes, making it possible to compare strategies across multiple trials without manually recreating each game.

Mazes and are stored as `ScriptableObject` assets. This allows configurations to be created, reused, and swapped directly in the Unity Editor. The project also supports human input through Unity's Input System, with configurable controls for stepping, resetting, autoplay, animation speed, and advancing through multi-game test runs.

MazeGame is primarily intended for editor-based experimentation, debugging, and visualization rather than distribution as a standalone build. The Scene view and Unity Inspector are the main tools for configuring and observing simulations; in-game menus and a polished player-facing interface are not currently goals of the project.

# AI DISCLAIMER
Unless explicitly disclosed, all text in this Readme and all code has been written by a human, which is me. Direct any praise and criticism to the person behind the project. I take pride in doing my own work. Thank you. :D

# Workflow
> This Unity project is intended to be used mostly in the `Scene` window and does not expect to ever be turned into a proper build. The tools made for testing and visualizing all live in the editor and In-Game tools or menus are not planned.

To create new [Players] or [Mazes]: Check their respective sections in the _Readme_.

To test several [Players] and [Mazes]:
1. Add a [MultiTester] prefab to project hierarchy.
    - 2 prefabs are already provided: one empty and one with [RandomPlayer] and all [Mazes] for you to try.
2. Fill any empty fields in the [MultiTester] with your desired data (select or drag new [Mazes] and [Players] to the list) and adjust the settings to your preference.
3. Press play in the editor.
4. Use the keybinds or the options in the [MultiTester] inspector window to change any settings during the game.

To test a single pairing of [Player] and [Maze]:
1. Add a [GameManager] prefab to project hierarchy.
2. Fill any empty fields in the [GameManager] with your desired data (select or drag a [Maze] and a [Player] to the Inspector fields) and adjust the settings to your preference.
    - Remember to set `Standalone` in `Game Visualization Settings` to true for inputs and [HumanPlayer] to work.
3. Press play in the editor.
4. Use the keybinds or the options in the [MultiTester] inspector window to change any settings during the game.

# TO DO
- [x] Player base script
- [x] Maze base script
- [x] Copy example mazes
- [x] Implement Game visualization and Management
    - GameManager must be a GameObject in the scene that is tasked with creating [Game] objects and the rest of dependencies, assign them a chosen [Maze] and [Player] and spawn a visualization of them in the game world.
    - **Idea**: After every step, record previous and current states of the board and chosen Action. This way an animation can be played even on invalid actions.
- [x] HumanPlayer script using Unity inputs instead of console commands.
- [x] Swap the hardcoded Spacebar input for Stepping the game for an Input System approach
- [x] Implement a multi-instancer
- [ ] Implement multiple concurrent visualizations for the multitester.
- [x] Visualization has animation for bumping into a wall with a setting to make it optional
- [ ] Implement Data Gathering into csv files
- [x] Finish Readme documentation
- [ ] Add a setter function to the game settings in multitester to allow real-time change of settings through inspector and not just keybinds

## Bugs
- [x] Dead Player instance spawns wrong.
- [x] Fix HumanPlayer
- [x] Add Clone() to Observation
- [x] Change Players to MonoBehaviour
    - [ ] Update Readme with the new info on how to use them

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

This Unity project is intended to be used mostly in the `Scene` window and does not expect to ever be turned into a proper build. The tools made for testing and visualizing all live in the editor and In-Game tools or menus are not planned.

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

## MultiTester
_MultiTester.cs_ is the tool for running a battery of GameManagers. It is the equivalent of the original `main_all.py`.
It receives a list of [Players] and [Mazes] and runs every [Player] on every [Maze] for a set number repetitions.

By default, every [GameManager] instance is deleted on game end. This can be toggled on and off.
By default, when a [Player] dies during repetitions on a [Maze], the ghost will stay visible. This can be toggled on and off.

The implementation creates every new game when needed to avoid creating too many GameObjects at once. It does this by keeping count of the index of both [Player] and [Maze] lists, as well as the repetition number for the current pairing.

### Inputs
[GameManager] and [MultiTester] also allow for inputs during gameplay to control the simulation, this inputs can be changed in the Input System Settings:
- `Spacebar` steps the simulation.
- `R` resets the simulation **only when the game has ended**
- `Shift + R` force resets the simulation at any point
- `A` toggles autoplay on and off
- `[+]` increases the animation speed
- `Shift + [+]` increases the animation speed by a bigger step
- `[-]` decreases the animation speed
- `Shift + [-]` decreases the animation speed by a bigger step
- `Enter` simulates the next game in [MultiTester] when the current one has ended. (Only if `auto_start_next_game_on_end` option is off).
By default [GameManager] doesn't receive inputs as they should be handled by the [MultiTester], which will update the settings for both the current and all next [GameManager] instances.
These inputs affect **all** [GameManager] instances in case there are several concurrent ones.

Both [GameManager] and [MultiTester] inherit from the base class _GameSimulator.cs_, to ensure their input result methods are callable by the handler.
Inputs are handled by _InputHandler.cs_, which has to be set up to know which script it need to call to apply the input.
> Originally [GameManager] and [MultiTester] didn't inherit from [GameSimulator] and [InputHandler] didn't exist, making each of them to have their own input checks and calls. This has been changed to reduce duplicate code and create a clearer call hierarchy.


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
AI Players work similarly to how they did in the Python version, with minor changes for Unity use comfort:
- They still are inheritors of a base [Player] class, with their `Think()` method.
- They are now also [MonoBehaviours], which the base class inherits and passes to its children. This allows for a few things:
    - Inspector field editing, to set any initial atributes you may want (like Heuristics, for example)
    - Access to the `Start()` or `Update()` methods, which [HumanPlayer] directly required in way or another to handle inputs.

To add them to a [MultiTester] or [GameManager], you can just choose or drag the script to the field. If you want to have more control over the initial atributes, create a prefab of an empty object with the script as component and set the info.
