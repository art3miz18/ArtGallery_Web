using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Loader : MonoBehaviour
{
    public TMP_Text loading;
    public TMP_Text loadingText;
    private string[] loadingMessages = 
    {
        "Generating awesomeness...",
        "Conjuring game magic...",
        "Summoning pixel fairies...",
        "Turning on the fun...",
        "Unleashing epic gameplay...",
        "Baking digital donuts...",
        "Pouring a cup of digital joy...",
        "Warping into the fun dimension...",
        "Sparking the game-o-tron...",
    };
    private string finalMessage = "Revving up the game engines...";
    private CanvasGroup fader;

    public float minLoadingTime = 5f;
    public float maxLoadingTime = 10f;
    private int lastPercentageInterval = -1;
    private int currentInterval;

//Image components
    public Image targetImage;
    public List<Color> colors;
    public float changeInterval = 1f;
    private Coroutine colorChangeCoroutine;
    private Loader self;
    private void Start() {
        fader = GetComponent<CanvasGroup>();
        FadeIn(1);
        currentInterval = Random.Range(20, 31); // Generate a random interval between 20% and 30%
        StartCoroutine(LoadingCounterRoutine());
    }

    private IEnumerator LoadingCounterRoutine()
    {
        float loadingTime = Random.Range(minLoadingTime, maxLoadingTime);
        float endTime = Time.time + loadingTime;
        int percentage = 0;
        while (percentage < 100)
        {
            float remainingTime = endTime - Time.time;
            percentage = Mathf.Clamp(100 - (int)((remainingTime / loadingTime) * 100), 0, 100);
            loading.text = percentage.ToString() + " %";
            targetImage.fillAmount = (float)percentage/100;
            
            // Update loading message
            if (percentage < 100)
            {
                // Change the loading message at each currentInterval
                if (percentage / currentInterval > lastPercentageInterval)
                {
                    lastPercentageInterval = percentage / currentInterval;
                    currentInterval = Random.Range(20, 31); // Generate a new random interval
                    int randomIndex = Random.Range(0, loadingMessages.Length);
                    loadingText.text = loadingMessages[randomIndex];
                }
            }
            else
            {
                loadingText.text = finalMessage;
            }
            // Wait for a random amount of time to simulate loading
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
        }

        loading.text = "100%";
        yield return new WaitForSeconds(Random.Range(0.2f, 0.7f));
        CloseLoader();
    }
    private void FadeIn(float fadeValue){
        fader.DOFade(fadeValue,0.2f);
    } 

    //Will change colors when the loader is active
    private IEnumerator ChangeColor()
    {
        while (true)
        {
            // Choose a random color from the list
            int colorIndex = Random.Range(0, colors.Count);

            // Use DOTween to animate the color change over the given interval
            targetImage.DOColor(colors[colorIndex], changeInterval);

            // Wait for the interval before changing color again
            yield return new WaitForSeconds(changeInterval);
        }
    }
    
    private void OnEnable()
    {
        // Start color change when script is enabled
        colorChangeCoroutine = StartCoroutine(ChangeColor());
    }

    private void OnDisable()
    {
        // Stop color change when script is disabled
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
            colorChangeCoroutine = null;
        }
    }
    private void CloseLoader(){
        FadeIn(0f);
        gameObject.GetComponent<Loader>().enabled = false;
    }
}
