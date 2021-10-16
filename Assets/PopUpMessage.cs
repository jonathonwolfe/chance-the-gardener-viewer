using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMessage : MonoBehaviour
{
    public Text message;
    float timeUntilHidden = 2;
    private void Start()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 pos = message.transform.position;
        pos.y = mousePos.y;
        
        message.transform.position = pos;
    }

    private void Update()
    {
        timeUntilHidden = timeUntilHidden - Time.deltaTime;

        if(timeUntilHidden <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
