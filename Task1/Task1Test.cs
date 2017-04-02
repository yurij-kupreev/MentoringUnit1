using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Task1
{
  [TestClass]
  public class Task1Test
  {
    public class CustomTransform : ExpressionVisitor
    {
      protected override Expression VisitBinary(BinaryExpression node)
      {
        if (node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract)
        {
          ParameterExpression param = null;
          ConstantExpression constant = null;
          switch (node.Left.NodeType)
          {
            case ExpressionType.Parameter:
              param = (ParameterExpression)node.Left;
              break;
            case ExpressionType.Constant:
              constant = (ConstantExpression)node.Left;
              break;
          }

          switch (node.Right.NodeType)
          {
            case ExpressionType.Parameter:
              param = (ParameterExpression)node.Right;
              break;
            case ExpressionType.Constant:
              constant = (ConstantExpression)node.Right;
              break;
          }

          if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
          {
            return node.NodeType == ExpressionType.Add ? Expression.Increment(param) : Expression.Decrement(param);
          }

        }

        return base.VisitBinary(node);
      }
    }


    [TestMethod]
    public void AddToIncrementTransformTest()
    {
      Expression<Func<int, int>> source_exp = (a) => a + (a + 1) * (a + 5) * (a - 1);
      var result_exp = (new CustomTransform().VisitAndConvert(source_exp, ""));


      Console.WriteLine(source_exp + " " + source_exp.Compile().Invoke(3));
      Console.WriteLine(result_exp + " " + result_exp.Compile().Invoke(3));

    }
  }
}
