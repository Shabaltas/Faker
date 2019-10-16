using System;

namespace Faker
{
    public class BoolGenerator: NumericGenerator<bool>
    {
        public override bool Generate()
        {
            return Random.Next(0, 2) == 1;
        }
    }
}