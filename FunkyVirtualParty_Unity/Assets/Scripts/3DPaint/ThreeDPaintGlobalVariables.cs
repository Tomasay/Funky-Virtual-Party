using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDPaintGlobalVariables : MonoBehaviour
{
    public const int NUMBER_OF_ROUNDS = 3;

    public const int MINIMUM_NUMBER_OF_PLAYERS = 3;

    //Correctly guessing which player wrote the chosen answer
    public const int POINTS_VR_CORRECT_PLAYER = 50;

    //Amount of points a client gets for having their guess selected as the prompt to draw
    public const int POINTS_CLIENT_SELECTED_PROMPT = 125;

    //Correctly guessing what the VR player drew
    public const int POINTS_CLIENT_CORRECT_GUESS = 100;

    //Bonus points for first client to correctly guess what the VR player drew
    public const int POINTS_CLIENT_FIRST_CORRECT_GUESS = 25;

    //Client Correctly guessing what player wrote an answer
    public const int POINTS_CLIENT_CORRECT_PLAYER = 50;

    //The amount of time the VR player gets to draw
    public const int DRAW_TIME_AMOUNT = 120;

    //The amount of time clients get to answer the prompt
    public const int CLIENT_ANSWER_TIME_AMOUNT = 60;

    //The amount of time between animating each player answer and players that chose that answer in the results phase
    public const int PLAYER_ANSWER_ANIMATION_TIME = 5;

    //The amount of time the leaderboard is displayed for between rounds
    public const int LEADERBOARD_DISPLAY_TIME = 6;


    //TOTAL Points that the VR player gets for clients that correctly guesses what the drawing is
    public static int calculatePointsVrCorrectGuesses(int correctGuesses)
    {
        float basePoints = (100f / ((float)ClientPlayer.clients.Count-1)) * correctGuesses;

        // Snap UP to nearest 25
        int snappedUp = Mathf.CeilToInt(basePoints / 25f) * 25;

        // Enforce minimum of 25
        return (correctGuesses <= 0) ? 0 : Mathf.Max(25, snappedUp);
    }
}
