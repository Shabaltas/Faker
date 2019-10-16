namespace Faker
{
    public class FloatGenerator: NumericGenerator<float>
    {
        public override float Generate()
        {
            int sign = Random.Next(2);
            int exponent = Random.Next((1 << 8) - 1); // do not generate 0xFF (infinities and NaN)
            int mantissa = Random.Next(1 << 23);
            var bits = (sign << 31) + (exponent << 23) + mantissa;
            return bits;
        }
    }
}