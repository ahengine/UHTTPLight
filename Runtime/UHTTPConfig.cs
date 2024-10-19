using System.Collections.Generic;
using UnityEngine;

namespace UHTTPLight
{
    [CreateAssetMenu(fileName = nameof(UHTTPConfig), menuName = nameof(UHTTPConfig), order = 1)]
    public class UHTTPConfig : ScriptableObject
    {      
        [Tooltip("UHTTPConfig (Don't change asset name), This asset must be in resources folder.")]
        [field:SerializeField] public string baseURL { get; set; }
        [field:SerializeField] public List<KeyValuePair<string,string>> defaultHeaders { get; set; } = new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("Content-Type", "application/json"),
            new KeyValuePair<string, string>("Accept", "application/json"),
        };
        [field:SerializeField] public bool useBearerPrefixAuthHeader { get; set; } = true;
    }
}