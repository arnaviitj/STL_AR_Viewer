using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;

public class STLLoadManager : MonoBehaviour
{
    public string[] fileNames; // put all STL file names here

    void Start()
    {
        StartCoroutine(LoadAllSTL());
    }

    IEnumerator LoadAllSTL()
    {
        STLLoader loader = new STLLoader();

        foreach (string file in fileNames)
        {
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, file);

            Debug.Log("Loading: " + path);

            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed: " + www.error);
                continue;
            }

            byte[] data = www.downloadHandler.data;

            // 🔥 IMPORTANT: write to temp file (because loader needs path)
            string tempPath = System.IO.Path.Combine(Application.persistentDataPath, file);
            System.IO.File.WriteAllBytes(tempPath, data);

            GameObject obj = loader.Load(tempPath);

            if (obj != null)
            {
                STLSpawner.Instance.AddModel(obj);
                Debug.Log("Model added!");
            }
        }

        Debug.Log("ALL STL LOADED");
    }
}