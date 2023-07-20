using DG.Tweening;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using StarterAssets;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
public class ArtworkLoader : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenModal(string imageUrl);
    public static ArtworkLoader instance;
    public string[] paintingGenre;
    public List<KeyValuePair<string, string>> Units;   
    public ArtworkTrigger[] artworktrigger;
    Texture2D myTexture;
    public static event Action<List<ArtDetails>> artworkDetails;
    int i = 0;
    public float MaxFrameSize;

    //cycle artwork
    [Header("Cycling Artwork")]
    public int RotatingTime = 20;

    private int ArtworkIndex = 0;
    private int ArtworkRotationIndex = 0;
    private bool ArtworkRotation = true;

    public struct ArtDetails
    {
        public string ArtworkName;
        public string ArtworkPrice;
        public string ArtworkDescription;

    }
    [Header("AI Generated Images")]
    // [Range(0.0f, 44.0f)] 
    public int TotalImages = 0;
    private String LexicaUri = "https://lexica.art/api/v1/search?q=";
    public bool FetchImages,IsOpen;   
    private int downloadIndex = 0,_downloadCount = 0;
    public RawImage GallerySelector,_painting;
    public TMP_Text PaintingName;
    public CanvasGroup GallImage;
    
    private ThirdPersonController player;
    public void SetReferences(){
        // GameObject pl = GameManager.Instance._mPlayer;
        GallImage = GameManager.Instance.GallImage;
        PaintingName = GameManager.Instance.PaintingName;
        _painting = GameManager.Instance._painting;
        GallerySelector = GameManager.Instance.GallerySelector;  
        
        artworktrigger = new ArtworkTrigger[transform.childCount];
        for (int j = 0; j < transform.childCount; j++)
        {
            artworktrigger[j] = transform.GetChild(j).GetComponent<ArtworkTrigger>();
            TotalImages++;
        }
        player = GameManager.Instance._mPlayer.GetComponent<ThirdPersonController>();
        LoadGallery();
        // StartCoroutine(GetAIGeneratedImages(CurrentCategory));
    }
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        StartCoroutine(initialize());        
    } 
    private IEnumerator initialize(){        
        yield return new WaitUntil(() => playerFound());
        SetReferences();
    }
    private bool playerFound(){
        return GameManager.Instance._mPlayer!= null;         
    }
    public void LoadGallery(){
        
        StartCoroutine(GetAIGeneratedImages(GameManager.Instance.paintingGenre[0]));
    }  
    private bool IsPointerOverUI(Vector2 mousePosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
    // for next Gallery 
    public void NextCategory(string category){
        if(FetchImages == false){ 
            StartCoroutine(GetAIGeneratedImages(category));
        }
    }    
    //For previous Gallery
    public void SearchParam(string searchFor){
        StopAllCoroutines();      
        i = 0;
        StartCoroutine(GetAIGeneratedImages(searchFor));
    }
    // public void GetLexicaData(){
    //     if(!string.IsNullOrEmpty(SearchParameter)){
    //         if(FetchImages == true){
    //             StopAllCoroutines();
    //         }
    //         ArtworkIndex = 0;
    //         i=0;
    //         StartCoroutine(GetAIGeneratedImages(SearchParameter));
    //     }
    // }
    public IEnumerator GetAIGeneratedImages(string Keyword){
        bool changeCategory = true;
        string Url = LexicaUri + Keyword;        
        UnityWebRequest request = UnityWebRequest.Get(Url);
        request.certificateHandler = new WebRequestCert();
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Connection Error");
        }
        else
        {
            Units = new List<KeyValuePair<string, string>>();
            string EncryptedString = request.downloadHandler.text;
            JSONNode jSONNode = JSON.Parse(EncryptedString);
            // Debug.Log(jSONNode);            
            // int totalCountOfListToshow = jSONNode["images"].Count;
            List<ArtDetails> artDetails = new List<ArtDetails>();
            for (int i = 0; i < TotalImages; i++)
            {                
                if(!jSONNode["images"][i]["nsfw"]){

                    Units.Add(new KeyValuePair<String, String>(jSONNode["images"][i]["src"], jSONNode["images"][i]["id"]));
                    string ArtworkName = jSONNode["images"][i]["prompt"];
                    string ArtworkPrice = "Free To Use";
                    string ArtworkDescription = "A Demo of Generative AI Gallery";
                    string _url = jSONNode["images"][i]["src"];
                    artworktrigger[i].SetDetails(ArtworkName, ArtworkPrice, ArtworkDescription,_url);

                    if(changeCategory == true){
                        string url = jSONNode["images"][i]["src"];
                        Debug.Log("First Image URL "+ url); 
                        StartCoroutine(DownloadImage(url));
                        changeCategory = false;
                    }    
                }
            }
            if (Units == null)
            {
                print("units is null");
            }
            // Debug.Log("Total Units being Updated" + Units.Count + " Triggers : " + artworktrigger.Length);
            CycleArtwork(Units);
        }
        yield return null;
    }
    
    public void CycleArtwork(List<KeyValuePair<string, string>> ArtworkDetails)
    {
        List<KeyValuePair<string, string>> units = new List<KeyValuePair<string, string>>();
        if (artworktrigger.Length == ArtworkDetails.Count)
        {
            ArtworkRotation = false;
            ArtworkIndex = 0;
            for (int i = 0; i < artworktrigger.Length; i++)
            {                
                    units.Add(ArtworkDetails[ArtworkIndex]);
                    ArtworkIndex++;                
            }
            StartCoroutine(SetImages(units, ArtworkDetails));
        }
        else if (ArtworkIndex + artworktrigger.Length < ArtworkDetails.Count)
        {
            // print("More artworks and less spaces, rotating artworks");
            ArtworkRotationIndex++;
            // print("Rotating Index:" + ArtworkRotationIndex);
            for (int i = 0; i < artworktrigger.Length; i++)
            {
                units.Add(ArtworkDetails[ArtworkIndex]);
                // print("ArtworkIndex: " + ArtworkIndex + " ; ArtwrokID: " + ArtworkDetails[ArtworkIndex].Value);
                ArtworkIndex++;
            }
            StartCoroutine(SetImages(units, ArtworkDetails));
        }
        else if (ArtworkDetails.Count < artworktrigger.Length){
            // print("Less Artworks and more spaces");
            ArtworkIndex = 0;                   
            for (int i = 0; i < artworktrigger.Length; i++)
            {
                if(i >= ArtworkDetails.Count){
                    ArtworkIndex = 0;                   
                    // Debug.Log("I is greater , reset index to 0");
                }
                // Debug.Log("artwork index"+ ArtworkIndex);
                units.Add(ArtworkDetails[ArtworkIndex]);
                // print("ArtworkIndex: " + ArtworkIndex + " ; ArtwrokID: " + ArtworkDetails[ArtworkIndex].Value);
                ArtworkIndex++;
            }    
        StartCoroutine(SetImages(units, ArtworkDetails));    
        }
        else
        {
            // print("Less Artworks and more spaces");

            if (ArtworkRotationIndex == 0)
            {
                ArtworkRotation = false;
                // print("Stopping rotation cuz spaces>artwork");
            }
            else
            {
                ArtworkRotationIndex++;
                // print("Rotating Index:" + ArtworkRotationIndex);
            }

            for (int i = 0; i < ArtworkDetails.Count; i++)
            {
                if (ArtworkIndex == ArtworkDetails.Count)
                {
                    ArtworkIndex = 0;
                }
                units.Add(ArtworkDetails[ArtworkIndex]);
                ArtworkIndex++;
            }

            StartCoroutine(SetImages(units, ArtworkDetails));
        }
    }

    public IEnumerator SetImages(List<KeyValuePair<String, String>> Units, List<KeyValuePair<String, String>> ArtworkDetails)
    {
        FetchImages = true;
        for (int i = 0; i < artworktrigger.Length; i++)
        {
            // print("on position"+ i);
            artworktrigger[i].material.SetTexture("_Main", null);
            artworktrigger[i].transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 1);
        }
        while (i < artworktrigger.Length )
        {
            artworktrigger[i].transform.GetChild(0).transform.DOScale(new Vector3(2.5f, 2.5f, 2), 1);
            yield return StartCoroutine(GetArtworkImage(Units[i].Key));

            float ratio = (float)myTexture.height / myTexture.width;

            if (ratio > 1)
            {
                ratio = (float)myTexture.width / myTexture.height;
                artworktrigger[i - 1].mesh.DOScale(new Vector3(MaxFrameSize * ratio, MaxFrameSize, 1), 1);
            }
            else
            {
                artworktrigger[i - 1].mesh.DOScale(new Vector3(MaxFrameSize, MaxFrameSize * ratio, 1), 1);
            }
            artworktrigger[i - 1].material.SetTexture("_Main", myTexture);
            artworktrigger[i - 1].ItemID = Units[i - 1].Value;
        }

        i = 0;
        yield return new WaitForSeconds(RotatingTime);
        if (ArtworkRotation)
        {
            CycleArtwork(ArtworkDetails);
        }
        FetchImages = false;
    }
    public IEnumerator GetArtworkImage(string url)
    {
        
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            
            myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        i++;
    }

    private IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // Get the downloaded image
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            GallerySelector.texture = texture;
        }
    }
    
    public void PaintingsDetails(Transform hit) {

        if(IsOpen == false && !IsPointerOverUI(Mouse.current.position.ReadValue())){         
            
            // Debug.Log("Clicked on " + hit.transform.name);
            GameObject g = hit.transform.gameObject;
            ArtworkTrigger _artworkTrigger = g.GetComponent<ArtworkTrigger>();
            if(_artworkTrigger != null){
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    GallImage.DOFade(1,0.2f);
                    IsOpen = true; // when the image is open should avoid raycasting through the open image
                    player.enabled = false; // disable player controller to prevent mis touches
                    GallImage.interactable = true;
                    GallImage.blocksRaycasts = true;

                    Texture textureImg = _artworkTrigger.material.GetTexture("_Main");
                    float ratio = (float)textureImg.height / textureImg.width;
                    if (ratio > 1)
                    {
                        ratio = (float)textureImg.width / textureImg.height;
                        _painting.transform.DOScale(new Vector3(1 * ratio, 1, 1), 1);
                    }
                    else
                    {
                        _painting.transform.DOScale(new Vector3(1, 1 * ratio, 1), 1);
                    }
                    PaintingName.text = _artworkTrigger.Name;
                    _painting.texture = textureImg;
                }
                else if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    string url = _artworkTrigger.Src;
                        if(url!= null){
                            OpenModal(url);
                        }                    
                } 
                
            }            
        }
    }
}
