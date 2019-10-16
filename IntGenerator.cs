namespace Faker
{
    public class IntGenerator: NumericGenerator<int>
    {
        public override int Generate()
        {
            return Random.Next();
        }
    }
}