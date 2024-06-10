using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class VRtistrySyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _state;

    [RealtimeProperty(2, true, true)]
    private int _timer;

    [RealtimeProperty(3, true, true)]
    private string _currentPrompt; //The current prompt that was chosen from list of answers

    [RealtimeProperty(4, true, true)]
    private string _answers; //list of client answers separated by \n, format is "CLIENT_ID:CLIENT_ANSWER"

    [RealtimeProperty(5, true, true)]
    private int _playersGuessed; //The amount of players who have guessed what the drawing is in the current phase

    [RealtimeProperty(6, true, true)]
    private int _chosenAnswerOwner; //Client ID of who's answer was chosen to give to the VR player as a prompt

    [RealtimeProperty(7, true, true)]
    private bool _isPainting; //Is the VR player spraying paint

    [RealtimeProperty(8, true, true)]
    private bool _isDrawing; //Is the VR player drawing with the pen

    [RealtimeProperty(9, true, true)]
    private Vector3 _drawingIncrement; //The latest point from pen drawing lines

    [RealtimeProperty(10, true, true)]
    private bool _isPenEnabled; //Is the pen enabled and in the VR player's hand? If false, the spray gun is

    [RealtimeProperty(11, true, true)]
    private Color _penColor;

    [RealtimeProperty(12, true, true)]
    private Color _sprayGunColor;

    [RealtimeProperty(13, true, true)]
    private bool _isPaletteMirrored;

    [RealtimeProperty(14, true, true)]
    private int _vrPlayerGuess; //The client ID that the vr player thinks wrote the answer

    [RealtimeProperty(15, true, true)]
    private int _vrPlayerPoints;

    [RealtimeProperty(16, true, true)]
    private bool _vrCompletedTutorial;

    [RealtimeProperty(17, true, true)]
    private string _guesses; //list of client guesses separated by \n, format is "CLIENT_ID:CLIENT_ANSWERID"

    [RealtimeProperty(18, true, true)]
    private float _clientAnswerTimer; //Time left for clients to submit an answer to the prompt

    [RealtimeProperty(19, true, true)]
    private float _drawingTimer; //Time left for vr player to finish drawing

    [RealtimeProperty(20, true, true)]
    private bool _isPaletteEnabled; //Is the palette enabled and visible. Set when vr player grabs/drops
}


