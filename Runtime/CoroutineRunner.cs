using System.Collections;
using UnityEngine;

namespace UHTTPLights
{
    [CreateAssetMenu(fileName = nameof(UHTTPLightConfig), menuName = nameof(UHTTPLightConfig), order = 1)]
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;
        private static CoroutineRunner Instance => 
            instance ??= new GameObject(nameof(CoroutineRunner)).AddComponent<CoroutineRunner>();

        public static Coroutine Run(IEnumerator func) =>
            Instance.StartCoroutine(func);

        public static void Stop(IEnumerator func) =>
            Instance.StopCoroutine(func);     
    }
}