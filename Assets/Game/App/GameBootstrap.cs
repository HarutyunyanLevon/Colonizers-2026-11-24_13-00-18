using Game.Economy;
using UnityEngine;

namespace Game.App
{
    /// <summary>
    /// Composition root in the Boot scene. Owns the economy and advances it with frame time.
    /// Other systems (HUD, settlers, debug panel) read it through <see cref="Instance"/>.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] EconomyTuningAsset economyTuning;

        public static GameBootstrap Instance { get; private set; }

        public EconomyModel Economy { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EconomyTuning tuning;
            if (economyTuning != null)
            {
                tuning = economyTuning.values;
            }
            else
            {
                Debug.LogWarning("GameBootstrap: no EconomyTuning asset assigned, using code defaults.", this);
                tuning = new EconomyTuning();
            }
            tuning.Sanitize();
            Economy = new EconomyModel(tuning);
        }

        void Update()
        {
            Economy.Tick(Time.deltaTime);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
