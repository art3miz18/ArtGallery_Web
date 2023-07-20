// using UnityEngine;
// using System.Collections;
// using DeepAI.Client; // make sure the DeepAI.Client library is imported into your Unity project

// public class DeepAIManager : MonoBehaviour
// {
//     void Start()
//     {
//         StartCoroutine(CallDeepAIWithURL());
//     }

//     IEnumerator CallDeepAIWithURL()
//     {
//         DeepAI_API api = new DeepAI_API(apiKey: "69eb61c5-aaa9-43c0-ac85-aabf68e0081c");
        
//         StandardApiResponse resp = null;

//         yield return api.callStandardApi("torch-srgan", new {
//             image = "YOUR_IMAGE_URL",
//         }, (response) => {
//             resp = response;
//         });

//         if (resp != null)
//         {
//             Debug.Log(api.objectAsJsonString(resp));
//         }
//         else
//         {
//             Debug.LogError("Failed to get response from DeepAI");
//         }
//     }
// }
