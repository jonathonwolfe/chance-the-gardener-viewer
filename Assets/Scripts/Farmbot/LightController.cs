using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightController : MonoBehaviour
{
    public GameObject pivot;
    public GameObject light;
    public Vector3 rotation;

    public Slider SunY, SunZ;
    public Text SunYText, SunZText;

    private void Start()
    {
        SunY.maxValue = 360;
        SunY.minValue = 0;

        SunZ.maxValue = 180;
        SunZ.minValue = 0;
    }

    private void Update()
    {
        light.transform.LookAt(transform);
        pivot.transform.rotation = Quaternion.Euler(rotation);
        rotation.y = SunY.value;
        rotation.z = SunZ.value;
        
        SunYText.text = rotation.y.ToString("f0");
        SunZText.text = rotation.z.ToString("f0");
    }

}
