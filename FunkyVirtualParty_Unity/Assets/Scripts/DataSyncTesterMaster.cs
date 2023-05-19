using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSyncTesterMaster : MonoBehaviour
{
    [SerializeField]
    private Color _color = default;
    private Color _previousColor = default;

    private DataSyncTester _colorSync;

    private void Awake()
    {
        // Get a reference to the color sync component
        _colorSync = GetComponent<DataSyncTester>();
    }

    private void Update()
    {
        // If the color has changed (via the inspector), call SetColor on the color sync component.
        if (_color != _previousColor)
        {
            _colorSync.SetColor(_color);
            _previousColor = _color;
        }
    }
}
