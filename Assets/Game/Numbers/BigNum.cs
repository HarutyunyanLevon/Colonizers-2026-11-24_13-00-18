using System;
using System.Globalization;
using ScriptGoblin.NumberForge;

namespace Game.Numbers
{
    /// <summary>
    /// The game-owned big-number type: mantissa × 10^exponent with unlimited range.
    /// This is the only type in the game that touches Number Forge; every other module
    /// uses <see cref="BigNum"/> so the backing library can be swapped.
    /// </summary>
    public readonly struct BigNum : IComparable<BigNum>, IEquatable<BigNum>
    {
        readonly BigNumber _value;

        BigNum(BigNumber value) { _value = value; }

        public static BigNum Zero => new BigNum(BigNumber.Zero);
        public static BigNum One => new BigNum(BigNumber.One);

        /// <summary>Normalized significand: [1, 10) for positive values, 0 for zero.</summary>
        public double Mantissa => _value.Mantissa;
        public long Exponent => _value.Exponent;
        public bool IsZero => _value.IsZero;
        public bool IsNegative => _value.IsNegative;
        public bool IsPositive => _value.IsPositive;

        public static BigNum FromParts(double mantissa, long exponent)
        {
            if (double.IsNaN(mantissa) || double.IsInfinity(mantissa))
                throw new ArgumentException("Mantissa must be finite.", nameof(mantissa));
            return mantissa == 0 ? Zero : new BigNum(new BigNumber(mantissa, exponent));
        }

        public static BigNum FromDouble(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be finite.", nameof(value));
            return value == 0 ? Zero : new BigNum(new BigNumber(value, 0));
        }

        public static implicit operator BigNum(int value) => FromDouble(value);
        public static implicit operator BigNum(long value) => FromDouble(value);
        public static implicit operator BigNum(double value) => FromDouble(value);

        public static BigNum operator +(BigNum a, BigNum b) => new BigNum(a._value + b._value);
        public static BigNum operator -(BigNum a, BigNum b) => new BigNum(a._value - b._value);
        public static BigNum operator *(BigNum a, BigNum b) => new BigNum(a._value * b._value);
        public static BigNum operator /(BigNum a, BigNum b)
        {
            if (b.IsZero) throw new DivideByZeroException();
            return new BigNum(a._value / b._value);
        }
        public static BigNum operator -(BigNum a) => new BigNum(-a._value);

        public static bool operator ==(BigNum a, BigNum b) => a.CompareTo(b) == 0;
        public static bool operator !=(BigNum a, BigNum b) => a.CompareTo(b) != 0;
        public static bool operator <(BigNum a, BigNum b) => a.CompareTo(b) < 0;
        public static bool operator >(BigNum a, BigNum b) => a.CompareTo(b) > 0;
        public static bool operator <=(BigNum a, BigNum b) => a.CompareTo(b) <= 0;
        public static bool operator >=(BigNum a, BigNum b) => a.CompareTo(b) >= 0;

        public static BigNum Min(BigNum a, BigNum b) => a <= b ? a : b;
        public static BigNum Max(BigNum a, BigNum b) => a >= b ? a : b;
        public static BigNum Clamp(BigNum value, BigNum min, BigNum max) => Max(min, Min(max, value));

        public BigNum Floor() => new BigNum(_value.Floor());
        public BigNum Pow(double power) => power == 0 ? One : new BigNum(_value.Pow(power));
        public double Log10() => _value.Log10();

        /// <summary>Converts to double; returns +/- infinity when out of double range.</summary>
        public double ToDouble() => IsZero ? 0 : _value.ToDouble();

        /// <summary>
        /// Sign-aware comparison. Number Forge's own CompareTo treats zero as exponent 0,
        /// so it reports 0 &gt; 0.5; this implementation handles zero and signs first.
        /// </summary>
        public int CompareTo(BigNum other)
        {
            int sa = Math.Sign(Mantissa), sb = Math.Sign(other.Mantissa);
            if (sa != sb) return sa.CompareTo(sb);
            if (sa == 0) return 0;
            if (Exponent != other.Exponent)
                return sa > 0 ? Exponent.CompareTo(other.Exponent) : other.Exponent.CompareTo(Exponent);
            return Mantissa.CompareTo(other.Mantissa);
        }
        public bool Equals(BigNum other) => CompareTo(other) == 0;
        public override bool Equals(object obj) => obj is BigNum other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();

        /// <summary>Short display form for the HUD and debug UI (e.g. "1.5K", "3.2M").</summary>
        public string Format() => _value.ToFormattedString();

        public override string ToString() => Format();

        /// <summary>
        /// Culture-independent, lossless text form for save files: "0" or "&lt;mantissa:R&gt;e&lt;exponent&gt;".
        /// Never depends on the machine's decimal separator.
        /// </summary>
        public string ToSaveString()
        {
            if (IsZero) return "0";
            return Mantissa.ToString("R", CultureInfo.InvariantCulture) + "e" +
                   Exponent.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Parses the output of <see cref="ToSaveString"/>. Never throws.</summary>
        public static bool TryParseSave(string text, out BigNum value)
        {
            value = Zero;
            if (string.IsNullOrWhiteSpace(text)) return false;
            text = text.Trim();
            int e = text.IndexOfAny(new[] { 'e', 'E' });
            string mantissaText = e < 0 ? text : text.Substring(0, e);
            string exponentText = e < 0 ? "0" : text.Substring(e + 1);
            if (!double.TryParse(mantissaText, NumberStyles.Float, CultureInfo.InvariantCulture, out double mantissa))
                return false;
            if (!long.TryParse(exponentText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long exponent))
                return false;
            if (double.IsNaN(mantissa) || double.IsInfinity(mantissa)) return false;
            value = FromParts(mantissa, exponent);
            return true;
        }
    }
}
