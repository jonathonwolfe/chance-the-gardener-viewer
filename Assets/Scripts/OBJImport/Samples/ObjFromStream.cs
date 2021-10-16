using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ObjFromStream : MonoBehaviour {
    public Text test;
	void Start () {
        //make www
       // var www = new WWW(Application.streamingAssetsPath + "/2021-08-18T063551.302Z/texturedMesh.obj");
        //Debug.Log(Application.streamingAssetsPath + "/2021-08-18T063551.302Z/texturedMesh.obj");
        //while (!www.isDone)
            //System.Threading.Thread.Sleep(1);
        
        //create stream and load
        //var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.text));
        //var loadedObj = new OBJLoader().Load(textStream);
        test.text = Application.dataPath + " " + Application.streamingAssetsPath;

    }
}
