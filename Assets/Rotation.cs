using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 0, 70 * -Time.deltaTime);
    }
}
