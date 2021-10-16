using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gets and stores information of plants
[CreateAssetMenu(menuName = "FarmBot/Plant")]
public class PlantID : ScriptableObject
{
    public string plantID;
    public string plantName;
    public int plantAge;
    [TextArea]
    public string plantDesc;
    public string gardenID;
    public float xCoord, yCoord;
}
