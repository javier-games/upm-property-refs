using System.Collections.Generic;
using UnityEngine;

namespace JG.UPM.PropertyRefs.Samples.RefsComponentPrefabs
{
    /// <summary>
    /// Assign this script to a game object and assign a GameObject or component to it.
    /// </summary>
    public class PrefabProperties : MonoBehaviour
    {
        /// <summary>
        /// List of references.
        /// </summary>
        [SerializeField]
        private List<PropertyRef> properties;
    }
}