using System.Collections.Generic;
using UnityEngine;

//This controlls all the objects in the scene and places objects/models depending on what is needed
public class ObjectManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject mesh;
    public GameObject currentMesh;
    public GardenID currentGarden;
    public Transform child;
    public GameObject farmbot;
    GameObject farmbotCopy;
    public GameObject UIObject;
    public GameObject lightController;
    public UIManager uIManager;
    
    [Header("Arrays")]
    public GardenID[] arrayOfGarden;
    public PlantID[] arrayOfPlantID;
    public GameObject[] arrayOfGameObjects;
    public GameObject[] arrayOfPlantObjects;

    [Header("Garden Size")]
    public float X, Z;
    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        uIManager.FoldersAndModels();
        

        CreateModelOnStart();
        uIManager.PopulateDropdown(arrayOfGameObjects);
        uIManager.ReadCSVFile(uIManager.dir + "/", currentMesh.name);
        uIManager.GardenSizeCSV(uIManager.dir + "/", currentMesh.name);
        createPlane(X, 1, Z);
        GardenAndPlants();
        uIManager.defaultTransform();
    }

    public void CreateModel()
    {
        //Creates objects/models on start
        Destroy(mesh);
        Destroy(farmbotCopy);
        CreateModelOnStart();
        uIManager.GardenSizeCSV(uIManager.dir + "/", currentMesh.name);
        createPlane(X, 1, Z);
    }

    public void CreateModelOnStart()
    {
        //Creates objects when needed or called
        Quaternion spawnRotation = Quaternion.Euler(90, 0, 0);
        Vector3 position = new Vector3(X / 20, 5, Z / 20);
        for (int i = 0; i < arrayOfGarden.Length; i++)
        {
            if (uIManager.gardens.value == i)
            {
                foreach (var gar in arrayOfGarden)
                {
                    if(arrayOfGarden[i].gardenMesh == gar.gardenMesh)
                    {
                        currentGarden = arrayOfGarden[i];
                    }
                }

                GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                currentMesh = arrayOfGameObjects[i];
                mesh = Instantiate(arrayOfGameObjects[i], this.transform) as GameObject;
                mesh.SetActive(true);
                //mesh.transform.SetParent();

                child = mesh.gameObject.transform.GetChild(0);
                child.transform.rotation = spawnRotation;
                child.transform.localScale = GardenSize(X, Z);
                child.gameObject.AddComponent<BoxCollider>().enabled = false;
                child.gameObject.layer = 8;

                Renderer objectMesh = child.gameObject.GetComponent<MeshRenderer>();

                testCube.transform.localPosition = objectMesh.bounds.center;
                child.transform.SetParent(testCube.transform);
                testCube.transform.localPosition = position;
                testCube.transform.SetParent(mesh.transform);
                testCube.GetComponent<MeshRenderer>().enabled = false;

                lightController.transform.position = testCube.transform.position;
            }
        }
        uIManager.Load();
    }

    public void GardenAndPlants()
    {
        //This finds all plants related to the current garden via the gardenID
        currentGarden = null;
        for (int i = 0; i < arrayOfGarden.Length; i++)
        {
            if (arrayOfGarden[i].gardenMesh == currentMesh)
            {
                currentGarden = arrayOfGarden[i];
            }
        }

        if(arrayOfPlantID != null)
        {
            CreateInteractivePlant();
        }
    }

    public void CreateInteractivePlant()
    {
        List<GameObject> refresh = new List<GameObject>();
        //Creates the plants interactive UI cubes (Just regular cubes for now)
        for (int i = 0; i < arrayOfPlantID.Length; i++)
        {
            Vector3 Test2 = new Vector3(arrayOfPlantID[i].xCoord / 10, 0, arrayOfPlantID[i].yCoord / 10);
            GameObject test = Instantiate(UIObject) as GameObject;
            
            test.transform.localPosition = Test2;
            test.transform.SetParent(mesh.transform);
            test.GetComponent<Interactable>().thisPlant = arrayOfPlantID[i];
            test.gameObject.SetActive(true);
            refresh.Add(test);
        }
        arrayOfPlantObjects = refresh.ToArray();
        refresh.Clear();
        arrayOfPlantID = null;
    }

    void createPlane(float Scale_x, float Scale_y, float Scale_z)
    {
        Vector3 farmscale = new Vector3(Scale_z / 10 , Scale_x /10, 123);
        Vector3 position = new Vector3(Scale_x / 20, 0, Scale_z / 20);
        Quaternion spawnRotation = Quaternion.Euler(-90, 0, 90);

        farmbotCopy = Instantiate(farmbot, position, spawnRotation) as GameObject;
        farmbotCopy.transform.localScale = farmscale;
    }

    Vector3 GardenSize(float Scale_x, float Scale_y)
    {
        Vector3 size = new Vector3(17.5f, 17.5f, 17.5f);
        return size;
    }

    
}
