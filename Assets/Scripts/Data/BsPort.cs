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

        private BsPort(string id, byte type, Func<Dictionary<string, object>, object> getValue)
        {
            Id = id;
            Type = type;
            GetValue = getValue;
            IsInput = false;
        }

        private BsPort(string id, byte type, Action<Dictionary<string, object>, object> setValue)
        {
            Id = id;
            Type = type;
            SetValue = setValue;
            IsInput = true;
        }

        public static BsPort Value(string id, byte type, object value)
        {
            return new BsPort(id, type, _ => value);
        }

        public static BsPort Input(string id, byte type)
        {
            return new BsPort(id, type, (state, value) => state[id] = value);
        }

        public static BsPort Output(string id, byte type)
        {
            return new BsPort(id, type, state => state[id]);
        }

        public static BsPort Getter(string id, byte type, Func<Dictionary<string, object>, object> get)
        {
            return new BsPort(id, type, get);
        }

        public static BsPort Setter(string id, byte type, Action<Dictionary<string, object>, object> set)
        {
            return new BsPort(id, type, set);
        }

        public static object Convert(object value, byte typeFrom, byte typeTo)
        {
            switch (typeFrom)
            {
                case TypeAny:
                    return Convert(value, value switch
                    {
                        bool => TypeBool,
                        int => TypeInt,
                        float => TypeFloat,
                        string => TypeString,
                        _ => throw Errors.InvalidItem("logic value", value)
                    }, typeTo);
                case TypeBool:
                    var valueBool = (bool)value;
                    return typeTo switch
                    {
                        TypeAny => value,
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
                        TypeAny => value,
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
                        TypeAny => value,
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
                        TypeAny => value,
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

        public override string ToString()
        {
            var typeString = Type switch
            {
                TypeAny => "any",
                TypeBool => "bol",
                TypeInt => "int",
                TypeFloat => "flt",
                TypeString => "str",
                _ => throw Errors.InvalidItem("port type", Type)
            };
            var ioString = IsInput ? "i" : "o";
            return $"PORT: {ioString}:{Id}.{typeString}";
        }
    }
}
