using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string sceneToLoad; // Assign the name of the scene you want to load in the Inspector
    public GameObject playerPrefab; // Assign your player prefab in the Inspector
    public Vector3 spawnLocation; // Assign where you want to spawn the player in the Inspector
    public CinemachineVirtualCamera vcam;
    public GameObject _mPlayer ,paintingDetails, _SceneLoader;
    GameObject childTarget;
    public RawImage GallerySelector,_painting;
    public TMP_Text categoryName,NextCategoryName,PaintingName;    
    public CanvasGroup GallImage;
    public TMP_InputField search;
    private ThirdPersonController _playerCC;
    private int downloadIndex = 0,_downloadCount = 0;
    public string[] paintingGenre;
    private string CurrentCategory;
    void Start()
    {
        sceneLoader  = _SceneLoader.GetComponent<Loader>();
        SceneLoader();
        // Start loading the scene asynchronously and continue running this frame
        StartCoroutine(LoadSceneAsync());
        if(Instance == null){
            Instance = this;
        }
        CurrentCategory = paintingGenre[downloadIndex];
        categoryName.text = CurrentCategory;
    }

    IEnumerator LoadSceneAsync()
    {       
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad,LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    public void SpawnCharacter()
    {
        _mPlayer = Instantiate(playerPrefab, spawnLocation, Quaternion.identity,this.transform);
        _playerCC = _mPlayer.GetComponent<ThirdPersonController>();
        foreach(Transform child in _mPlayer.transform){
            if(child.CompareTag("CinemachineTarget")){
                childTarget = child.gameObject;
                break;
            }
        }
        vcam.Follow = childTarget.transform;
        // ArtworkLoader.instance.SetReferences();
    }
    public void SearchFade(float fadeValue){
        search.transform.DOScaleY(fadeValue,0.2f);
        _playerCC.enabled = false;
    }
    public void SearchFor(){
        if (search.text.EndsWith("\n"))
        {
            string searchT = search.text;
            SearchFade(0f);
            search.DeactivateInputField();          
            CurrentCategory = searchT;
            categoryName.text = CurrentCategory;
            _playerCC.enabled = true;   
            search.transform.GetComponent<TMP_InputField>().enabled = false;
            ArtworkLoader.instance.SearchParam(CurrentCategory);         
        }
    }
    public void CloseCanvas(){
        GallImage.DOFade(0,0.2f);
        GallImage.interactable = false;
        GallImage.blocksRaycasts = false;
        _playerCC.enabled = true; // enable Player Movements
        ArtworkLoader.instance.IsOpen = false;
    }

    public void NextCategory(){                  
        Debug.Log("Next Gallery");
        if(downloadIndex+1 > _downloadCount ){
            downloadIndex = 0 ;
        }            
        downloadIndex++;
        CurrentCategory = paintingGenre[downloadIndex];
        categoryName.text = CurrentCategory;
        ArtworkLoader.instance.NextCategory(CurrentCategory);
    }        
    private void OnEnable(){
        
        GameModeLoader.BundleLoaded += SpawnCharacter;
    }
    private void OnDisable(){
        
        GameModeLoader.BundleLoaded -= SpawnCharacter;
    }
    private Loader sceneLoader;

    private void SceneLoader(){
        sceneLoader.enabled = true;
    }
}
