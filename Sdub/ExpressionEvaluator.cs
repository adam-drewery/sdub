using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Sdub;

internal class ExpressionEvaluator
{
    public static object Evaluate(Expression expression)
    {
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        if (expression.NodeType == ExpressionType.Constant)
        {
            return ((ConstantExpression)expression).Value;
        }
        else if (expression.NodeType == ExpressionType.MemberAccess)
        {
            var memberExpression = (MemberExpression)expression;
            var memberValue = Evaluate(memberExpression.Expression);
            if (memberValue == null)
                throw new InvalidOperationException("Cannot access member of null object.");
            
            if (memberExpression.Member is System.Reflection.PropertyInfo propertyInfo)
            {
                return propertyInfo.GetValue(memberValue);
            }
            else if (memberExpression.Member is System.Reflection.FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(memberValue);
            }
        }
        else if (expression.NodeType == ExpressionType.Call)
        {
            var methodCallExpression = (MethodCallExpression)expression;
            var method = methodCallExpression.Method;
            var target = Evaluate(methodCallExpression.Object);

            return method.Invoke(target, EvaluateArguments(methodCallExpression.Arguments));
        }

        throw new ArgumentException("Unsupported expression type.");
    }

    private static object[] EvaluateArguments(ReadOnlyCollection<Expression> arguments)
    {
        if (arguments == null)
            return null;

        var evaluatedArguments = new object[arguments.Count];
        for (int i = 0; i < arguments.Count; i++)
        {
            evaluatedArguments[i] = Evaluate(arguments[i]);
        }

        return evaluatedArguments;
    }
}