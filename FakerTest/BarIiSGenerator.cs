using Faker;

namespace FakerTest
{
    public class BarIisGenerator: NumericGenerator<int>
    {
        public override int Generate()
        {
            return 123;
        }
    }
}