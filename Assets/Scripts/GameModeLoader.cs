//AG_3 win= https://pressions.art:5000/assets/skyverseAssets/6f911eb8319560de1c3f0ad00
//AG_3 webGl = https://pressions.art:5000/assets/skyverseAssets/6f911eb8319560de1c3f0ad01
//spatial url  - https://cdn.spatial.io/assets/v1/unity-assets/packBundle-20-production/647e543cbfa8b8e3445f7834/Unity/Space/647e6afdb4856aa6d8fabd2f/20/bundles/WEB_UNITY/r/3c9358d47810472c1bb9c8fd33e947811f43b06e80d1cc0a337c617427eb5e01/1689687512
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameModeLoader : MonoBehaviour
{
    public static event Action BundleLoaded;
    private Scene targetScene;
    public string windowsUrl;
    public string webGLUrl;
    public string _prefabName;
    public string _WindowsBundleName,_WebGlBundleName;

    public List<GameObject> _sceneObjs;
    

    private void Start() {
        targetScene = SceneManager.GetSceneByName(GameManager.Instance.sceneToLoad);
        SceneManager.SetActiveScene(targetScene);
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            
            LoadFromMemory(_WindowsBundleName);
            
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            
            StartCoroutine(DownloadFromMemory());
        }  
    }
    void LoadFromMemory(string BundleName)
    {
        // Debug.Log("My application AssetStreaming path "+ Application.streamingAssetsPath);
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, BundleName));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        var prefab = myLoadedAssetBundle.LoadAsset<GameObject>(_prefabName);
        // Instantiate(prefab);
        GameObject loadedAsset = Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(loadedAsset, targetScene);
        LoadSceneObjects();
        BundleLoaded?.Invoke();
        myLoadedAssetBundle.Unload(false);
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
                var prefab = myLoadedAssetBundle.LoadAsset<GameObject>(_prefabName);                
                GameObject loadedAsset = Instantiate(prefab);
                SceneManager.MoveGameObjectToScene(loadedAsset, targetScene);
                LoadSceneObjects();
                BundleLoaded?.Invoke();
                Debug.Log("Asset bundle loaded from URL "+ url);
                // Unload the AssetBundle
                myLoadedAssetBundle.Unload(false);
            }
        }
    }
    private void LoadSceneObjects(){
        foreach(GameObject obj in _sceneObjs){
            obj.SetActive(true);
        }
    }
}
