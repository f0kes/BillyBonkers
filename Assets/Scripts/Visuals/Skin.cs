using UnityEngine;
using UnityEngine.Serialization;

namespace Visuals
{
    [CreateAssetMenu(fileName = "New Skin", menuName = "Skin", order = 1)]
    public class Skin : ScriptableObject
    {
    
        [FormerlySerializedAs("materials")] [SerializeField] public Material material;
        [SerializeField] public Color mainColor = Color.white;
        [SerializeField] public Sprite icon;
        
    }
}
