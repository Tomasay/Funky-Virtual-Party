using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Glitch9.Reflection;


namespace Glitch9
{
    internal class TypeInformation
    {
        public Type Type { get; }
        public TypeCode TypeCode { get; }

        public TypeInformation(Type type, TypeCode typeCode)
        {
            Type = type;
            TypeCode = typeCode;
        }
    }

    internal enum ParseResult
    {
        None = 0,
        Success = 1,
        Overflow = 2,
        Invalid = 3
    }

    internal static class TypeUtil
    {
        private static readonly Dictionary<Type, TypeCode> kTypeCodeMap =
            new()
            {
                { typeof(char), TypeCode.Char },
                { typeof(bool), TypeCode.Boolean },
                { typeof(sbyte), TypeCode.SByte },
                { typeof(short), TypeCode.Int16 },
                { typeof(ushort), TypeCode.UInt16 },
                { typeof(int), TypeCode.Int32 },
                { typeof(byte), TypeCode.Byte },
                { typeof(uint), TypeCode.UInt32 },
                { typeof(long), TypeCode.Int64 },
                { typeof(ulong), TypeCode.UInt64 },
                { typeof(float), TypeCode.Single },
                { typeof(double), TypeCode.Double },
                { typeof(DateTime), TypeCode.DateTime },
                { typeof(decimal), TypeCode.Decimal },
                { typeof(string), TypeCode.String },
                { typeof(DBNull), TypeCode.DBNull }
            };


        private static readonly TypeInformation[] kTypeCodes =
        {
            // need all of these. lookup against the index with TypeCode value
            new(typeof(object), TypeCode.Empty),
            new(typeof(object), TypeCode.Object),
            new(typeof(object), TypeCode.DBNull),
            new(typeof(bool), TypeCode.Boolean),
            new(typeof(char), TypeCode.Char),
            new(typeof(sbyte), TypeCode.SByte),
            new(typeof(byte), TypeCode.Byte),
            new(typeof(short), TypeCode.Int16),
            new(typeof(ushort), TypeCode.UInt16),
            new(typeof(int), TypeCode.Int32),
            new(typeof(uint), TypeCode.UInt32),
            new(typeof(long), TypeCode.Int64),
            new(typeof(ulong), TypeCode.UInt64),
            new(typeof(float), TypeCode.Single),
            new(typeof(double), TypeCode.Double),
            new(typeof(decimal), TypeCode.Decimal),
            new(typeof(DateTime), TypeCode.DateTime),
            new(typeof(object), TypeCode.Empty), // no 17 in TypeCode for some reason
            new(typeof(string), TypeCode.String)
        };


        public static TypeCode GetTypeCode(Type t)
        {
            return GetTypeCode(t, out _);
        }

        public static TypeCode GetTypeCode(Type t, out bool isEnum)
        {
            if (kTypeCodeMap.TryGetValue(t, out TypeCode typeCode))
            {
                isEnum = false;
                return typeCode;
            }

            if (t.IsEnum)
            {
                isEnum = true;
                return GetTypeCode(Enum.GetUnderlyingType(t));
            }

            // performance?
            if (ReflectionUtil.IsNullableType(t))
            {
                Type nonNullable = Nullable.GetUnderlyingType(t)!;
                if (nonNullable.IsEnum)
                {
                    Type nullableUnderlyingType = typeof(Nullable<>).MakeGenericType(Enum.GetUnderlyingType(nonNullable));
                    isEnum = true;
                    return GetTypeCode(nullableUnderlyingType);
                }
            }

            isEnum = false;
            return TypeCode.Object;
        }


        public static TypeInformation GetTypeInformation(IConvertible convertable)
        {
            TypeInformation typeInformation = kTypeCodes[(int)convertable.GetTypeCode()];
            return typeInformation;
        }

        public static bool IsConvertible(Type t)
        {
            return typeof(IConvertible).IsAssignableFrom(t);
        }

        public static TimeSpan ParseTimeSpan(string input)
        {
            return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
        }

        public static bool VersionTryParse(string input, [NotNullWhen(true)] out Version result)
        {
            try
            {
                result = new Version(input);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static ParseResult Int32TryParse(char[] chars, int start, int length, out int value)
        {
            value = 0;

            if (length == 0)
            {
                return ParseResult.Invalid;
            }

            bool isNegative = (chars[start] == '-');

            if (isNegative)
            {
                // text just a negative sign
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }

                start++;
                length--;
            }

            int end = start + length;

            // Int32.MaxValue and MinValue are 10 chars
            // Or is 10 chars and start is greater than two
            // Need to improve this!
            if (length > 10 || (length == 10 && chars[start] - '0' > 2))
            {
                // invalid result takes precedence over overflow
                for (int i = start; i < end; i++)
                {
                    int c = chars[i] - '0';

                    if (c < 0 || c > 9)
                    {
                        return ParseResult.Invalid;
                    }
                }

                return ParseResult.Overflow;
            }

            for (int i = start; i < end; i++)
            {
                int c = chars[i] - '0';

                if (c < 0 || c > 9)
                {
                    return ParseResult.Invalid;
                }

                int newValue = (10 * value) - c;

                // overflow has caused the number to loop around
                if (newValue > value)
                {
                    i++;

                    // double check the rest of the string that there wasn't anything invalid
                    // invalid result takes precedence over overflow result
                    for (; i < end; i++)
                    {
                        c = chars[i] - '0';

                        if (c < 0 || c > 9)
                        {
                            return ParseResult.Invalid;
                        }
                    }

                    return ParseResult.Overflow;
                }

                value = newValue;
            }

            // go from negative to positive to avoids overflow
            // negative can be slightly bigger than positive
            if (!isNegative)
            {
                // negative integer can be one bigger than positive
                if (value == int.MinValue)
                {
                    return ParseResult.Overflow;
                }

                value = -value;
            }

            return ParseResult.Success;
        }

        public static ParseResult Int64TryParse(char[] chars, int start, int length, out long value)
        {
            value = 0;

            if (length == 0)
            {
                return ParseResult.Invalid;
            }

            bool isNegative = (chars[start] == '-');

            if (isNegative)
            {
                // text just a negative sign
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }

                start++;
                length--;
            }

            int end = start + length;

            // Int64.MaxValue and MinValue are 19 chars
            if (length > 19)
            {
                // invalid result takes precedence over overflow
                for (int i = start; i < end; i++)
                {
                    int c = chars[i] - '0';

                    if (c < 0 || c > 9)
                    {
                        return ParseResult.Invalid;
                    }
                }

                return ParseResult.Overflow;
            }

            for (int i = start; i < end; i++)
            {
                int c = chars[i] - '0';

                if (c < 0 || c > 9)
                {
                    return ParseResult.Invalid;
                }

                long newValue = (10 * value) - c;

                // overflow has caused the number to loop around
                if (newValue > value)
                {
                    i++;

                    // double check the rest of the string that there wasn't anything invalid
                    // invalid result takes precedence over overflow result
                    for (; i < end; i++)
                    {
                        c = chars[i] - '0';

                        if (c < 0 || c > 9)
                        {
                            return ParseResult.Invalid;
                        }
                    }

                    return ParseResult.Overflow;
                }

                value = newValue;
            }

            // go from negative to positive to avoids overflow
            // negative can be slightly bigger than positive
            if (!isNegative)
            {
                // negative integer can be one bigger than positive
                if (value == long.MinValue)
                {
                    return ParseResult.Overflow;
                }

                value = -value;
            }

            return ParseResult.Success;
        }

#if HAS_CUSTOM_DOUBLE_PARSE
        private static class IEEE754
        {
            /// <summary>
            /// Exponents for both powers of 10 and 0.1
            /// </summary>
            private static readonly int[] MultExp64Power10 = new int[]
            {
                4, 7, 10, 14, 17, 20, 24, 27, 30, 34, 37, 40, 44, 47, 50
            };

            /// <summary>
            /// Normalized powers of 10
            /// </summary>
            private static readonly ulong[] MultVal64Power10 = new ulong[]
            {
                0xa000000000000000, 0xc800000000000000, 0xfa00000000000000,
                0x9c40000000000000, 0xc350000000000000, 0xf424000000000000,
                0x9896800000000000, 0xbebc200000000000, 0xee6b280000000000,
                0x9502f90000000000, 0xba43b74000000000, 0xe8d4a51000000000,
                0x9184e72a00000000, 0xb5e620f480000000, 0xe35fa931a0000000,
            };

            /// <summary>
            /// Normalized powers of 0.1
            /// </summary>
            private static readonly ulong[] MultVal64Power10Inv = new ulong[]
            {
                0xcccccccccccccccd, 0xa3d70a3d70a3d70b, 0x83126e978d4fdf3c,
                0xd1b71758e219652e, 0xa7c5ac471b478425, 0x8637bd05af6c69b7,
                0xd6bf94d5e57a42be, 0xabcc77118461ceff, 0x89705f4136b4a599,
                0xdbe6fecebdedd5c2, 0xafebff0bcb24ab02, 0x8cbccc096f5088cf,
                0xe12e13424bb40e18, 0xb424dc35095cd813, 0x901d7cf73ab0acdc,
            };

            /// <summary>
            /// Exponents for both powers of 10^16 and 0.1^16
            /// </summary>
            private static readonly int[] MultExp64Power10By16 = new int[]
            {
                54, 107, 160, 213, 266, 319, 373, 426, 479, 532, 585, 638,
                691, 745, 798, 851, 904, 957, 1010, 1064, 1117,
            };

            /// <summary>
            /// Normalized powers of 10^16
            /// </summary>
            private static readonly ulong[] MultVal64Power10By16 = new ulong[]
            {
                0x8e1bc9bf04000000, 0x9dc5ada82b70b59e, 0xaf298d050e4395d6,
                0xc2781f49ffcfa6d4, 0xd7e77a8f87daf7fa, 0xefb3ab16c59b14a0,
                0x850fadc09923329c, 0x93ba47c980e98cde, 0xa402b9c5a8d3a6e6,
                0xb616a12b7fe617a8, 0xca28a291859bbf90, 0xe070f78d39275566,
                0xf92e0c3537826140, 0x8a5296ffe33cc92c, 0x9991a6f3d6bf1762,
                0xaa7eebfb9df9de8a, 0xbd49d14aa79dbc7e, 0xd226fc195c6a2f88,
                0xe950df20247c83f8, 0x81842f29f2cce373, 0x8fcac257558ee4e2,
            };

            /// <summary>
            /// Normalized powers of 0.1^16
            /// </summary>
            private static readonly ulong[] MultVal64Power10By16Inv = new ulong[]
            {
                0xe69594bec44de160, 0xcfb11ead453994c3, 0xbb127c53b17ec165,
                0xa87fea27a539e9b3, 0x97c560ba6b0919b5, 0x88b402f7fd7553ab,
                0xf64335bcf065d3a0, 0xddd0467c64bce4c4, 0xc7caba6e7c5382ed,
                0xb3f4e093db73a0b7, 0xa21727db38cb0053, 0x91ff83775423cc29,
                0x8380dea93da4bc82, 0xece53cec4a314f00, 0xd5605fcdcf32e217,
                0xc0314325637a1978, 0xad1c8eab5ee43ba2, 0x9becce62836ac5b0,
                0x8c71dcd9ba0b495c, 0xfd00b89747823938, 0xe3e27a444d8d991a,
            };

            /// <summary>
            /// Packs <paramref name="val"/>*10^<paramref name="scale"/> as 64-bit floating point value according to IEEE 754 standard
            /// </summary>
            /// <param name="negative">Sign</param>
            /// <param name="val">Mantissa</param>
            /// <param name="scale">Exponent</param>
            /// <remarks>
            /// Adoption of native function NumberToDouble() from coreclr sources,
            /// see https://github.com/dotnet/coreclr/blob/master/src/classlibnative/bcltype/number.cpp#L451
            /// </remarks>
            public static double PackDouble(bool negative, ulong val, int scale)
            {
                // handle zero value
                if (val == 0)
                {
                    return negative ? -0.0 : 0.0;
                }

                // normalize the mantissa
                int exp = 64;

                if ((val & 0xFFFFFFFF00000000) == 0)
                {
                    val <<= 32;
                    exp -= 32;
                }
                if ((val & 0xFFFF000000000000) == 0)
                {
                    val <<= 16;
                    exp -= 16;
                }
                if ((val & 0xFF00000000000000) == 0)
                {
                    val <<= 8;
                    exp -= 8;
                }
                if ((val & 0xF000000000000000) == 0)
                {
                    val <<= 4;
                    exp -= 4;
                }
                if ((val & 0xC000000000000000) == 0)
                {
                    val <<= 2;
                    exp -= 2;
                }
                if ((val & 0x8000000000000000) == 0)
                {
                    val <<= 1;
                    exp -= 1;
                }

                if (scale < 0)
                {
                    scale = -scale;

                    // check scale bounds
                    if (scale >= 22 * 16)
                    {
                        // underflow
                        return negative ? -0.0 : 0.0;
                    }

                    // perform scaling
                    int index = scale & 15;
                    if (index != 0)
                    {
                        exp -= MultExp64Power10[index - 1] - 1;
                        val = Mul64Lossy(val, MultVal64Power10Inv[index - 1], ref exp);
                    }

                    index = scale >> 4;
                    if (index != 0)
                    {
                        exp -= MultExp64Power10By16[index - 1] - 1;
                        val = Mul64Lossy(val, MultVal64Power10By16Inv[index - 1], ref exp);
                    }
                }
                else
                {
                    // check scale bounds
                    if (scale >= 22 * 16)
                    {
                        // overflow
                        return negative ? double.NegativeInfinity : double.PositiveInfinity;
                    }

                    // perform scaling
                    int index = scale & 15;
                    if (index != 0)
                    {
                        exp += MultExp64Power10[index - 1];
                        val = Mul64Lossy(val, MultVal64Power10[index - 1], ref exp);
                    }

                    index = scale >> 4;
                    if (index != 0)
                    {
                        exp += MultExp64Power10By16[index - 1];
                        val = Mul64Lossy(val, MultVal64Power10By16[index - 1], ref exp);
                    }
                }

                // round & scale down

                if ((val & (1 << 10)) != 0)
                {
                    // IEEE round to even
                    ulong tmp = val + ((1UL << 10) - 1 + ((val >> 11) & 1));
                    if (tmp < val)
                    {
                        // overflow
                        tmp = (tmp >> 1) | 0x8000000000000000;
                        exp++;
                    }
                    val = tmp;
                }

                // return the exponent to a biased state

                exp += 0x3FE;

                // handle overflow, underflow, "Epsilon - 1/2 Epsilon", denormalized, and the normal case

                if (exp <= 0)
                {
                    if (exp == -52 && (val >= 0x8000000000000058))
                    {
                        // round X where {Epsilon > X >= 2.470328229206232730000000E-324} up to Epsilon (instead of down to zero)
                        val = 0x0000000000000001;
                    }
                    else if (exp <= -52)
                    {
                        // underflow
                        val = 0;
                    }
                    else
                    {
                        // denormalized value
                        val >>= (-exp + 12);
                    }
                }
                else if (exp >= 0x7FF)
                {
                    // overflow
                    val = 0x7FF0000000000000;
                }
                else
                {
                    // normal positive exponent case
                    val = ((ulong)exp << 52) | ((val >> 11) & 0x000FFFFFFFFFFFFF);
                }

                // apply sign

                if (negative)
                {
                    val |= 0x8000000000000000;
                }

                return BitConverter.Int64BitsToDouble((long)val);
            }

            private static ulong Mul64Lossy(ulong a, ulong b, ref int exp)
            {
                ulong a_hi = (a >> 32);
                uint a_lo = (uint)a;
                ulong b_hi = (b >> 32);
                uint b_lo = (uint)b;

                ulong result = a_hi * b_hi;

                // save some multiplications if lo-parts aren't big enough to produce carry
                // (hi-parts will be always big enough, since a and b are normalized)

                if ((b_lo & 0xFFFF0000) != 0)
                {
                    result += (a_hi * b_lo) >> 32;
                }

                if ((a_lo & 0xFFFF0000) != 0)
                {
                    result += (a_lo * b_hi) >> 32;
                }

                // normalize
                if ((result & 0x8000000000000000) == 0)
                {
                    result <<= 1;
                    exp--;
                }

                return result;
            }
        }

        public static ParseResult DoubleTryParse(char[] chars, int start, int length, out double value)
        {
            value = 0;

            if (length == 0)
            {
                return ParseResult.Invalid;
            }

            bool isNegative = (chars[start] == '-');
            if (isNegative)
            {
                // text just a negative sign
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }

                start++;
                length--;
            }

            int i = start;
            int end = start + length;
            int numDecimalStart = end;
            int numDecimalEnd = end;
            int exponent = 0;
            ulong mantissa = 0UL;
            int mantissaDigits = 0;
            int exponentFromMantissa = 0;
            for (; i < end; i++)
            {
                char c = chars[i];
                switch (c)
                {
                    case '.':
                        if (i == start)
                        {
                            return ParseResult.Invalid;
                        }
                        if (i + 1 == end)
                        {
                            return ParseResult.Invalid;
                        }

                        if (numDecimalStart != end)
                        {
                            // multiple decimal points
                            return ParseResult.Invalid;
                        }

                        numDecimalStart = i + 1;
                        break;
                    case 'e':
                    case 'E':
                        if (i == start)
                        {
                            return ParseResult.Invalid;
                        }
                        if (i == numDecimalStart)
                        {
                            // E follows decimal point
                            return ParseResult.Invalid;
                        }
                        i++;
                        if (i == end)
                        {
                            return ParseResult.Invalid;
                        }

                        if (numDecimalStart < end)
                        {
                            numDecimalEnd = i - 1;
                        }

                        c = chars[i];
                        bool exponentNegative = false;
                        switch (c)
                        {
                            case '-':
                                exponentNegative = true;
                                i++;
                                break;
                            case '+':
                                i++;
                                break;
                        }

                        // parse 3 digit
                        for (; i < end; i++)
                        {
                            c = chars[i];
                            if (c < '0' || c > '9')
                            {
                                return ParseResult.Invalid;
                            }

                            int newExponent = (10 * exponent) + (c - '0');
                            // stops updating exponent when overflowing
                            if (exponent < newExponent)
                            {
                                exponent = newExponent;
                            }
                        }

                        if (exponentNegative)
                        {
                            exponent = -exponent;
                        }
                        break;
                    default:
                        if (c < '0' || c > '9')
                        {
                            return ParseResult.Invalid;
                        }

                        if (i == start && c == '0')
                        {
                            i++;
                            if (i != end)
                            {
                                c = chars[i];
                                if (c == '.')
                                {
                                    goto case '.';
                                }
                                if (c == 'e' || c == 'E')
                                {
                                    goto case 'E';
                                }

                                return ParseResult.Invalid;
                            }
                        }

                        if (mantissaDigits < 19)
                        {
                            mantissa = (10 * mantissa) + (ulong)(c - '0');
                            if (mantissa > 0)
                            {
                                ++mantissaDigits;
                            }
                        }
                        else
                        {
                            ++exponentFromMantissa;
                        }
                        break;
                }
            }

            exponent += exponentFromMantissa;

            // correct the decimal point
            exponent -= (numDecimalEnd - numDecimalStart);

            value = IEEE754.PackDouble(isNegative, mantissa, exponent);
            return double.IsInfinity(value) ? ParseResult.Overflow : ParseResult.Success;
        }
#endif

        public static ParseResult DecimalTryParse(char[] chars, int start, int length, out decimal value)
        {
            value = 0M;
            const decimal decimalMaxValueHi28 = 7922816251426433759354395033M;
            const ulong decimalMaxValueHi19 = 7922816251426433759UL;
            const ulong decimalMaxValueLo9 = 354395033UL;
            const char decimalMaxValueLo1 = '5';

            if (length == 0)
            {
                return ParseResult.Invalid;
            }

            bool isNegative = (chars[start] == '-');
            if (isNegative)
            {
                // text just a negative sign
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }

                start++;
                length--;
            }

            int i = start;
            int end = start + length;
            int numDecimalStart = end;
            int numDecimalEnd = end;
            int exponent = 0;
            ulong hi19 = 0UL;
            ulong lo10 = 0UL;
            int mantissaDigits = 0;
            int exponentFromMantissa = 0;
            char? digit29 = null;
            bool? storeOnly28Digits = null;
            for (; i < end; i++)
            {
                char c = chars[i];
                switch (c)
                {
                    case '.':
                        if (i == start)
                        {
                            return ParseResult.Invalid;
                        }
                        if (i + 1 == end)
                        {
                            return ParseResult.Invalid;
                        }

                        if (numDecimalStart != end)
                        {
                            // multiple decimal points
                            return ParseResult.Invalid;
                        }

                        numDecimalStart = i + 1;
                        break;
                    case 'e':
                    case 'E':
                        if (i == start)
                        {
                            return ParseResult.Invalid;
                        }
                        if (i == numDecimalStart)
                        {
                            // E follows decimal point		
                            return ParseResult.Invalid;
                        }
                        i++;
                        if (i == end)
                        {
                            return ParseResult.Invalid;
                        }

                        if (numDecimalStart < end)
                        {
                            numDecimalEnd = i - 1;
                        }

                        c = chars[i];
                        bool exponentNegative = false;
                        switch (c)
                        {
                            case '-':
                                exponentNegative = true;
                                i++;
                                break;
                            case '+':
                                i++;
                                break;
                        }

                        // parse 3 digit 
                        for (; i < end; i++)
                        {
                            c = chars[i];
                            if (c < '0' || c > '9')
                            {
                                return ParseResult.Invalid;
                            }

                            int newExponent = (10 * exponent) + (c - '0');
                            // stops updating exponent when overflowing
                            if (exponent < newExponent)
                            {
                                exponent = newExponent;
                            }
                        }

                        if (exponentNegative)
                        {
                            exponent = -exponent;
                        }
                        break;
                    default:
                        if (c < '0' || c > '9')
                        {
                            return ParseResult.Invalid;
                        }

                        if (i == start && c == '0')
                        {
                            i++;
                            if (i != end)
                            {
                                c = chars[i];
                                if (c == '.')
                                {
                                    goto case '.';
                                }
                                if (c == 'e' || c == 'E')
                                {
                                    goto case 'E';
                                }

                                return ParseResult.Invalid;
                            }
                        }

                        if (mantissaDigits < 29 && (mantissaDigits != 28 || !(storeOnly28Digits ?? (storeOnly28Digits = (hi19 > decimalMaxValueHi19 || (hi19 == decimalMaxValueHi19 && (lo10 > decimalMaxValueLo9 || (lo10 == decimalMaxValueLo9 && c > decimalMaxValueLo1))))).GetValueOrDefault())))
                        {
                            if (mantissaDigits < 19)
                            {
                                hi19 = (hi19 * 10UL) + (ulong)(c - '0');
                            }
                            else
                            {
                                lo10 = (lo10 * 10UL) + (ulong)(c - '0');
                            }
                            ++mantissaDigits;
                        }
                        else
                        {
                            if (!digit29.HasValue)
                            {
                                digit29 = c;
                            }
                            ++exponentFromMantissa;
                        }
                        break;
                }
            }

            exponent += exponentFromMantissa;

            // correct the decimal point
            exponent -= (numDecimalEnd - numDecimalStart);

            if (mantissaDigits <= 19)
            {
                value = hi19;
            }
            else
            {
                value = (hi19 / new decimal(1, 0, 0, false, (byte)(mantissaDigits - 19))) + lo10;
            }

            if (exponent > 0)
            {
                mantissaDigits += exponent;
                if (mantissaDigits > 29)
                {
                    return ParseResult.Overflow;
                }
                if (mantissaDigits == 29)
                {
                    if (exponent > 1)
                    {
                        value /= new decimal(1, 0, 0, false, (byte)(exponent - 1));
                        if (value > decimalMaxValueHi28)
                        {
                            return ParseResult.Overflow;
                        }
                    }
                    else if (value == decimalMaxValueHi28 && digit29 > decimalMaxValueLo1)
                    {
                        return ParseResult.Overflow;
                    }
                    value *= 10M;
                }
                else
                {
                    value /= new decimal(1, 0, 0, false, (byte)exponent);
                }
            }
            else
            {
                if (digit29 >= '5' && exponent >= -28)
                {
                    ++value;
                }
                if (exponent < 0)
                {
                    if (mantissaDigits + exponent + 28 <= 0)
                    {
                        value = isNegative ? -0M : 0M;
                        return ParseResult.Success;
                    }
                    if (exponent >= -28)
                    {
                        value *= new decimal(1, 0, 0, false, (byte)(-exponent));
                    }
                    else
                    {
                        value /= 1e28M;
                        value *= new decimal(1, 0, 0, false, (byte)(-exponent - 28));
                    }
                }
            }

            if (isNegative)
            {
                value = -value;
            }

            return ParseResult.Success;
        }

        public static bool TryConvertGuid(string s, out Guid g)
        {
            return Guid.TryParseExact(s, "D", out g);
        }

        public static bool TryHexTextToInt(char[] text, int start, int end, out int value)
        {
            value = 0;

            for (int i = start; i < end; i++)
            {
                char ch = text[i];
                int chValue;

                if (ch <= 57 && ch >= 48)
                {
                    chValue = ch - 48;
                }
                else if (ch <= 70 && ch >= 65)
                {
                    chValue = ch - 55;
                }
                else if (ch <= 102 && ch >= 97)
                {
                    chValue = ch - 87;
                }
                else
                {
                    value = 0;
                    return false;
                }

                value += chValue << ((end - 1 - i) * 4);
            }

            return true;
        }
    }
}