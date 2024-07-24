using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KaijuHealthSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    KaijuBehavior kaijuBehavior;
    // Start is called before the first frame update
    void Start()
    {
        kaijuBehavior = FindFirstObjectByType<KaijuBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = kaijuBehavior.health;
    }
}
