using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Faker
{
    public class FakerConfig
    {
        internal Dictionary<MemberInfo, Type> Generators { get; } = new Dictionary<MemberInfo, Type>();
        public void Add<TClass, TMember, TGenerator>(Expression<Func<TClass, TMember>> expressionTree)
            where TGenerator: IGenerator<TMember>
        {
            MemberExpression body = (MemberExpression)expressionTree.Body;
            MemberInfo memberInfo = body.Member;
            Generators.Add(memberInfo, typeof(TGenerator));
        }
    }
}