using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectMapper
{
    public class Mapper<TSource, TDestination> where TDestination : new()
    {
        private void MapMatchingProperties(TSource source, TDestination destination)
        {
            foreach (var destinationProperty in typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
            {
                var sourceProp = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                .Where(p => p.Name == destinationProperty.Name && p.PropertyType == destinationProperty.PropertyType)
                                                .FirstOrDefault();
                if (sourceProp != null)
                {
                    var getter = GetGetter(sourceProp);
                    var value = getter(source);
                    var setter = GetSetter(destinationProperty);
                    setter.DynamicInvoke(destination, value);
                }
            }
        }

        private Func<TSource, object> GetGetter(PropertyInfo propertyInfo)
        {
            var getterMethodInfo = propertyInfo.GetGetMethod();
            var entity = Expression.Parameter(typeof(TSource));
            var getterCall = Expression.Call(entity, getterMethodInfo);

            var castToObject = Expression.Convert(getterCall, typeof(object));
            var lambda = Expression.Lambda(castToObject, entity);

            var getter = (Func<TSource, object>)lambda.Compile();
            return getter;
        }

        private Delegate GetSetter(PropertyInfo propertyInfo)
        {
            var objectParameter = Expression.Parameter(typeof(TDestination));
            var valueParameter = Expression.Parameter(propertyInfo.PropertyType);
            var assignment = Expression.Assign(Expression.MakeMemberAccess(objectParameter, propertyInfo), valueParameter);
            var assign = Expression.Lambda(assignment, objectParameter, valueParameter);
            Delegate setter = assign.Compile();
            return setter;
        }

        private TDestination MapObject(TSource source, TDestination destination)
        {
            MapMatchingProperties(source, destination);
            return destination;
        }

        public TDestination CreateMappedObject(TSource source)
        {
            TDestination destination = new TDestination();
            return MapObject(source, destination);
        }
    }
}
