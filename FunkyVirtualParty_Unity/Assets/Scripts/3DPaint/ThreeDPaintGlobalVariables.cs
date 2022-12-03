using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDPaintGlobalVariables : MonoBehaviour
{
    //Every player that correctly guesses what the VR player's drawing is
    public const int NUMBER_OF_ROUNDS = 3;

    //Every player that correctly guesses what the VR player's drawing is
    public const int POINTS_VR_CORRECT_GUESSES = 50;

    //Correctly guessing which player wrote the chosen answer
    public const int POINTS_VR_CORRECT_PLAYER = 50;

    //Correctly guessing what the VR player drew
    public const int POINTS_CLIENT_CORRECT_GUESS = 100;

    //Every player the successfully chose the chosen player's answer
    public const int POINTS_CLIENT_OTHER_CORRECT_GUESSES = 50;
}
