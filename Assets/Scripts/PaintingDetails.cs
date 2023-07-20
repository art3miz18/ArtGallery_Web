using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PaintingDetails : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform descriptionRect;    
    public float animationTime = 0.3f;

    public Vector2 hiddenPosition;
    public Vector2 shownPosition;
    private bool isHiding;

    public void Show(){
        descriptionRect.DOAnchorPos(shownPosition,animationTime);
    }
    public void hide(){
        if (isHiding) return;
        StartCoroutine(HideAfterDelay());
    }
    private IEnumerator halt(){
        yield return new WaitForSeconds(1.2f);
        
    }
    private IEnumerator HideAfterDelay() 
    {
        isHiding = true;

        yield return StartCoroutine(halt());

        descriptionRect.DOAnchorPos(hiddenPosition, animationTime);

        isHiding = false;
    }
}
