using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using static ArtworkLoader;

public class ArtworkTrigger : MonoBehaviour
{
    public string ItemID;
    //GameModeBase gameMode;
    public Transform mesh;
    public Material material;
    public string Name, Price, Description,Src;
    
    public List<ArtworkLoader.ArtDetails> artworkDetails;
    private CanvasGroup _fadeUi;
    
    private void Awake()
    {
        ArtworkLoader.artworkDetails += CartDetails;
        _fadeUi = transform.GetChild(0).GetChild(0).GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        material = mesh.GetComponent<Renderer>().materials[0];
        /*gameMode = GameModeBase.Instance;*/
    }
    public void CartDetails(List<ArtworkLoader.ArtDetails> Data)
    {
        artworkDetails = Data;
    }

    public void SetDetails(string name,string price,string desc,string url)
    {
        Name = name;
        Price = price;
        Description = desc;
        Src = url;
    }

    public void FadeUi(float fadeValue){
        _fadeUi.DOFade(fadeValue , 0.2f);
    }
    

}
