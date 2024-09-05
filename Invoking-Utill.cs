using System;
using System.Reflection;

public static class ReflectionExtensions
{
    public static ReflectionUtil<T> Sys<T>(this T obj)
    {
        return new ReflectionUtil<T>(obj);
    }
}

public class ReflectionUtil<T>
{
    private readonly T targetObject;

    public ReflectionUtil(T targetObject)
    {
        this.targetObject = targetObject;
    }

    public void InvokeMethod(string methodName, params object[] args)
    {
        Type targetType = typeof(T);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        MethodInfo method = targetType.GetMethod(methodName, bindingFlags);

        if (method == null)
        {
            throw new ArgumentException($"Method '{methodName}' not found in type '{targetType}'.");
        }

        method.Invoke(targetObject, args);
    }

    public R GetValue<R>(string fieldName, bool isStatic = false, bool isProperty = false)
    {
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                     (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                                     (isProperty ? BindingFlags.GetProperty : BindingFlags.GetField);

        if (isProperty)
        {
            return FetchProperty<R>(fieldName, bindingFlags);
        }
        else
        {
            return FetchField<R>(fieldName, bindingFlags);
        }
    }

    public void SetValue(string fieldName, object value, bool isStatic = false, bool isProperty = false)
    {
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                     (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                                     (isProperty ? BindingFlags.SetProperty : BindingFlags.SetField);

        if (isProperty)
        {
            SetProperty(fieldName, value, bindingFlags);
        }
        else
        {
            SetField(fieldName, value, bindingFlags);
        }
    }

    public R Invoke<R>(string methodName, bool isStatic = false, params object[] args)
    {
        BindingFlags bindingFlags = isStatic
            ? BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod
            : BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod;

        MethodInfo method = typeof(T).GetMethod(methodName, bindingFlags);

        if (method == null)
        {
            throw new ArgumentException($"Method '{methodName}' not found in type '{typeof(T)}'.");
        }

        return (R)method.Invoke(targetObject, args);
    }

    public object GetValue(string fieldName, bool isStatic = false, bool isProperty = false)
    {
        return GetValue<object>(fieldName, isStatic, isProperty);
    }

    private R FetchField<R>(string fieldName, BindingFlags bindingFlags)
    {
        FieldInfo field = typeof(T).GetField(fieldName, bindingFlags);

        if (field == null)
        {
            throw new ArgumentException($"Field '{fieldName}' not found.");
        }

        return (R)field.GetValue(targetObject);
    }

    private R FetchProperty<R>(string propertyName, BindingFlags bindingFlags)
    {
        PropertyInfo property = typeof(T).GetProperty(propertyName, bindingFlags);

        if (property == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found.");
        }

        return (R)property.GetValue(targetObject);
    }

    private void SetField(string fieldName, object value, BindingFlags bindingFlags)
    {
        FieldInfo field = typeof(T).GetField(fieldName, bindingFlags);

        if (field == null)
        {
            throw new ArgumentException($"Field '{fieldName}' not found.");
        }

        field.SetValue(targetObject, value);
    }

    private void SetProperty(string propertyName, object value, BindingFlags bindingFlags)
    {
        PropertyInfo property = typeof(T).GetProperty(propertyName, bindingFlags);

        if (property == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found.");
        }

        property.SetValue(targetObject, value);
    }
}
