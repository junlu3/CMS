using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Data
{
    public static class OrderByT
    {
        public static IList<T> IListOrderBy<T>(IList<T> list, string propertyName) where T : new()
        {
            if (list == null || list.Count == 0)
            {
                return list;
            }

            Type elementType = list[0].GetType();
            PropertyInfo propertyInfo = elementType.GetProperty(propertyName);
            ParameterExpression parameter = Expression.Parameter(elementType,"");
            Expression body = Expression.Property(parameter,propertyInfo);

            Expression sourceExpression = list.AsQueryable().Expression;

            Type sourcePropertyType = propertyInfo.PropertyType;

            Expression lambda = Expression.Call(typeof(Queryable),"OrderBy",new Type[] { elementType,sourcePropertyType },sourceExpression,Expression.Lambda(body,parameter));

            return list.AsQueryable().Provider.CreateQuery<T>(lambda).ToList<T>();
        }

        public static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }
    }
}
