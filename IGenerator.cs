namespace Faker
{
    public interface IGenerator<T>
    {
        T Generate();
    }
}