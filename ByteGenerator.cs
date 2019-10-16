namespace Faker
{
    public class ByteGenerator: NumericGenerator<byte>
    {
        public override byte Generate()
        {
            return (byte) Random.Next(0, 256);
        }
    }
}