using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class ClientSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _name;

    [RealtimeProperty(2, true, true)]
    private Color _color;

    [RealtimeProperty(3, true, true)]
    private Color _nameColor;

    [RealtimeProperty(4, true, true)]
    private int _headType;

    [RealtimeProperty(5, true, true)]
    private float _height;

    [RealtimeProperty(6, true, true)]
    private int _hatIndex;

    [RealtimeProperty(7, false, true)]
    private float _animSpeed;

    [RealtimeProperty(8, true, true)]
    private bool _isReady;

    [RealtimeProperty(9, true, true)]
    private bool _onDeathTrigger; //Used for when player gets killed/knocked out in game. Triggers a callback and immediately gets set back to false

    [RealtimeProperty(10, true, true)]
    private bool _isDebugPlayer;

    [RealtimeProperty(11, true, true)]
    private int _isDancing; //Stores last dance anim. -1 if not dancing
}


