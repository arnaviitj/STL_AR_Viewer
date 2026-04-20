using System.Collections.Generic;
using UnityEngine;

public class STLSpawner : MonoBehaviour
{
    public static STLSpawner Instance;

    private List<GameObject> models = new List<GameObject>();
    private int currentIndex = 0;

    private bool isPlaced = false;
    private GameObject rootObject;

    private bool isPlaying = false;
    private float timer = 0f;
    private float playSpeed = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isPlaying) return;
        if (models.Count == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / Mathf.Max(playSpeed, 0.1f))
        {
            timer = 0f;

            currentIndex = (currentIndex + 1) % models.Count;
            ShowModel(currentIndex);
        }
    }

    // ================= LOAD =================

    public void AddModel(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.color = Color.gray;

        foreach (var r in obj.GetComponentsInChildren<MeshRenderer>())
        {
            r.material = mat;
        }

        models.Add(obj);
    }

    public int GetLoaded() => models.Count;
    public int GetTotal() => models.Count;
    public int GetCurrent() => currentIndex;
    public bool IsAllLoaded() => models.Count > 0;

    // ================= STATE =================

    public bool IsPlaced() => isPlaced;
    public bool IsPlaying() => isPlaying;

    public void SetPlaying(bool value)
    {
        isPlaying = value;
    }

    public GameObject GetRoot() => rootObject;

    // ================= PLACEMENT =================

    public void PlaceRoot(Vector3 position, Quaternion rotation)
    {
        if (models.Count == 0) return;

        if (rootObject == null)
        {
            rootObject = new GameObject("STLRoot");

            foreach (GameObject model in models)
                model.transform.SetParent(rootObject.transform);
        }

        rootObject.transform.position = position;
        rootObject.transform.rotation = rotation;
        rootObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        isPlaced = true;

        ShowModel(currentIndex);
    }

    public void MoveRoot(Vector3 position, Quaternion rotation)
    {
        if (rootObject != null)
        {
            rootObject.transform.position = position;
            rootObject.transform.rotation = rotation;
        }
    }

    // ================= MODEL CONTROL =================

    void ShowModel(int index)
    {
        for (int i = 0; i < models.Count; i++)
        {
            models[i].SetActive(i == index);
        }
    }

    public void StepForward()
    {
        if (isPlaying) return;

        SetModel(currentIndex + 1);
    }

    public void StepBack()
    {
        if (isPlaying) return;

        SetModel(currentIndex - 1);
    }

    public void SetModel(int index)
    {
        if (models.Count == 0) return;

        currentIndex = (index % models.Count + models.Count) % models.Count;
        ShowModel(currentIndex);
    }

    // ================= PLAY =================

    public void TogglePlay()
    {
        isPlaying = !isPlaying;
    }

    public void SetPlaySpeed(float speed)
    {
        playSpeed = speed;
    }

    // ================= RESET =================

    public void ResetAll()
    {
        isPlaying = false;
        currentIndex = 0;
        timer = 0f;

        if (rootObject != null)
        {
            // 🛠️ FIX: Unparent and hide models so they survive the destruction of rootObject
            foreach (GameObject model in models)
            {
                if (model != null)
                {
                    model.transform.SetParent(null);
                    model.SetActive(false);
                }
            }

            Destroy(rootObject);
            rootObject = null;
        }

        isPlaced = false;

        Debug.Log("RESET DONE");
    }
}