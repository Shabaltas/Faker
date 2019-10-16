namespace Faker
{
    public abstract class EnumerableGenerator<T>: IGenerator<T>
    {
        internal Faker Faker { get; set; }
        public abstract T Generate();
    }
}