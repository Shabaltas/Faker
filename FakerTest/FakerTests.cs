using System;
using System.Collections.Generic;
using Faker;
using NUnit.Framework;

namespace FakerTest
{
  [TestFixture]
    public class FakerTests
    {
        private readonly Faker.Faker faker = new Faker.Faker();
        [Test]
        public void ListTest()
        {
            List<int> list = faker.Create<List<int>>();
            Assert.IsNotNull(list);
            Assert.AreNotEqual(list, default);
        }

        [Test]
        public void Deadlock()
        {
            Assert.Throws<ArgumentException>(() => faker.Create<Foo>());
        }

        [Test]
        public void ClassTest()
        {
            Bar myObject = faker.Create<Bar>();
            Assert.IsNotNull(myObject);
            Assert.AreNotEqual(myObject, default);
        }
        
        [Test]
        public void Generic()
        {
            MyGeneric<int> myObject = faker.Create<MyGeneric<int>>();
            Assert.IsNotNull(myObject);
            Assert.AreNotEqual(myObject, default);
            Assert.AreNotEqual(myObject._T, default);
        }
        
        /*[Test]
        public void FailGeneric()
        {
            Assert.Throws<ArgumentException>(() => faker.Create<MyGeneric>());
        }*/

        [Test]
        public void DefaultInt()
        {
            var int32Value = faker.Create<int>();
            Assert.AreNotEqual(int32Value, default);
        }

        /*[Test]
        public void DefaultBool()
        {
            var boolValue = _faker.Create(typeof(bool));
            Assert.AreNotEqual(boolValue, default);
        }*/

        [Test]
        public void DefaultDouble()
        {
            var doubleValue = faker.Create<double>();
            Assert.AreNotEqual(doubleValue, default);
        }

        [Test]
        public void DefaultEnum()
        {
            var enumValue = faker.Create<MyEnum>();
            Assert.AreNotEqual(enumValue, default);
        }

        [Test]
        public void DefaultLong()
        {
            var longValue = faker.Create<long>();
            Assert.AreNotEqual(longValue, default);
        }

        [Test]
        public void DefaultString()
        {
            var stringValue = faker.Create<string>();
            Assert.AreNotEqual(stringValue, default);
        }

        [Test]
        public void Config()
        {
            FakerConfig fakerConfig = new FakerConfig();
            fakerConfig.Add<Bar, int, BarIisGenerator>(Bar => Bar.isS);
            Faker.Faker configFaker = new Faker.Faker(fakerConfig);
            Bar bar = (Bar)configFaker.Create<Bar>();
            Assert.AreEqual(bar.isS, 123);
        }

        private class NoConstructorsCls
        {
            private NoConstructorsCls()
            {

            }
        }
            

        [Test]
        public void NoConstructors()
        {
            NoConstructorsCls result = (NoConstructorsCls)faker.Create<NoConstructorsCls>();
            Assert.NotNull(result);
            Assert.AreNotEqual(result, default);
        }
    }
}