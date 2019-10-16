using System;
using System.Text;

namespace Faker
{
    //[Generator("string")]
    public class StringGenerator: IGenerator<string>
    {
        public string Generate()
        {
            Random random = new Random();
            IGenerator<char> charGenerator = new CharGenerator();
            int length = random.Next(50);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(charGenerator.Generate());
            }

            return stringBuilder.ToString();
        }
    }
}