using Game.Numbers;

namespace Game.Economy
{
    /// <summary>The whole economy state of a run. Everything a save file needs.</summary>
    public sealed class EconomyState
    {
        public BigNum Settlers;
        public BigNum ColonizationPercent;
        public BigNum ResearchPoints;

        /// <summary>Levels bought of the repeatable "+1 settler per click" research.</summary>
        public int SettlersPerClickResearchLevel;

        /// <summary>Total simulated play time of the run, in seconds.</summary>
        public double ElapsedSeconds;

        /// <summary>Elapsed time at which colonization first hit the cap, or -1 if not yet.</summary>
        public double ReachedFullAtSeconds = -1;

        public long TotalClicks;
        public int ResearchBought;

        public EconomyState Clone() => (EconomyState)MemberwiseClone();

        public static EconomyState CreateNew(EconomyTuning tuning) => new EconomyState
        {
            Settlers = BigNum.FromDouble(tuning.startingSettlers),
            ColonizationPercent = BigNum.Zero,
            ResearchPoints = BigNum.Zero,
        };
    }
}
