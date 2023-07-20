using UnityEngine;
using UnityEditor;

public class ReplaceObjects : MonoBehaviour
{
    public GameObject prefabReplacement; // Assign your prefab in the inspector

    public void ReplaceChildObjects()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform oldTransform = transform.GetChild(i);

            // Store transform data
            Vector3 oldPosition = oldTransform.position;
            Quaternion oldRotation = oldTransform.rotation;
            Vector3 oldScale = oldTransform.localScale;

            // Destroy old object
            DestroyImmediate(oldTransform.gameObject);

            // Instantiate new object
            GameObject newObject = PrefabUtility.InstantiatePrefab(prefabReplacement) as GameObject;
            newObject.transform.position = oldPosition;
            newObject.transform.rotation = oldRotation;
            newObject.transform.localScale = oldScale;
            newObject.transform.SetParent(transform, false);
        }
    }
}
