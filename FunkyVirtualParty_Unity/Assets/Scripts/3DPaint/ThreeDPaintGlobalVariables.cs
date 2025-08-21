using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDPaintGlobalVariables : MonoBehaviour
{
    //Every player that correctly guesses what the VR player's drawing is
    public const int NUMBER_OF_ROUNDS = 3;

    //Every player that correctly guesses what the VR player's drawing is
    public const int POINTS_VR_CORRECT_GUESSES = 25;

    //Correctly guessing which player wrote the chosen answer
    public const int POINTS_VR_CORRECT_PLAYER = 75;

    //Correctly guessing what the VR player drew
    public const int POINTS_CLIENT_CORRECT_GUESS = 100;

    //Client Correctly guessing what player wrote an answer
    public const int POINTS_CLIENT_CORRECT_PLAYER = 25;

    //The amount of time the VR player gets to draw
    public const int DRAW_TIME_AMOUNT = 120;

    //The amount of time clients get to answer the prompt
    public const int CLIENT_ANSWER_TIME_AMOUNT = 60;

    //The amount of time between animating each player answer and players that chose that answer in the results phase
    public const int PLAYER_ANSWER_ANIMATION_TIME = 5;

    //The amount of time the leaderboard is displayed for between rounds
    public const int LEADERBOARD_DISPLAY_TIME = 6;
}
