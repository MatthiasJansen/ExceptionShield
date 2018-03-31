using System;
using JetBrains.Annotations;

namespace ExceptionShield.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSubclassOfOpenGeneric(this Type current, [NotNull] Type baseType)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }

            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            while (current != typeof(object))
            {
                var cur = current.IsGenericType ? current.GetGenericTypeDefinition() : current;
                if (baseType == cur)
                {
                    return true;
                }

                current = current.BaseType;
            }

            return false;
        }
    }
}