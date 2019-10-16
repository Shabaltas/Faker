using System;

namespace Faker
{
    public abstract class NumericGenerator<T>: IGenerator<T>
    {
        protected static Random Random = new Random();
        public abstract T Generate();
    }
}