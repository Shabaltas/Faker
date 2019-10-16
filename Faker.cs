﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Faker
{
    public class Faker
    {
        public Dictionary<String, Type> Generators { get; } = new Dictionary<string, Type>();
        private FakerConfig Config { get; set; }
        private readonly ISet<Type> _currentDependencies = new HashSet<Type>();

        public Faker(FakerConfig fakerConfig)
        {
            Config = fakerConfig;
            LoadGeneratorsClasses();
        }
        public Faker()
        {
            Config = new FakerConfig();
            LoadGeneratorsClasses();
        }

        private void LoadGeneratorsClasses()
        {
            Assembly generatorsAssembly = Assembly.GetAssembly(typeof(Generator));
            Type[] assemblyTypes = generatorsAssembly.GetTypes();
            foreach (var type in assemblyTypes)
            {
                Type iInterface = type.GetInterface(typeof(IGenerator<>).Name);
                if (type.IsAbstract || iInterface == null) continue;
                Type key = iInterface.GetGenericArguments()[0];
                Generators.Add(makeTypeFullName(key), type);
            }
        }
        public object Create(Type type)
        { 
            if (type.IsInterface || type.IsAbstract || type == typeof(void))
            {
                //return null;
                throw new ArgumentException("Type must not be abstract, interface or void");
            }

            if (!_currentDependencies.Add(type))
            {
                //return null;
                throw new ArgumentException("Deadlock has occured");
            }
            object currentObject;
            try
            {
                currentObject = CreateWithGenerator(type, Generators[makeTypeFullName(type)]);
            }
            catch (KeyNotFoundException)
            {
                if (type.IsEnum)
                {
                    currentObject = CreateEnum(type);
                }
                else if (type.IsPrimitive)
                {
                    currentObject = Activator.CreateInstance(type);
                }
                else if (type.IsValueType)
                {
                    currentObject = CreateStruct(type);
                }
                else
                {
                    currentObject = CreateObject(type);
                }
            }

            _currentDependencies.Remove(type);
            return currentObject;
        }

        private void FillObject(object o)
        {
            Type type = o.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                field.SetValue(o, Config.Generators.ContainsKey(field)
                    ? CreateWithGenerator(field.FieldType, Config.Generators[field]) 
                    : Create(field.FieldType));
            }
        }

        private object CreateEnum(Type type)
        {
            Type generatorType = Generators[makeTypeFullName(typeof(Enum))].MakeGenericType(type);
            object generator = Activator.CreateInstance(generatorType);
            return generatorType.GetMethod("Generate").Invoke(generator, new object[0]);
        }

        private object CreateStruct(Type type)
        {
            object o = Activator.CreateInstance(type);
            FieldInfo[] fields = type.GetFields();
            foreach (var field in fields)
            {
                field.SetValue(o, Create(field.FieldType));
            }
            return o;
        }

        private object CreateObject(Type type)
        {
            object currentObject;
            //TODO do for the genericks, because GetConstructors() doesn't return genericks constructors
            ConstructorInfo constructorWithMaxParams = GetConstructorWithMaxParams(type);
            if (constructorWithMaxParams != null)
            {
                currentObject = CreateAndFillObjectWithConstructor(constructorWithMaxParams);
            }
            else
            {
                currentObject = CreateAndSetValues(type);
            }
            return currentObject;
        }

        private ConstructorInfo GetConstructorWithMaxParams(Type type)
        {
            ConstructorInfo constructorWithMaxParams = null;
            int i = 0;
            foreach (var constructor in type.GetConstructors())
            {
                if (i < constructor.GetParameters().Length)
                {
                    constructorWithMaxParams = constructor;
                }
            }

            return constructorWithMaxParams;
        }

        private object CreateAndFillObjectWithConstructor(ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            int length = parameters.Length;
            object[] parametersValues = new object[length];
            for (int i = 0; i < length; i++)
            {
                parametersValues[i] = Create(parameters[i].ParameterType);
            }
            object currentObject = constructor.Invoke(parametersValues);
            if (length == 0) FillObject(currentObject);
            return currentObject;
        }

        private object CreateAndSetValues(Type type)
        {
            object currentObject = Activator.CreateInstance(type, true);
            FillObject(currentObject);
            return currentObject;
        }

        private object CreateWithGenerator(Type type, Type generatorType)
        {
            object generator;
            if (generatorType.BaseType != null && IsTheSameGenericClass(generatorType.BaseType, typeof(EnumerableGenerator<>)))
            {
                generatorType = generatorType.MakeGenericType(type.GenericTypeArguments[0]);
                generator = Activator.CreateInstance(generatorType);
                generatorType.BaseType.GetProperty("Faker", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(generator, this);
                
            }
            else
            {
                generator = Activator.CreateInstance(generatorType);
            }
            return generatorType.GetMethod("Generate").Invoke(generator, new object[0]);
        }
        
        private bool IsTheSameGenericClass(Type type1, Type type2)
        {
            return type1.Assembly.FullName.Equals(type2.Assembly.FullName)
                   && type1.Name.Equals(type2.Name);
        }

        private string makeTypeFullName(Type type)
        {
            return type.Assembly + "." + type.Name;
        }
    }
}