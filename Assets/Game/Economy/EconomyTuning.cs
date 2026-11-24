using System;

namespace Game.Economy
{
    /// <summary>
    /// Every tunable economy number. Plain serializable data: the Unity side wraps it in a
    /// ScriptableObject (Game.App.EconomyTuningAsset) and the debug panel edits it live.
    /// Values and their reasons are listed in docs/gamedev/TUNING.md.
    /// </summary>
    [Serializable]
    public sealed class EconomyTuning
    {
        /// <summary>Settlers at the start of a run. Design: the player starts with one settler.</summary>
        public double startingSettlers = 1;

        /// <summary>Settlers added by one click before any research. Design: starts at 1.</summary>
        public double baseSettlersPerClick = 1;

        /// <summary>Settlers per click added by each level of the "+1 settler per click" research.</summary>
        public double settlersPerClickPerResearchLevel = 1;

        /// <summary>Colonization percentage points per settler per second. Design: start near 0.0001%.</summary>
        public double progressPercentPerSettlerPerSecond = 0.0001;

        /// <summary>Colonization cap in percent. "100%" means the v1 map is fully colonized.</summary>
        public double maxColonizationPercent = 100;

        // DESIGN-Q: research point accrual rate is TBD in the pack (tuned in playtest).
        /// <summary>Research points earned per percentage point of colonization gained.</summary>
        public double researchPointsPerPercent = 10;

        // DESIGN-Q: research cost curve formula is TBD; exponential cost = base × growth^level used.
        /// <summary>Cost of the first research level, in research points.</summary>
        public double researchBaseCost = 100;

        /// <summary>Cost multiplier per research level already bought.</summary>
        public double researchCostGrowth = 2;

        public EconomyTuning Clone() => (EconomyTuning)MemberwiseClone();

        /// <summary>Clamps values into ranges the economy can run with. Returns true if anything changed.</summary>
        public bool Sanitize()
        {
            bool changed = false;
            changed |= Fix(ref startingSettlers, 0, double.MaxValue);
            changed |= Fix(ref baseSettlersPerClick, 0, double.MaxValue);
            changed |= Fix(ref settlersPerClickPerResearchLevel, 0, double.MaxValue);
            changed |= Fix(ref progressPercentPerSettlerPerSecond, 0, double.MaxValue);
            changed |= Fix(ref maxColonizationPercent, 0, 100);
            changed |= Fix(ref researchPointsPerPercent, 0, double.MaxValue);
            changed |= Fix(ref researchBaseCost, 0, double.MaxValue);
            changed |= Fix(ref researchCostGrowth, 1, double.MaxValue);
            return changed;
        }

        static bool Fix(ref double value, double min, double max)
        {
            double v = double.IsNaN(value) ? min : Math.Max(min, Math.Min(max, value));
            if (v.Equals(value)) return false;
            value = v;
            return true;
        }
    }
}
