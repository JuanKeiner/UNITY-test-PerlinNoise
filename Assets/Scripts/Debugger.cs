using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    //[Header("Buen script este")]
    //[Space]

    public static void DebugVariable<T>(Expression<Func<T>> variableExpression) {
        string variableName = GetMemberName(variableExpression);
        string variableValue = variableExpression.Compile()().ToString();
        Debug.Log((variableName != "" ? (variableName + ": ") : "") + variableValue);
    }
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) {
        var expression = (memberExpression.Body is UnaryExpression unaryExpression)
            ? unaryExpression.Operand
            : memberExpression.Body;

        var memberNameVisitor = new MemberNameVisitor();
        memberNameVisitor.Visit(expression);

        return memberNameVisitor.MemberName;
    }

    private class MemberNameVisitor : ExpressionVisitor {
        public string MemberName { get; private set; }

        protected override Expression VisitMember(MemberExpression node) {
            MemberName = node.Member.Name;
            return node;
        }
    }
}
