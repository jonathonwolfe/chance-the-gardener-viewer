using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gets and stores information for gardens
[CreateAssetMenu(menuName ="FarmBot/Garden")]
public class GardenID : ScriptableObject
{
    public string gardenID;
    public int gardenAge;
    public GameObject gardenMesh;

    private void OnEnable()
    {
        gardenID = this.name;
    }
}
