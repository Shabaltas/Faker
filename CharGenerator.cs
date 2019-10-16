namespace Faker
{
    public class CharGenerator: NumericGenerator<char>
    {
        public override char Generate()
        {
            return (char) Random.Next(0, 256);
        }
    }
}