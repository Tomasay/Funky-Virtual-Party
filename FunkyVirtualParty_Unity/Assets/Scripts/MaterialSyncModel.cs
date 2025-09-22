using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class MaterialSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _setColor; //"parameterName,#HTMLStringRGB"

    [RealtimeProperty(2, true, true)]
    private string _setColorWithTween; //"parameterName,#HTMLStringRGB,durationInSeconds"

    [RealtimeProperty(3, true, true)]
    private string _setSecondColorWithTween; //Second color param to prevent desync when changing multiple color vals
}