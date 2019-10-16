namespace Faker
{
    public class DoubleGenerator: NumericGenerator<double>
    {
        public override double Generate()
        {
            return Random.NextDouble();
        }
    }
}