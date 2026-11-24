using System;
using Game.Numbers;

namespace Game.Economy
{
    /// <summary>
    /// v1 economy rules in plain C#, no engine types. Deterministic: the same tuning,
    /// starting state and sequence of calls always give the same state.
    /// Progress is a formula, not a simulation: progress/s = settlers × progress per settler per second.
    /// </summary>
    public sealed class EconomyModel
    {
        public EconomyTuning Tuning { get; }
        public EconomyState State { get; private set; }

        /// <summary>Raised once, the first time colonization reaches the cap in this run.</summary>
        public event Action ReachedFull;

        public EconomyModel(EconomyTuning tuning, EconomyState state = null)
        {
            Tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
            State = state ?? EconomyState.CreateNew(tuning);
        }

        public BigNum MaxColonization => BigNum.FromDouble(Tuning.maxColonizationPercent);
        public bool IsFull => State.ColonizationPercent >= MaxColonization;

        public BigNum SettlersPerClick =>
            BigNum.FromDouble(Tuning.baseSettlersPerClick) +
            BigNum.FromDouble(Tuning.settlersPerClickPerResearchLevel) * State.SettlersPerClickResearchLevel;

        /// <summary>Colonization percentage points gained per second right now (0 once full).</summary>
        public BigNum ProgressPerSecond =>
            IsFull ? BigNum.Zero : State.Settlers * Tuning.progressPercentPerSettlerPerSecond;

        public BigNum ResearchCost =>
            BigNum.FromDouble(Tuning.researchBaseCost) *
            BigNum.FromDouble(Tuning.researchCostGrowth).Pow(State.SettlersPerClickResearchLevel);

        public bool CanBuyResearch => State.ResearchPoints >= ResearchCost;

        /// <summary>
        /// Seconds until 100% at the current rate, assuming no further clicks or research.
        /// 0 when full, +infinity when nothing is progressing.
        /// </summary>
        public double ProjectedSecondsToFull
        {
            get
            {
                if (IsFull) return 0;
                var rate = ProgressPerSecond;
                if (!rate.IsPositive) return double.PositiveInfinity;
                return ((MaxColonization - State.ColonizationPercent) / rate).ToDouble();
            }
        }

        /// <summary>Advances the economy by <paramref name="deltaSeconds"/> of play time.</summary>
        public void Tick(double deltaSeconds)
        {
            if (!(deltaSeconds > 0) || double.IsInfinity(deltaSeconds)) return;
            State.ElapsedSeconds += deltaSeconds;
            if (IsFull) return;

            var before = State.ColonizationPercent;
            var after = BigNum.Min(before + ProgressPerSecond * deltaSeconds, MaxColonization);
            var gained = after - before;
            State.ColonizationPercent = after;
            // DESIGN-Q: research points accrue only from colonization actually gained, so they stop at 100%.
            State.ResearchPoints += gained * Tuning.researchPointsPerPercent;
            CheckFull();
        }

        /// <summary>The player's click: adds settlers-per-click settlers. Returns how many were added.</summary>
        public BigNum Click()
        {
            var added = SettlersPerClick;
            State.Settlers += added;
            State.TotalClicks++;
            return added;
        }

        /// <summary>Buys one level of "+1 settler per click". Fails cleanly (no change) when unaffordable.</summary>
        public bool TryBuyResearch()
        {
            var cost = ResearchCost;
            if (State.ResearchPoints < cost) return false;
            State.ResearchPoints -= cost;
            State.SettlersPerClickResearchLevel++;
            State.ResearchBought++;
            return true;
        }

        // ---- Direct state changes used by debug cheats and loading. Not part of normal play. ----

        public void AddSettlers(BigNum amount) => State.Settlers = BigNum.Max(BigNum.Zero, State.Settlers + amount);

        public void AddResearchPoints(BigNum amount) =>
            State.ResearchPoints = BigNum.Max(BigNum.Zero, State.ResearchPoints + amount);

        public void SetColonizationPercent(double percent)
        {
            State.ColonizationPercent = BigNum.Clamp(BigNum.FromDouble(percent), BigNum.Zero, MaxColonization);
            CheckFull();
        }

        /// <summary>Starts a fresh run with the current tuning.</summary>
        public void ResetRun() => State = EconomyState.CreateNew(Tuning);

        /// <summary>Replaces the state, e.g. after loading a save. Does not raise <see cref="ReachedFull"/> for an old run.</summary>
        public void LoadState(EconomyState state) => State = state ?? throw new ArgumentNullException(nameof(state));

        void CheckFull()
        {
            if (State.ReachedFullAtSeconds >= 0 || !IsFull) return;
            State.ReachedFullAtSeconds = State.ElapsedSeconds;
            ReachedFull?.Invoke();
        }
    }
}
