using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using TMPro;

public class ABloader : MonoBehaviour
{
    public string webGLUrl; //http://192.168.1.9/gallery3/Utahjazz
    public TMP_InputField urlBox; 
    // public string _prefabName;
    public void AddUrl(){
        webGLUrl = urlBox.text;
        StartCoroutine(DownloadFromMemory());
    }
    IEnumerator DownloadFromMemory(){
        string url = webGLUrl;
        // Download the asset bundle
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Load asset bundle
                AssetBundle myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(www);

                if (myLoadedAssetBundle == null)
                {
                    Debug.Log("Failed to load AssetBundle!");
                    yield break;
                }

                // Load the asset from the bundle
                // var prefab = myLoadedAssetBundle.LoadAsset<GameObject>(_prefabName);                
                // GameObject loadedAsset = Instantiate(prefab);  
                // Get all prefab names
                string[] assetNames = myLoadedAssetBundle.GetAllAssetNames();

                // Instantiate all prefabs
                foreach (string assetName in assetNames)
                {
                    var prefab = myLoadedAssetBundle.LoadAsset<GameObject>(assetName);
                    if (prefab != null) 
                    {
                        GameObject loadedAsset = Instantiate(prefab);
                    }
                }              
                Debug.Log("Asset bundle loaded from URL "+ url);
                // Unload the AssetBundle
                myLoadedAssetBundle.Unload(false);
                Camera.main.transform.position =  new Vector3(0, 0, -10); 
            }
        }
    }
}
