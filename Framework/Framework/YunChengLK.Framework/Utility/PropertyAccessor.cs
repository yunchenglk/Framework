using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;
using System.Linq.Expressions;

namespace YunChengLK.Framework.Utility
{
    /// <summary>
    /// The PropertyAccessor is used to set or get the property value.
    /// </summary>
    /// <typeparam name="T">The type of target instance.</typeparam>
    /// <remarks>The underlying mechanism is based on IL Emit, which provides better performance than pure reflection.</remarks>
    public class PropertyAccessor
    {
        #region Private Static Fields
        private static Dictionary<PropertyAccessorKey, PropertyAccessor> propertyAccessors = new Dictionary<PropertyAccessorKey, PropertyAccessor>();
        private static object synchHelper = new object();
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets or sets the delegate to get property value.
        /// </summary>
        /// <value>The delegate to get property value.</value>
        public Func<object, object> GetFunction { get; private set; }

        /// <summary>
        /// Gets or sets the delegate to set property value.
        /// </summary>
        /// <value>The delegate to set property value.</value>
        public Action<object, object> SetAction { get; private set; }

        #endregion

        #region Constructorsrs
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessor&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyAccessor(Type targetType, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(targetType, "targetType");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyInfo property = targetType.GetProperty(propertyName);
            if (null == property)
            {
                throw new ArgumentException(
                    string.Format("Cannot find the property \"{0\" from the type \"{1}\".}", property, targetType.FullName));
            }
            this.Property = property;
            this.TargetType = targetType;
            this.PropertyName = propertyName;
            this.PropertyType = property.PropertyType;
            if (this.Property.CanRead)
            {
                this.GetFunction = CreateGetFunction();
            }
            if (this.Property.CanWrite)
            {
                this.SetAction = CreateSetAction();
            }
        }
        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Gets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <returns>The property value of the given object.</returns>
        public object Get(object obj)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            EnsureValidType(obj, "obj");
            if (null == this.GetFunction)
            { 
                throw new InvalidOperationException(
                    string.Format("The property \"{0}\" of type \"{1}\" is not readable.", this.PropertyName,this.TargetType.FullName));
            }
            return this.GetFunction(obj);
        }

        /// <summary>
        /// Sets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="value">The property value.</param>
        public void Set(object obj, object value)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            EnsureValidType(obj, "obj");
            if (null == this.SetAction)
            {
                throw new InvalidOperationException(
                    string.Format("The property \"{0}\" of type \"{1}\" is not writable.", this.PropertyName, this.TargetType.FullName));
            }
            this.SetAction(obj, value);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Gets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value of the given object.</returns>
        public static object Get(object obj, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessor propertyAccessor;
            PropertyAccessorKey key = new PropertyAccessorKey(obj.GetType(), propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                propertyAccessor = propertyAccessors[key];
                return propertyAccessor.Get(obj);
            }
            lock (synchHelper)
            {
                if (propertyAccessors.ContainsKey(key))
                {
                    propertyAccessor = propertyAccessors[key];
                    return propertyAccessor.Get(obj);
                }
                propertyAccessor = new PropertyAccessor(obj.GetType(), propertyName);
                propertyAccessors[key] = propertyAccessor;
            }
            return propertyAccessor.Get(obj);
        }

        /// <summary>
        /// Sets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The property value.</param>
        public static void Set(object obj, string propertyName, object value)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessor propertyAccessor;
            PropertyAccessorKey key = new PropertyAccessorKey(obj.GetType(), propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                propertyAccessor = propertyAccessors[key];
                propertyAccessor.Set(obj, value);
                return;
            }
            lock (synchHelper)
            {
                if (propertyAccessors.ContainsKey(key))
                {
                    propertyAccessor = propertyAccessors[key];
                    propertyAccessor.Set(obj, value);
                    return;
                }
                propertyAccessor = new PropertyAccessor(obj.GetType(), propertyName);
                propertyAccessors[key] = propertyAccessor;
            }
            propertyAccessor.Set(obj, value);
        }


        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Type GetPropertyType(Type targetType, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(targetType, "targetType");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessorKey key = new PropertyAccessorKey(targetType, propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                return propertyAccessors[key].PropertyType;
            }
            lock (synchHelper)
            {
                if (propertyAccessors.ContainsKey(key))
                {
                    return propertyAccessors[key].PropertyType;
                }
                var propertyAccessor = new PropertyAccessor(targetType, propertyName);
                propertyAccessors[key] = propertyAccessor;
                return propertyAccessor.PropertyType;
            }
        }

        #endregion

        #region Private Methods

        private void EnsureValidType(object value, string parameterName)
        {
            if (!this.TargetType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException("The target type cannot be assignable from the type of given object.", parameterName);
            }
        }

        private Func<object, object> CreateGetFunction()
        {
            var getMethod = this.Property.GetGetMethod();
            var target = Expression.Parameter(typeof(object), "target");
            var castedTarget = getMethod.IsStatic ? null : Expression.Convert(target, this.TargetType);
            var getProperty = Expression.Property(castedTarget, this.Property);
            var castPropertyValue = Expression.Convert(getProperty, typeof(object));
            return Expression.Lambda<Func<object, object>>(castPropertyValue, target).Compile();
        }

        private Action<object, object> CreateSetAction()
        {
            var setMethod = this.Property.GetSetMethod();
            var target = Expression.Parameter(typeof(object), "target");
            var propertyValue = Expression.Parameter(typeof(object), "value");
            var castedTarget = setMethod.IsStatic ? null : Expression.Convert(target, this.TargetType);
            var castedpropertyValue = Expression.Convert(propertyValue, this.PropertyType);
            var propertySet = Expression.Call(castedTarget, setMethod, castedpropertyValue);
            return Expression.Lambda<Action<object, object>>(propertySet, target, propertyValue).Compile();
        }
        #endregion
    }
}
