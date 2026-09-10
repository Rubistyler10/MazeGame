# TO DO
- [x] Player base script
- [x] Maze base script
- [ ] Copy example mazes
- [ ] Implement Game visualization and Management
    - GameManager must be a GameObject in the scene that is tasked with creating [Game] objects and the rest of dependencies, assign them a chosen [Maze] and [Player] and spawn a visualization of them in the game world.
    - **Idea**: After every step, record previous and current states of the board and chosen Action. This way an animation can be played even on invalid actions.
- [ ] HumanPlayer script using Unity inputs instead of console commands.
- [ ] Camera adjusts to maze size and is centered.
- [ ] In-game menu for selecting Players, Mazes and starting games


# Scripts
## Game
_Game.cs_ controls the main flow of the game itself. It tells the other scripts when to participate in Step().
It also saves the path taken by the player of the current game.

Although the flow is controlled by Game, the decision to take a step or keep the flow paused is taken by the [GameManager]
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
