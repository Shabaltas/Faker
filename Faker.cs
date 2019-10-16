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
                Generators.Add(MakeTypeFullName(key), type);
            }
        }
        public T Create<T>()
        {
            Type type = typeof(T);
            if (type.IsInterface || type.IsAbstract || type == typeof(void))
            {
                //return null;
                throw new ArgumentException("Type must not be abstract, interface or void");
            }

            if (type.IsGenericType && type.GenericTypeArguments.Length == 0)
            {
                //return null
                throw new ArgumentException("Generic type is undefined");
            }
            if (!_currentDependencies.Add(type))
            {
                return default;
            }
            T currentObject;
            if (Generators.TryGetValue(MakeTypeFullName(type), out Type generatorType))
            {
                currentObject = CreateWithGenerator<T>(generatorType);
            }
            else if (type.IsEnum)
            { 
                currentObject = CreateEnum<T>();
            }
            else if (type.IsPrimitive) 
            { 
                currentObject = (T) Activator.CreateInstance(type);
            }
            else if (type.IsValueType) 
            { 
                currentObject = CreateStruct<T>();
            }
            else 
            { 
                currentObject = CreateObject<T>();
            }
            _currentDependencies.Remove(type);
            return currentObject;
        }

        private void FillObject(object o)
        {
            Type type = o.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic
                                                                      | BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                field.SetValue(o, Config.Generators.ContainsKey(field)
                    ? GetType().GetMethod("CreateWithGenerator", BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(field.FieldType).Invoke(this, new object [] {Config.Generators[field]})
                    : GetType().GetMethod("Create")
                        .MakeGenericMethod(field.FieldType).Invoke(this, new object [0]));
            }
        }
        
        private T CreateEnum<T>()
        {
            Type type = typeof(T);
            Type generatorType = Generators[MakeTypeFullName(typeof(Enum))].MakeGenericType(type);
            object generator = Activator.CreateInstance(generatorType);
            return (T) generatorType.GetMethod("Generate").Invoke(generator, new object[0]);
        }

        private T CreateStruct<T>()
        {
            Type type = typeof(T);
            T o = (T) Activator.CreateInstance(type);
            FieldInfo[] fields = type.GetFields();
            foreach (var field in fields)
            {
                field.SetValue(o, GetType().GetMethod("Create").MakeGenericMethod(type).Invoke(this, new object [0]));
            }
            return o;
        }

        private T CreateObject<T>()
        {
            Type type = typeof(T);
            T currentObject;
            ConstructorInfo constructorWithMaxParams = GetConstructorWithMaxParams(type);
            if (constructorWithMaxParams != null)
            {
                currentObject = CreateAndFillObjectWithConstructor<T>(constructorWithMaxParams);
            }
            else
            {
                currentObject = CreateAndSetValues<T>();
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

        private T CreateAndFillObjectWithConstructor<T>(ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            int length = parameters.Length;
            object[] parametersValues = new object[length];
            for (int i = 0; i < length; i++)
            {
                parametersValues[i] = GetType().GetMethod("Create").MakeGenericMethod(parameters[i].ParameterType).Invoke(this, new object [0]);;
            }
            T currentObject = (T) constructor.Invoke(parametersValues);
            FillObject(currentObject);
            return currentObject;
        }

        private T CreateAndSetValues<T>()
        {
            Type type = typeof(T);
            T currentObject = (T) Activator.CreateInstance(type, true);
            FillObject(currentObject);
            return currentObject;
        }

        private T CreateWithGenerator<T>(Type generatorType)
        {
            Type type = typeof(T);
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
            return (T) generatorType.GetMethod("Generate").Invoke(generator, new object[0]);
        }
        
        private bool IsTheSameGenericClass(Type type1, Type type2)
        {
            return type1.Assembly.FullName.Equals(type2.Assembly.FullName)
                   && type1.Name.Equals(type2.Name);
        }

        private string MakeTypeFullName(Type type)
        {
            return type.Assembly + "." + type.Name;
        }
    }
}