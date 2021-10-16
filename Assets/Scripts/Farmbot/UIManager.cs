using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Dummiesman;

//This just controls the UI elements
public class UIManager : MonoBehaviour
{
    #region UI Elements
    public Dropdown gardens;
    public Slider CamSpeed;
    public Slider CamSens;
    public Text CamSpeedText;
    public Text CamSensText;
    #endregion

    public ObjectManager objectManager;
    public FlyCamera flyCamera;

    List<PlantID> plantList = new List<PlantID>();
    [SerializeField]PopUpMessage popUp;


    Vector3 startPostion;
    Vector3 startScale;
    Quaternion startRotation;

    [SerializeField]
    private GameObject _go;
    [SerializeField]
    private GameObject _gc;
    [SerializeField]
    private GameObject debugButtons;

    public bool _enabled, _enabled1, _enabled2 = true;
    public DirectoryInfo dir;
    DirectoryInfo[] info;

    void Awake()
    {
        dir = new DirectoryInfo(SaveSystem.Renders + "/" + SaveSystem.UserID);
        info = dir.GetDirectories();

        objectManager = FindObjectOfType<ObjectManager>();
        gardens = FindObjectOfType<Dropdown>();
        flyCamera = FindObjectOfType<FlyCamera>();

        _enabled2 = false;

        CamSpeed.maxValue = 100f;
        CamSens.maxValue = 5f;
        gardens.onValueChanged.AddListener((UnityEngine.Events.UnityAction<int>)delegate { buttonMethods(); });
    }

    void buttonMethods()
    {
        objectManager.CreateModel();
        ReadCSVFile(dir + "/", objectManager.currentMesh.name);
        objectManager.GardenAndPlants();
        _enabled2 = false;
        defaultTransform();
    }
     

    void Update()
    {
        SliderValues();
        CamSens.onValueChanged.AddListener(delegate { SliderUse(); });
        CamSpeed.onValueChanged.AddListener(delegate { SliderUse(); });
        ToggleUI();
    }

    public void defaultTransform()
    {
        GameObject objectChild = objectManager.child.gameObject;
        startPostion = objectChild.transform.position;
        startScale = objectChild.transform.localScale;
        startRotation = objectChild.transform.rotation;
    }

    public void PopulateDropdown(GameObject[] mesh)
    {
         List<string> options = new List<string>();
         foreach(var option in mesh)
         {
             options.Add(option.name);
         }
         gardens.ClearOptions();
         gardens.AddOptions(options);
         options.Clear();
    }

    void SliderValues()
    {
        CamSpeedText.text = flyCamera._movementSpeed.ToString("#.00");
        CamSensText.text = flyCamera._mouseSense.ToString("#.00");
        CamSpeed.value = flyCamera._movementSpeed;
        CamSens.value = flyCamera._mouseSense;
    }

    void SliderUse()
    {
        flyCamera._movementSpeed = CamSpeed.value;
        flyCamera._mouseSense = CamSens.value;
        flyCamera._boostedSpeed = CamSpeed.value * 2;
    }

    public void ReadCSVFile(string path, string folderName)
    {    
        StreamReader strReader = new StreamReader(path + folderName + "/plant_data.csv");
        bool endOfFile = false;
        string name = folderName;
        int skip = 0;
        while (!endOfFile)
        {
            string data_string = strReader.ReadLine();
            
            if (data_string == null)
            {
                endOfFile = true;
                break;
            }
            var data_values = data_string.Split(',');
            if (skip == 0)
            {
                skip += 1;
            }
            else if (skip > 0)
            {
                var so = ScriptableObject.CreateInstance<PlantID>();

                #region Date from csv
                int index = data_values[12].IndexOf("T");
                if (index >= 0)
                    data_values[12] = data_values[12].Substring(0, index);
                data_values[12] = data_values[12].Substring(1);
                var date = data_values[12].Split('-');
                int Year = int.Parse(date[0]);
                int Month = int.Parse(date[1]);
                int Day = int.Parse(date[2]);
                #endregion

                #region Date from folder name
                int index1 = folderName.IndexOf(" ");
                if (index1 >= 0)
                    folderName = folderName.Substring(0, index1);
                var date2 = folderName.Split('-');
                int Year1 = int.Parse(date2[0]);
                int Month1 = int.Parse(date2[1]);
                int Day1 = int.Parse(date2[2]);
                #endregion

                #region Days from Date(CSV) to Date(FN)
                System.DateTime datevalue1 = new System.DateTime(Year, Month, Day);
                System.DateTime datevalue2 = new System.DateTime(Year1, Month1, Day1);
                int Days = (int)(datevalue2 - datevalue1).TotalDays;
                #endregion

                so.plantID = (data_values[0].ToString());
                so.gardenID = name;
                so.plantName = (data_values[4].ToString());
                so.plantAge = int.Parse(Days.ToString());
                so.plantDesc = (data_values[11].ToString());
                so.xCoord = float.Parse(data_values[7]);
                so.yCoord = float.Parse(data_values[8]);

                plantList.Add(so);
            }
        }
        objectManager.arrayOfPlantID = plantList.ToArray();
        plantList.Clear();
    }

    public void FoldersAndModels()
    {
        List<GardenID> gardenList = new List<GardenID>();
        List<GameObject> Mesh = new List<GameObject>();
        foreach (DirectoryInfo f in info)
        { 
            string par = Path.Combine(dir + "/" + f.Name + "/" + "texturedMesh.obj");

            GameObject model = new OBJLoader().Load(par);
            model.gameObject.name = f.Name;
            model.SetActive(false);

            var so = ScriptableObject.CreateInstance<GardenID>();
            so.gardenID = f.Name;
            so.gardenMesh = model;

            gardenList.Add(so);
            Mesh.Add(so.gardenMesh);
            //ReadCSVFile(dir + "/", f.Name);
        }
        objectManager.arrayOfGameObjects = Mesh.ToArray();
        objectManager.arrayOfGarden = gardenList.ToArray();
        
        Mesh.Clear();
    }

    void ToggleUI()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            _enabled = !_enabled;
            _go.SetActive(_enabled);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _enabled1 = !_enabled1;
            _gc.SetActive(_enabled1);
        }

        if (_enabled1)
            flyCamera.inMenu = true;
        else
            flyCamera.inMenu = false;
    }

    public void CloseTab()
    {
        _enabled1 = false;
        _gc.SetActive(_enabled1);
        flyCamera.inMenu = false;
    }

    public void DebugButton()
    {
        _enabled2 = !_enabled2;
        GameObject objectChild = objectManager.child.gameObject;
        objectChild.GetComponent<Collider>().enabled = _enabled2;
        foreach (var plant in objectManager.arrayOfPlantObjects)
        {
            plant.GetComponent<Interactable>().debug.SetActive(_enabled2);
            debugButtons.SetActive(_enabled2);
        }
    }

    public void Save()
    {
        GameObject objectChild = objectManager.child.gameObject;
        Vector3 objectPosition = objectChild.transform.position;
        Quaternion objectRotation = objectChild.transform.rotation;
        Vector3 objectScale = objectChild.transform.localScale;

        ObjectData objectData = new ObjectData
        {
            position = objectPosition,
            rotation = objectRotation,
            scale = objectScale
        };
        string json = JsonUtility.ToJson(objectData);
        SaveSystem.Save(json , objectManager.currentMesh.name);
        PopUpMessage pop = Instantiate(popUp);
        pop.message.text = "Save Successful";
    }

    public void Load()
    {
        string saveString = SaveSystem.Load(objectManager.currentMesh.name);
        if(saveString != null)
        {
            ObjectData objectData = JsonUtility.FromJson<ObjectData>(saveString);

            GameObject objectChild = objectManager.child.gameObject;
            objectChild.transform.position = objectData.position;
            objectChild.transform.rotation = objectData.rotation;
            objectChild.transform.localScale = objectData.scale;
        }
    }

    public void Delete()
    {
        string message = SaveSystem.Delete(objectManager.currentMesh.name);
        PopUpMessage pop = Instantiate(popUp);
        pop.message.text = message;
    }

    public class ObjectData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    public void resetValues(int transform)
    {
        string saveString = SaveSystem.Load(objectManager.currentMesh.name);
        GameObject objectChild = objectManager.child.gameObject;
        if (saveString != null)
        {
            ObjectData objectData = JsonUtility.FromJson<ObjectData>(saveString);

            switch(transform)
            {
                case 1:
                    objectChild.transform.position = objectData.position;
                    break;
                case 2:
                    objectChild.transform.rotation = objectData.rotation;
                    break;
                case 3:
                    objectChild.transform.localScale = objectData.scale;  
                    break;
                default:
                    break;
            }
            objectChild.transform.position = startPostion;
        }
        else
        {
            switch (transform)
            {
                case 1:
                    objectChild.transform.position = startPostion;
                    break;
                case 2:
                    objectChild.transform.rotation = startRotation;  
                    break;
                case 3:
                    objectChild.transform.localScale = startScale;            
                    break;
                default:
                    break;
            }
            objectChild.transform.position = startPostion;
        }
    }

    public void GardenSizeCSV(string path, string folderName)
    {
        StreamReader strReader = new StreamReader(path + folderName + "/farm_size.csv");
        bool endOfFile = false;
        int skip = 0;
        while (!endOfFile)
        {
            string data_string = strReader.ReadLine();

            if (data_string == null)
            {
                endOfFile = true;
                break;
            }
            var data_values = data_string.Split(',');
            if (skip == 0)
            {
                skip += 1;
            }
            else if (skip > 0)
            {
                objectManager.X = float.Parse(data_values[0]);
                objectManager.Z = float.Parse(data_values[1]);
            }
        }
    }
}
