using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Task1
{
  [TestClass]
  public class Task2Test
  {
    public class Mapper<TSource, TDestination>
    {
      readonly Func<TSource, TDestination> _mapFunction;

      internal Mapper(Func<TSource, TDestination> func)
      {
        _mapFunction = func;
      }

      public TDestination Map(TSource source)
      {
        return _mapFunction(source);
      }
    }

    public class MappingGenerator
    {
      public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
      {
        var sourceFields = typeof(TSource).GetProperties().Select(item => item.Name);
        var destinationFields = typeof(TDestination).GetProperties().Select(item => item.Name);

        var fieldsToMap = destinationFields.Intersect(sourceFields);

        var sourceParam = Expression.Parameter(typeof(TSource));

        var bindingArray = fieldsToMap
          .Select(fieldName => Expression.Bind(
              typeof(TDestination).GetMember(fieldName)[0],
              Expression.Property(sourceParam, fieldName)));

        var mapFunction =
          Expression.Lambda<Func<TSource, TDestination>>(
            Expression.MemberInit(Expression.New(typeof(TDestination)), bindingArray),
            sourceParam
          );

        return new Mapper<TSource, TDestination>(mapFunction.Compile());
      }
    }

    public class Foo
    {
      public int A { get; set; }

      public int B { get; set; }

      public string C { get; set; }
    }

    public class Bar
    {
      public int A { get; set; }

      public int B { get; set; }

      public string C { get; set; }
    }

    [TestMethod]
    public void TestMethod3()
    {
      var mapGenerator = new MappingGenerator();
      var mapper = mapGenerator.Generate<Foo, Bar>();

      var res = mapper.Map(new Foo
      {
        A = 1,
        B = 2,
        C = "qwerty"
      });

      Console.WriteLine($"{res.A} {res.B} {res.C}");
    }
  }
}