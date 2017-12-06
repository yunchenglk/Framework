﻿using System;

namespace YunChengLK.Framework.Utility
{
    public class PropertyAccessorKey
    {
        public Type TargetType { get; private set; }
        public string PropertyName { get; private set; }
        public PropertyAccessorKey(Type targetType, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(targetType, "targetType");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            this.TargetType = targetType;
            this.PropertyName = propertyName;
        }
        public override int GetHashCode()
        {
            return this.TargetType.GetHashCode() ^ this.PropertyName.GetHashCode();
        }
    }
}
