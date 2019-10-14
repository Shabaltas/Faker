using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Faker
{
    public class Faker
    {
        public Dictionary<String, Type> Generators { get; }
        private ISet<Type> _currentDependencies = new HashSet<Type>();
        public Faker()
        {
            Generators = new Dictionary<string, Type>();
            Assembly generatorsAssembly = Assembly.GetAssembly(typeof(Generator));
            Type[] assemblyTypes = generatorsAssembly.GetTypes();
            foreach (var type in assemblyTypes)
            {
                /*with attriibute
                foreach (var attribute in type.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(Generator))
                    {
                        Generators.Add(((Generator)attribute).TypeName, type);
                    }
                }
                foreach (var attribute in type.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(Generator))
                    {
                        Generators.Add(((Generator)attribute).TypeName, type);
                    }
                }*/
                
                //with interface
                foreach (var iInterface in type.GetInterfaces())
                {
                    if (iInterface == typeof(IGenerator<>))
                    {
                        Generators.Add(iInterface.GetGenericArguments()[0].Name, type);
                    }
                }
            }
        }
        public object Create<T>()
        {
            Type type = typeof(T);
            if (type.IsInterface || type.IsAbstract || type == typeof(void) || !_currentDependencies.Add(type))
            {
                return null;
            }
            T currentObject = (T) Activator.CreateInstance(type);
            Type generatorType = Generators[type.Name.ToLower()];
            if (generatorType != null)
            {
                currentObject = ((IGenerator<T>) Activator.CreateInstance(generatorType)).Generate();
            }
            else
            {
                
            }
            return currentObject;
        }
    }
}