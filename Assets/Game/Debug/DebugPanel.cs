using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Game.App;
using Game.Economy;
using Game.Numbers;
using UnityEngine;

namespace Game.DebugTools
{
    /// <summary>
    /// Development-only IMGUI panel: economy state, the playable v1 loop (click / research buttons),
    /// live tuning of every economy value, and cheats. The whole Game.Debug assembly is compiled only
    /// in the Editor and in development builds (asmdef define constraint), so none of this ships.
    /// Toggle with F1.
    /// </summary>
    public sealed class DebugPanel : MonoBehaviour
    {
        const float ReferenceHeight = 1080f;
        const float PanelWidth = 430f;

        static readonly float[] TimeScales = { 1f, 10f, 100f };

        bool _visible = true;
        Vector2 _scroll;
        bool _showTuning = true;
        bool _showCheats = true;

        readonly List<FieldInfo> _tuningFields = new List<FieldInfo>();
        readonly Dictionary<string, string> _tuningText = new Dictionary<string, string>();
        EconomyTuning _boundTuning;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Create()
        {
            if (FindAnyObjectByType<DebugPanel>() != null) return;
            var go = new GameObject("[DebugPanel]");
            DontDestroyOnLoad(go);
            go.AddComponent<DebugPanel>();
        }

        void Awake()
        {
            foreach (var f in typeof(EconomyTuning).GetFields(BindingFlags.Public | BindingFlags.Instance))
                if (f.FieldType == typeof(double)) _tuningFields.Add(f);
        }

        void OnGUI()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.F1)
            {
                _visible = !_visible;
                e.Use();
            }
            if (!_visible) return;

            float scale = Mathf.Max(1f, Screen.height / ReferenceHeight);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            float height = Screen.height / scale - 20f;

            GUILayout.BeginArea(new Rect(10, 10, PanelWidth, height), GUI.skin.box);
            _scroll = GUILayout.BeginScrollView(_scroll);
            GUILayout.Label("<b>ColonizeEarth debug (F1)</b>", RichLabel);

            var boot = GameBootstrap.Instance;
            if (boot == null || boot.Economy == null)
            {
                GUILayout.Label("No GameBootstrap in the scene. Open Assets/Scenes/Boot.unity.");
            }
            else
            {
                DrawState(boot.Economy);
                DrawLoop(boot.Economy);
                DrawCheats(boot.Economy);
                DrawTuning(boot.Economy.Tuning);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        static GUIStyle _richLabel;
        static GUIStyle RichLabel => _richLabel ??= new GUIStyle(GUI.skin.label) { richText = true };

        void DrawState(EconomyModel eco)
        {
            var s = eco.State;
            GUILayout.Label($"Elapsed play time: {FormatTime(s.ElapsedSeconds)}");
            GUILayout.Label($"Colonization: {s.ColonizationPercent.ToDouble():F4} %");
            GUILayout.Label($"Projected to 100% at current rate: {FormatTime(eco.ProjectedSecondsToFull)}");
            GUILayout.Label(s.ReachedFullAtSeconds >= 0
                ? $"Reached 100% at: {FormatTime(s.ReachedFullAtSeconds)}"
                : "Reached 100% at: not yet");
            GUILayout.Label($"Settlers: {s.Settlers.Format()}   per click: {eco.SettlersPerClick.Format()}");
            GUILayout.Label($"Progress: {eco.ProgressPerSecond.ToDouble():F6} %/s");
            GUILayout.Label($"Research points: {s.ResearchPoints.Format()}");
            GUILayout.Label($"Research level (+1/click): {s.SettlersPerClickResearchLevel}");
            GUILayout.Label($"Clicks: {s.TotalClicks}   research bought: {s.ResearchBought}");
            GUILayout.Label($"Time scale: {Time.timeScale:0.#}x");
        }

        void DrawLoop(EconomyModel eco)
        {
            GUILayout.Space(6);
            if (GUILayout.Button($"CLICK: spawn {eco.SettlersPerClick.Format()} settler(s)", GUILayout.Height(48)))
                eco.Click();

            bool enabled = GUI.enabled;
            GUI.enabled = eco.CanBuyResearch;
            if (GUILayout.Button($"Research +1 settler per click (cost {eco.ResearchCost.Format()} RP)", GUILayout.Height(36)))
                eco.TryBuyResearch();
            GUI.enabled = enabled;
        }

        void DrawCheats(EconomyModel eco)
        {
            GUILayout.Space(6);
            _showCheats = GUILayout.Toggle(_showCheats, " Cheats");
            if (!_showCheats) return;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+10 settlers")) eco.AddSettlers(10);
            if (GUILayout.Button("+1K")) eco.AddSettlers(1000);
            if (GUILayout.Button("x10")) eco.AddSettlers(eco.State.Settlers * 9);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+100 RP")) eco.AddResearchPoints(100);
            if (GUILayout.Button("+10K RP")) eco.AddResearchPoints(10000);
            if (GUILayout.Button("RP = 0")) eco.AddResearchPoints(-eco.State.ResearchPoints);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set 0%")) eco.SetColonizationPercent(0);
            if (GUILayout.Button("50%")) eco.SetColonizationPercent(50);
            if (GUILayout.Button("99.9%")) eco.SetColonizationPercent(99.9);
            if (GUILayout.Button("100%")) eco.SetColonizationPercent(100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            foreach (var ts in TimeScales)
                if (GUILayout.Button($"Time {ts:0}x")) Time.timeScale = ts;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset run")) eco.ResetRun();
        }

        void DrawTuning(EconomyTuning tuning)
        {
            GUILayout.Space(6);
            _showTuning = GUILayout.Toggle(_showTuning, " Tuning (applies live)");
            if (!_showTuning) return;

            if (_boundTuning != tuning)
            {
                _boundTuning = tuning;
                _tuningText.Clear();
            }

            foreach (var f in _tuningFields)
            {
                double current = (double)f.GetValue(tuning);
                if (!_tuningText.TryGetValue(f.Name, out var text))
                    text = current.ToString("R", CultureInfo.InvariantCulture);

                GUILayout.BeginHorizontal();
                GUILayout.Label(f.Name, GUILayout.Width(250));
                string edited = GUILayout.TextField(text, GUILayout.Width(140));
                GUILayout.EndHorizontal();

                _tuningText[f.Name] = edited;
                if (edited != text &&
                    double.TryParse(edited, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
                {
                    f.SetValue(tuning, parsed);
                    if (tuning.Sanitize()) _tuningText.Clear(); // show the clamped value
                }
            }
            GUILayout.Label("Decimal point is '.'. Record kept values in docs/gamedev/TUNING.md.");
        }

        static string FormatTime(double seconds)
        {
            if (double.IsInfinity(seconds) || double.IsNaN(seconds)) return "never";
            if (seconds > 99 * 3600) return $"{seconds / 3600.0:F0} h";
            var t = TimeSpan.FromSeconds(seconds);
            return t.TotalHours >= 1 ? $"{(int)t.TotalHours}:{t.Minutes:00}:{t.Seconds:00}" : $"{t.Minutes:00}:{t.Seconds:00}";
        }
    }
}
