namespace Faker
{
    public class ShortGenerator: NumericGenerator<short>
    {
        public override short Generate()
        {
            return  (short) Random.Next(0, 256);
        }
    }
}