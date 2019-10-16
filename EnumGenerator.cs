using System;

namespace Faker
{
    public class EnumGenerator<T>: IGenerator<Enum> where T: Enum
    {
        public Enum Generate()
        {
            Array enumNames = typeof(T).GetEnumValues(); 
            return (T) enumNames.GetValue(new Random().Next(0, enumNames.Length));
        }
    }
}