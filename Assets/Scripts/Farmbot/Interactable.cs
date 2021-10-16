using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Putting this on an object gives it a hit box for the interaction from the camera
public class Interactable : MonoBehaviour
{
    public float radius = 0.6f;
    public PlantID thisPlant;
    public GameObject UIObject;
    public GameObject debug;
    public GameObject line;
    float timeUntilBarIsHidden = 10;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public virtual void Interact()
    {
        InteractableUI UI = UIObject.GetComponentInChildren<InteractableUI>();
        UI.plantName.text = thisPlant.plantName;
        UI.plantAge.text = thisPlant.plantAge.ToString() + " days old";
        UI.plantDesc.text = thisPlant.plantDesc;
        UI.plantCoord.text = "X: " + thisPlant.xCoord + " Y: " + thisPlant.yCoord;
    }

    private void Update()
    {
        UIObject.transform.rotation = Camera.main.transform.rotation;
        timeUntilBarIsHidden = timeUntilBarIsHidden - Time.deltaTime;

        if (timeUntilBarIsHidden <= 0)
        {
            timeUntilBarIsHidden = 10;
            UIObject.SetActive(false);
            line.SetActive(false);
        }
    }
}