using UnityEngine;
using System.Collections;
using StarterAssets;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CursorManager : MonoBehaviour
{
     public enum CursorState
    {
        Idle,
        OverPainting,
        OverGround,
        OverUI
    }
    public float RayDistance = 10f;
    private CursorState currentCursorState = CursorState.Idle;
    public Texture2D[] cursorTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;    
    public Transform FocusObject;    
    public Camera mainCam;    
    public Vector2 _cursorPosition;
    public ArtworkLoader GallerySystem;
    public GameObject locomotionObj;
    private PaintingDetails pd;
    public Vector3 initialScale;
    private Material locoMat;
    private CharacterController characterController;
    private float screenBottomTresh = 400f;

    private void Start() {
        GallerySystem = FindObjectOfType<ArtworkLoader>(); 
        OnLoadDone(GameManager.Instance.LocomotionObjPlane);
        mainCam = Camera.main;         
        characterController = GetComponent<CharacterController>();
        pd = GameManager.Instance.paintingDetails.GetComponent<PaintingDetails>();
    }    
    private void UpdateCursorState() {
        if (IsMouseOverUI()) {
            currentCursorState = CursorState.OverUI;
        } else if (IsMouseOverPainting()) {
            currentCursorState = CursorState.OverPainting;
        } else if (IsMouseOverGround()) {
            currentCursorState = CursorState.OverGround;
        } else {
            currentCursorState = CursorState.Idle;
        }
    }
    private bool IsMouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }
    private bool IsMouseOverPainting() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int paintingLayerMask = 1 << 6;
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, RayDistance, paintingLayerMask)){
            if(hit.transform!= null ){
                if(FocusObject!= null && FocusObject != hit.transform){
                    FadeOutCanvas(FocusObject);
                    FocusObject = hit.transform;
                }
                else if (FocusObject == null){
                    FocusObject = hit.transform;
                }
                FadeInCanvas(FocusObject);
                return true;
            }
        }
        else if(FocusObject != null){
            FadeOutCanvas(FocusObject);
            FocusObject = null;
        }
        return false;       
        
    }

    private bool IsMouseOverGround() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int groundLayerMask = 1 << 7;
        return Physics.Raycast(ray, RayDistance * 2, groundLayerMask);
    }
    public void RaycastTrigger(){
        Ray ray = mainCam.ScreenPointToRay(_cursorPosition);
        RaycastHit hit;
        int PaintingMask = 1 << 6;
        int groundMask = 1 << 7;
        if (Physics.Raycast(ray,out hit,RayDistance * 2, groundMask)){	
            if(hit.transform != null && currentCursorState != CursorState.OverUI){
                Vector3 point = hit.point;
                Debug.DrawLine(mainCam.transform.position,point,Color.blue,1f);
                StartCoroutine(LocomotionPlane(point));
            }
        }
        else if (Physics.Raycast(ray,out hit,RayDistance, PaintingMask)){	
            if(hit.transform != null){
                FocusObject = hit.transform;
                GallerySystem.PaintingsDetails(FocusObject);
                StartCoroutine(PaintingDetails()); 
            }           
        }
    }
    private IEnumerator PaintingDetails(){
        while(GallerySystem.IsOpen){
            if(_cursorPosition.y <= screenBottomTresh){
                pd.Show();
            }
            else{
                
                pd.hide();
            }
            yield return null;
        }
    }
    //Called on Button Click
    public void SimilarImageFill(){
        string src = FocusObject.GetComponent<ArtworkTrigger>().Src;
        Debug.Log(src);
        StartCoroutine(GallerySystem.GetAIGeneratedImages(src));
        pd.hide();
        // GallerySystem.CloseCanvas(); // replace at GameManager
    }
    private IEnumerator LocomotionPlane(Vector3 position){
        Vector3 offset = new Vector3(0,0.3f,0);
        locomotionObj.transform.position = position+offset;
        locomotionObj.transform.localScale = initialScale;
        locomotionObj.transform.DOScale(Vector3.zero , 0.2f); 
        yield return new WaitForSeconds(.25f);       
        // characterController.Move(position+offset);
        characterController.enabled = false;
        transform.position = position +offset;
        characterController.enabled = true;
                   
        yield return null;
    }
    public void SetCursorTexture(Texture2D cur){
        Cursor.SetCursor(cur,hotSpot,cursorMode);
    }
    private void HandleMouseEvents(Vector2 MousePosition){
        _cursorPosition = MousePosition;
        UpdateCursorState();        
    }
    // Method to fade in the canvas of a painting
    private void FadeInCanvas(Transform painting) {
        CanvasGroup cg = painting.GetChild(0).GetChild(0).GetComponent<CanvasGroup>();
        if (cg != null) {
            cg.DOFade(1, 0.2f);
        }
    }

    // Method to fade out the canvas of a painting
    private void FadeOutCanvas(Transform painting) {
        CanvasGroup cg = painting.GetChild(0).GetChild(0).GetComponent<CanvasGroup>();
        if (cg != null) {
            cg.DOFade(0, 0.2f);
        }
    }
    //load locomotion plane directly here avoid addressables currently 
    private void OnLoadDone(GameObject prefab)
    {            // Instantiate the loaded prefab
            GameObject ob = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            locomotionObj = ob;
            locoMat = locomotionObj.GetComponent<MeshRenderer>().material;        
    }
    void OnEnable(){
        StarterAssetsInputs._mouseState += HandleMouseEvents;
    }
    void OnDisable(){
        StarterAssetsInputs._mouseState -= HandleMouseEvents;
    }
}