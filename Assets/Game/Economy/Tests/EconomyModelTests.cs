using Game.Numbers;
using NUnit.Framework;

namespace Game.Economy.Tests
{
    public class EconomyModelTests
    {
        static EconomyTuning Tuning() => new EconomyTuning
        {
            startingSettlers = 1,
            baseSettlersPerClick = 1,
            settlersPerClickPerResearchLevel = 1,
            progressPercentPerSettlerPerSecond = 0.0001,
            maxColonizationPercent = 100,
            researchPointsPerPercent = 100,
            researchBaseCost = 10,
            researchCostGrowth = 1.5,
        };

        // ---- M1-T3: state, tick, cap, research points, determinism ----

        [Test]
        public void NewRun_StartsWithOneSettlerAndNothingElse()
        {
            var e = new EconomyModel(Tuning());
            Assert.AreEqual(1.0, e.State.Settlers.ToDouble());
            Assert.IsTrue(e.State.ColonizationPercent.IsZero);
            Assert.IsTrue(e.State.ResearchPoints.IsZero);
            Assert.AreEqual(1.0, e.SettlersPerClick.ToDouble());
        }

        [Test]
        public void Tick_AddsSettlersTimesRateTimesTime()
        {
            var e = new EconomyModel(Tuning());
            e.AddSettlers(99); // 100 settlers
            e.Tick(10);        // 100 × 0.0001 × 10 = 0.1 %
            Assert.AreEqual(0.1, e.State.ColonizationPercent.ToDouble(), 1e-12);
            Assert.AreEqual(10.0, e.State.ElapsedSeconds, 1e-12);
        }

        [Test]
        public void Tick_ResearchPointsAccrueWithProgress()
        {
            var e = new EconomyModel(Tuning());
            e.AddSettlers(99);
            e.Tick(10); // 0.1 % × 100 points per % = 10 points
            Assert.AreEqual(10.0, e.State.ResearchPoints.ToDouble(), 1e-9);
        }

        [Test]
        public void Tick_CapsAtMaxAndRaisesReachedFullOnce()
        {
            var e = new EconomyModel(Tuning());
            int raised = 0;
            e.ReachedFull += () => raised++;
            e.AddSettlers(BigNum.FromParts(1, 9));
            e.Tick(1000);
            e.Tick(1000);
            Assert.AreEqual(100.0, e.State.ColonizationPercent.ToDouble());
            Assert.IsTrue(e.IsFull);
            Assert.AreEqual(1, raised);
            Assert.AreEqual(1000.0, e.State.ReachedFullAtSeconds, 1e-9);
            Assert.IsTrue(e.ProgressPerSecond.IsZero);
            Assert.AreEqual(0.0, e.ProjectedSecondsToFull);
        }

        [Test]
        public void Tick_ResearchPointsStopAtCap()
        {
            var e = new EconomyModel(Tuning());
            e.AddSettlers(BigNum.FromParts(1, 9));
            e.Tick(1000);
            var points = e.State.ResearchPoints;
            Assert.AreEqual(100.0 * 100, points.ToDouble(), 1e-6); // only the 100 % actually gained
            e.Tick(1000);
            Assert.AreEqual(points, e.State.ResearchPoints);
        }

        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Tick_IgnoresInvalidDelta(double dt)
        {
            var e = new EconomyModel(Tuning());
            e.Tick(dt);
            Assert.IsTrue(e.State.ColonizationPercent.IsZero);
            Assert.AreEqual(0.0, e.State.ElapsedSeconds);
        }

        [Test]
        public void SameInputs_GiveIdenticalState()
        {
            EconomyState Run()
            {
                var e = new EconomyModel(Tuning());
                for (int i = 0; i < 20000; i++)
                {
                    if (i % 7 == 0) e.Click();
                    if (i % 50 == 0) e.TryBuyResearch();
                    e.Tick(1.0 / 60.0);
                }
                return e.State;
            }

            var a = Run();
            var b = Run();
            Assert.AreEqual(a.Settlers.Mantissa, b.Settlers.Mantissa);
            Assert.AreEqual(a.Settlers.Exponent, b.Settlers.Exponent);
            Assert.AreEqual(a.ColonizationPercent.Mantissa, b.ColonizationPercent.Mantissa);
            Assert.AreEqual(a.ColonizationPercent.Exponent, b.ColonizationPercent.Exponent);
            Assert.AreEqual(a.ResearchPoints.Mantissa, b.ResearchPoints.Mantissa);
            Assert.AreEqual(a.ResearchPoints.Exponent, b.ResearchPoints.Exponent);
            Assert.AreEqual(a.SettlersPerClickResearchLevel, b.SettlersPerClickResearchLevel);
            Assert.AreEqual(a.TotalClicks, b.TotalClicks);
        }

        [Test]
        public void ProjectedSecondsToFull_IsRemainingOverRate()
        {
            var e = new EconomyModel(Tuning());
            e.AddSettlers(9); // 10 settlers → 0.001 %/s → 100 000 s to 100 %
            Assert.AreEqual(100000.0, e.ProjectedSecondsToFull, 1e-6);
            e.AddSettlers(-10); // 0 settlers
            Assert.IsTrue(double.IsPositiveInfinity(e.ProjectedSecondsToFull));
        }

        // ---- M1-T4: click and research ----

        [Test]
        public void Click_AddsSettlersPerClick()
        {
            var e = new EconomyModel(Tuning());
            var added = e.Click();
            Assert.AreEqual(1.0, added.ToDouble());
            Assert.AreEqual(2.0, e.State.Settlers.ToDouble());
            Assert.AreEqual(1, e.State.TotalClicks);
        }

        [Test]
        public void Research_AddsOneSettlerPerClickPerLevel()
        {
            var e = new EconomyModel(Tuning());
            e.AddResearchPoints(10 + 15);
            Assert.IsTrue(e.TryBuyResearch());
            Assert.AreEqual(2.0, e.SettlersPerClick.ToDouble());
            Assert.IsTrue(e.TryBuyResearch());
            Assert.AreEqual(3.0, e.SettlersPerClick.ToDouble());
            e.Click();
            Assert.AreEqual(4.0, e.State.Settlers.ToDouble());
        }

        [Test]
        public void ResearchCost_FollowsConfigurableCurve()
        {
            var t = Tuning();
            var e = new EconomyModel(t);
            Assert.AreEqual(10.0, e.ResearchCost.ToDouble(), 1e-9);
            e.AddResearchPoints(10);
            e.TryBuyResearch();
            Assert.AreEqual(15.0, e.ResearchCost.ToDouble(), 1e-9);

            t.researchBaseCost = 100;
            t.researchCostGrowth = 2;
            Assert.AreEqual(200.0, e.ResearchCost.ToDouble(), 1e-9); // live tuning applies immediately
        }

        [Test]
        public void ResearchCost_HandlesHugeLevels()
        {
            var e = new EconomyModel(Tuning());
            e.State.SettlersPerClickResearchLevel = 5000; // 10 × 1.5^5000 ≈ 1e881, far past double range
            var cost = e.ResearchCost;
            Assert.IsTrue(cost.IsPositive);
            Assert.AreEqual(881, cost.Exponent);
        }

        [Test]
        public void Research_Unaffordable_FailsWithoutChangingState()
        {
            var e = new EconomyModel(Tuning());
            e.AddResearchPoints(9.99);
            var before = e.State.Clone();
            Assert.IsFalse(e.CanBuyResearch);
            Assert.IsFalse(e.TryBuyResearch());
            Assert.AreEqual(before.ResearchPoints, e.State.ResearchPoints);
            Assert.AreEqual(before.SettlersPerClickResearchLevel, e.State.SettlersPerClickResearchLevel);
            Assert.AreEqual(before.ResearchBought, e.State.ResearchBought);
        }

        [Test]
        public void Research_Affordable_SpendsExactCost()
        {
            var e = new EconomyModel(Tuning());
            e.AddResearchPoints(12);
            Assert.IsTrue(e.CanBuyResearch);
            Assert.IsTrue(e.TryBuyResearch());
            Assert.AreEqual(2.0, e.State.ResearchPoints.ToDouble(), 1e-9);
        }

        [Test]
        public void Research_SpendingNeverLowersColonization()
        {
            var e = new EconomyModel(Tuning());
            e.AddSettlers(999);
            e.Tick(100); // 10 %
            var colonized = e.State.ColonizationPercent;
            int bought = 0;
            while (e.TryBuyResearch()) bought++;
            Assert.Greater(bought, 0);
            Assert.AreEqual(colonized, e.State.ColonizationPercent);
        }

        // ---- Cheat entry points ----

        [Test]
        public void SetColonizationPercent_ClampsAndRaisesFull()
        {
            var e = new EconomyModel(Tuning());
            int raised = 0;
            e.ReachedFull += () => raised++;
            e.SetColonizationPercent(150);
            Assert.AreEqual(100.0, e.State.ColonizationPercent.ToDouble());
            Assert.AreEqual(1, raised);
            e.SetColonizationPercent(-5);
            Assert.IsTrue(e.State.ColonizationPercent.IsZero);
        }

        [Test]
        public void AddSettlers_NeverGoesNegative()
        {
            var e = new EconomyModel(Tuning());
            e.AddSettlers(-50);
            Assert.IsTrue(e.State.Settlers.IsZero);
        }

        [Test]
        public void Tuning_SanitizeFixesBadValues()
        {
            var t = Tuning();
            t.researchCostGrowth = 0.5;
            t.maxColonizationPercent = 500;
            t.progressPercentPerSettlerPerSecond = double.NaN;
            Assert.IsTrue(t.Sanitize());
            Assert.AreEqual(1.0, t.researchCostGrowth);
            Assert.AreEqual(100.0, t.maxColonizationPercent);
            Assert.AreEqual(0.0, t.progressPercentPerSettlerPerSecond);
            Assert.IsFalse(t.Sanitize());
        }
    }
}
