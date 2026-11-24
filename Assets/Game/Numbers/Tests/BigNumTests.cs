using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Game.Numbers.Tests
{
    public class BigNumTests
    {
        [Test]
        public void Add_SmallAndLarge()
        {
            Assert.AreEqual(5.0, ((BigNum)2 + 3).ToDouble(), 1e-12);
            Assert.AreEqual(1.5e20, (BigNum.FromParts(1, 20) + BigNum.FromParts(5, 19)).ToDouble(), 1e8);
        }

        [Test]
        public void Add_FarBeyondDoubleRange_Works()
        {
            var a = BigNum.FromParts(1, 400);
            var sum = a + a;
            Assert.AreEqual(2.0, sum.Mantissa, 1e-12);
            Assert.AreEqual(400, sum.Exponent);
        }

        [Test]
        public void Multiply_AndDivide()
        {
            var a = BigNum.FromParts(2, 300) * BigNum.FromParts(3, 300);
            Assert.AreEqual(6.0, a.Mantissa, 1e-12);
            Assert.AreEqual(600, a.Exponent);
            Assert.AreEqual(4.0, ((BigNum)10 / 2.5).ToDouble(), 1e-12);
            Assert.IsTrue(((BigNum)0 * BigNum.FromParts(9, 999)).IsZero);
        }

        [Test]
        public void Subtract_ToZero_IsZero()
        {
            Assert.IsTrue(((BigNum)7 - 7).IsZero);
        }

        [Test]
        public void Compare_ZeroAgainstFractions()
        {
            // Regression: Number Forge's own CompareTo reports 0 > 0.5.
            Assert.IsTrue(BigNum.Zero < 0.5);
            Assert.IsTrue(BigNum.Zero < 0.0001);
            Assert.IsTrue(BigNum.Zero > -0.5);
            Assert.IsTrue((BigNum)0.5 > BigNum.Zero);
        }

        [Test]
        public void Compare_OrdersAcrossExponentsAndSigns()
        {
            Assert.IsTrue((BigNum)9 < 10);
            Assert.IsTrue((BigNum)(-10) < -9);
            Assert.IsTrue((BigNum)(-1) < 1);
            Assert.IsTrue(BigNum.FromParts(1, 100) > BigNum.FromParts(9.99, 99));
            Assert.IsTrue((BigNum)3 == 3.0);
            Assert.IsTrue((BigNum)3 <= 3 && (BigNum)3 >= 3);
            Assert.AreEqual(2.0, BigNum.Min(2, 5).ToDouble());
            Assert.AreEqual(5.0, BigNum.Max(2, 5).ToDouble());
            Assert.AreEqual(100.0, BigNum.Clamp(150, 0, 100).ToDouble());
        }

        [Test]
        public void Pow_ZeroPower_IsOne()
        {
            Assert.AreEqual(1.0, ((BigNum)1.5).Pow(0).ToDouble(), 1e-12);
            Assert.AreEqual(2.25, ((BigNum)1.5).Pow(2).ToDouble(), 1e-9);
        }

        [Test]
        public void Format_ShortForm()
        {
            Assert.IsFalse(string.IsNullOrEmpty(BigNum.FromParts(1.5, 6).Format()));
            Assert.AreNotEqual(BigNum.FromParts(1.5, 6).Format(), BigNum.FromParts(1.5, 9).Format());
        }

        [TestCase(0.0, 0)]
        [TestCase(1.0, 0)]
        [TestCase(1.2345678901234, 7)]
        [TestCase(-3.25, -12)]
        [TestCase(9.999999999, 123456)]
        public void SaveString_RoundTrips(double mantissa, long exponent)
        {
            var v = BigNum.FromParts(mantissa, exponent);
            Assert.IsTrue(BigNum.TryParseSave(v.ToSaveString(), out var back));
            Assert.AreEqual(v.Mantissa, back.Mantissa);
            Assert.AreEqual(v.Exponent, back.Exponent);
        }

        [Test]
        public void SaveString_IgnoresMachineCulture()
        {
            var old = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU"); // comma decimal separator
                var v = BigNum.FromParts(1.5, 3);
                string s = v.ToSaveString();
                StringAssert.DoesNotContain(",", s);
                Assert.IsTrue(BigNum.TryParseSave(s, out var back));
                Assert.AreEqual(v, back);
            }
            finally { Thread.CurrentThread.CurrentCulture = old; }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("abc")]
        [TestCase("1e")]
        [TestCase("NaN")]
        public void TryParseSave_RejectsGarbage(string text)
        {
            Assert.IsFalse(BigNum.TryParseSave(text, out var v));
            Assert.IsTrue(v.IsZero);
        }
    }
}
