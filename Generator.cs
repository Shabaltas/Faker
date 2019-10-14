namespace Faker
{
    public class Generator : System.Attribute
    {
        public string TypeName {  get; }

        public Generator(string name)
        {
            TypeName = name;
        }
    } 
}