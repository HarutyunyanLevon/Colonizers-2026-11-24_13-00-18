using NUnit.Framework;

namespace Game.Economy.Tests
{
    /// <summary>
    /// Headless bot playthroughs: a player clicking at a steady rate and buying research
    /// whenever affordable. Reports time to 100% so tuning can be checked without a build.
    /// These tests assert only that a run completes; the pacing target (20-40 min) is for
    /// the owner to judge in playtests and is reported in the test output.
    /// </summary>
    public class PacingSimulationTests
    {
        public static double SimulateSecondsToFull(EconomyTuning tuning, double clicksPerSecond,
            double maxSeconds = 6 * 3600, double step = 0.1)
        {
            var e = new EconomyModel(tuning);
            double clickDebt = 0;
            while (!e.IsFull && e.State.ElapsedSeconds < maxSeconds)
            {
                clickDebt += clicksPerSecond * step;
                while (clickDebt >= 1) { e.Click(); clickDebt -= 1; }
                while (e.TryBuyResearch()) { }
                e.Tick(step);
            }
            return e.IsFull ? e.State.ReachedFullAtSeconds : double.PositiveInfinity;
        }

        [TestCase(1.0)]
        [TestCase(3.0)]
        [TestCase(6.0)]
        public void DefaultTuning_BotReachesFull(double clicksPerSecond)
        {
            double seconds = SimulateSecondsToFull(new EconomyTuning(), clicksPerSecond);
            TestContext.WriteLine($"{clicksPerSecond} clicks/s → 100% at {seconds / 60.0:F1} min");
            Assert.IsFalse(double.IsInfinity(seconds));
        }

        [Test]
        public void NoClicks_TakesFarLongerThanClicking()
        {
            double idle = SimulateSecondsToFull(new EconomyTuning(), 0, maxSeconds: 7200);
            double clicking = SimulateSecondsToFull(new EconomyTuning(), 3);
            Assert.Greater(idle, clicking);
        }
    }
}
