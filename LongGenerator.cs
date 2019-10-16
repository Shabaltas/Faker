using System;

namespace Faker
{
    public class LongGenerator: NumericGenerator<long>
    {
        public override long Generate()
        {
            byte[] buf = new byte[8];
            Random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (unchecked(long.MaxValue - long.MinValue))) + long.MinValue);
        }
    }
}