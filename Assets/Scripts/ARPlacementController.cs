using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARPlacementController : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private float lastX;
    private float lastDistance;
    private bool isPinching = false;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        if (raycastManager == null)
        {
            Debug.LogError("❌ ARRaycastManager not found!");
        }
    }

    void Update()
    {
        if (STLSpawner.Instance == null) return;
        if (!STLSpawner.Instance.IsAllLoaded()) return;

        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        // 🔥 BLOCK UI TOUCHES
        if (IsTouchOverUI(t.position))
            return;

        // 🔍 PINCH ZOOM
        if (Input.touchCount == 2 && STLSpawner.Instance.IsPlaced())
        {
            HandlePinch();
            return;
        }

        isPinching = false;

        // 📍 TAP TO PLACE / MOVE
        if (t.phase == TouchPhase.Began)
        {
            lastX = t.position.x;

            if (raycastManager.Raycast(t.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose pose = hits[0].pose;

                if (!STLSpawner.Instance.IsPlaced())
                {
                    STLSpawner.Instance.PlaceRoot(pose.position, pose.rotation);
                }
                else
                {
                    STLSpawner.Instance.MoveRoot(pose.position, pose.rotation);
                }
            }
        }

        // 🔄 ROTATION
        if (t.phase == TouchPhase.Moved && STLSpawner.Instance.IsPlaced())
        {
            float delta = t.position.x - lastX;
            lastX = t.position.x;

            GameObject root = STLSpawner.Instance.GetRoot();
            if (root != null)
            {
                root.transform.Rotate(Vector3.up, -delta * 0.3f, Space.World);
            }
        }
    }

    void HandlePinch()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float dist = Vector2.Distance(t0.position, t1.position);

        if (!isPinching)
        {
            lastDistance = dist;
            isPinching = true;
            return;
        }

        float delta = dist - lastDistance;
        lastDistance = dist;

        GameObject root = STLSpawner.Instance.GetRoot();
        if (root != null)
        {
            float scaleFactor = 1f + delta * 0.002f;
            root.transform.localScale *= scaleFactor;
        }
    }

    bool IsTouchOverUI(Vector2 pos)
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        return results.Count > 0;
    }
}