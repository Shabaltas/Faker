using System;

namespace Faker
{
    //[Generator("int")]
    public class IntGenerator: NumericGenerator<int>
    {
        public override int Generate()
        {
            return Random.Next();
        }
    }
}