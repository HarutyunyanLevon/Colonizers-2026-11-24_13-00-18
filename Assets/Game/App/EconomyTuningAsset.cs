using Game.Economy;
using UnityEngine;

namespace Game.App
{
    /// <summary>
    /// The one data asset holding every economy value. Edited in the Inspector or live through the
    /// development debug panel (in the Editor, live edits made in Play mode are kept in the asset).
    /// </summary>
    [CreateAssetMenu(menuName = "ColonizeEarth/Economy Tuning", fileName = "EconomyTuning")]
    public sealed class EconomyTuningAsset : ScriptableObject
    {
        public EconomyTuning values = new EconomyTuning();

        void OnValidate() => values?.Sanitize();
    }
}
