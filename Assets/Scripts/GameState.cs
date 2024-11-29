// GameState.cs
using System.Collections.Generic;
using UnityEngine;
public class GameState
{
    public Vector2 PlayerPosition { get; }
    public List<BoxState> BoxStates { get; }
    public GameState(Vector2 playerPosition, List<BoxState> boxStates)
    {
        PlayerPosition = playerPosition;
        BoxStates = boxStates;
    }
}
public class BoxState
{
    public Vector2 Position { get; }
    public BoxState(Vector2 position)
    {
        Position = position;
    }
}