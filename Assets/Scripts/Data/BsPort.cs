using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Data
{
    public class BsPort : IHasId
    {
        public const byte TypeAny = 0;
        public const byte TypeBool = 1;
        public const byte TypeInt = 2;
        public const byte TypeFloat = 3;
        public const byte TypeString = 4;

        public string Id { get; set; }
        public byte Type { get; set; }
        public bool IsInput { get; set; }
        public Func<Dictionary<string, object>, object> GetValue { get; set; }
        public Action<Dictionary<string, object>, object> SetValue { get; set; }

        public BsPort(string id, byte type, Func<Dictionary<string, object>, object> getValue)
        {
            Id = id;
            Type = type;
            GetValue = getValue;
            IsInput = false;
        }

        public BsPort(string id, byte type, Action<Dictionary<string, object>, object> setValue)
        {
            Id = id;
            Type = type;
            SetValue = setValue;
            IsInput = true;
        }

        public static object Convert(object value, byte typeFrom, byte typeTo)
        {
            switch (typeFrom)
            {
                case TypeBool:
                    var valueBool = (bool)value;
                    return typeTo switch
                    {
                        TypeBool => valueBool,
                        TypeInt => valueBool ? 1 : 0,
                        TypeFloat => valueBool ? 1f : 0f,
                        TypeString => valueBool ? "true" : "false",
                        _ => throw Errors.InvalidItem("logic type", typeTo)
                    };
                case TypeInt:
                    var valueInt = (int)value;
                    return typeTo switch
                    {
                        TypeBool => valueInt >= 1,
                        TypeInt => valueInt,
                        TypeFloat => (float)valueInt,
                        TypeString => valueInt.ToString(),
                        _ => throw Errors.InvalidItem("logic type", typeTo)
                    };
                case TypeFloat:
                    var valueFloat = (float)value;
                    return typeTo switch
                    {
                        TypeBool => valueFloat >= .5f,
                        TypeInt => Mathf.RoundToInt(valueFloat),
                        TypeFloat => valueFloat,
                        TypeString => valueFloat.ToString(),
                        _ => throw Errors.InvalidItem("logic type", typeTo)
                    };
                case TypeString:
                    var valueString = (string)value;
                    return typeTo switch
                    {
                        TypeBool => valueString.Length > 0,
                        TypeInt => valueString.Length,
                        TypeFloat => (float)valueString.Length,
                        TypeString => valueString,
                        _ => throw Errors.InvalidItem("logic type", typeTo)
                    };
                default:
                    throw Errors.InvalidItem("logic type", typeFrom);
            }
        }
    }
}
