using System;
using System.Collections.Generic;

namespace Faker
{
    public class ListGenerator<T>: EnumerableGenerator<List<T>>
    {
        public override List<T> Generate()
        {
            int size = new Random().Next(0, 21);
            List<T> list = new List<T>();
            for (int i = 1; i < size; i++)
            {
                list.Add(Faker.Create<T>());
            }
            return list;
        }
    }
}