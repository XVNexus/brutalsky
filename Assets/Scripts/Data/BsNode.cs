using System;
using System.Collections.Generic;
using Lcs;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Data
{
    public class BsNode : ILcsLine
    {
        public string Tag { get; set; }
        public object[] Config { get; set; }

        public Dictionary<string, object> State { get; set; } = new();
        public Func<BsPort[]> GetPorts { get; set; }
        public Action<Dictionary<string, object>> Init { get; set; }
        public Action<Dictionary<string, object>> Update { get; set; }

        public BsNode(string tag, params object[] config)
        {
            Tag = tag;
            Config = config;
        }

        public BsNode() { }

        public static BsNode Bool(bool value)
        {
            return new BsNode("bol", value)
            {
                GetPorts = () => new[] { new BsPort("value", BsPort.TypeBool, _ => value) }
            };
        }

        public static BsNode Int(int value)
        {
            return new BsNode("int", value)
            {
                GetPorts = () => new[] { new BsPort("value", BsPort.TypeInt, _ => value) }
            };
        }

        public static BsNode Float(float value)
        {
            return new BsNode("flt", value)
            {
                GetPorts = () => new[] { new BsPort("value", BsPort.TypeFloat, _ => value) }
            };
        }

        public static BsNode String(string value)
        {
            return new BsNode("str", value)
            {
                GetPorts = () => new[] { new BsPort("value", BsPort.TypeString, _ => value) }
            };
        }

        public static BsNode RandomBool()
        {
            return new BsNode("rbl")
            {
                GetPorts = () => new[]
                {
                    new BsPort("regen", BsPort.TypeBool, (state, value) =>
                    {
                        if ((bool)value)
                        {
                            state["value"] = Random.Range(0, 2) == 1;
                        }
                    }),
                    new BsPort("value", BsPort.TypeBool, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = Random.Range(0, 2) == 1;
                }
            };
        }

        public static BsNode RandomInt(int min, int max)
        {
            return new BsNode("rin", min, max)
            {
                GetPorts = () => new[]
                {
                    new BsPort("regen", BsPort.TypeBool, (state, value) =>
                    {
                        if ((bool)value)
                        {
                            state["value"] = Random.Range((int)state["min"], (int)state["max"]);
                        }
                    }),
                    new BsPort("min", BsPort.TypeInt, (state, value) => state["min"] = value),
                    new BsPort("max", BsPort.TypeInt, (state, value) => state["max"] = value),
                    new BsPort("value", BsPort.TypeInt, state => state["value"])
                },
                Init = state =>
                {
                    state["min"] = min;
                    state["max"] = max;
                    state["value"] = Random.Range(min, max);
                }
            };
        }

        public static BsNode RandomFloat(float min, float max)
        {
            return new BsNode("rfl", min, max)
            {
                GetPorts = () => new[]
                {
                    new BsPort("regen", BsPort.TypeBool, (state, value) =>
                    {
                        if ((bool)value)
                        {
                            state["value"] = Random.Range((float)state["min"], (float)state["max"]);
                        }
                    }),
                    new BsPort("min", BsPort.TypeFloat, (state, value) => state["min"] = value),
                    new BsPort("max", BsPort.TypeFloat, (state, value) => state["max"] = value),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["min"] = min;
                    state["max"] = max;
                    state["value"] = Random.Range(min, max);
                }
            };
        }

        public static BsNode Timer()
        {
            return new BsNode("tmr")
            {
                GetPorts = () => new[]
                {
                    new BsPort("value", BsPort.TypeInt, state => (int)state["value"])
                },
                Init = state =>
                {
                    state["value"] = 0;
                },
                Update = state =>
                {
                    state["value"] = (int)state["value"] + 1;
                }
            };
        }

        public static BsNode Clock(int interval)
        {
            return new BsNode("clk", interval)
            {
                GetPorts = () => new[]
                {
                    new BsPort("value", BsPort.TypeFloat, state => (bool)state["value"])
                },
                Init = state =>
                {
                    state["counter"] = 0;
                    state["value"] = false;
                },
                Update = state =>
                {
                    var counter = (int)state["counter"];
                    if (++counter >= interval)
                    {
                        state["value"] = !(bool)state["value"];
                        counter = 0;
                    }
                    state["counter"] = counter;
                }
            };
        }

        public static BsNode Delay(int duration)
        {
            return new BsNode("dly", duration)
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeAny, (state, value) => state["input"] = value),
                    new BsPort("value", BsPort.TypeAny, state => state["value"])
                },
                Init = state =>
                {
                    state["input"] = false;
                    state["buffer"] = new object[duration];
                    state["value"] = false;
                },
                Update = state =>
                {
                    var buffer = (object[])state["buffer"];
                    for (var i = 1; i < duration; i++)
                    {
                        buffer[i] = buffer[i - 1];
                    }
                    buffer[0] = state["input"];
                    state["buffer"] = buffer;
                    state["value"] = buffer[^1];
                }
            };
        }

        public static BsNode Multiplex(int size)
        {
            return new BsNode("mux", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort>
                    {
                        new("select", BsPort.TypeInt, (state, value) => state["select"] = value),
                        new("value", BsPort.TypeAny, state => state["value"])
                    };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeAny,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    state["select"] = 0;
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = false;
                    }
                    state["value"] = false;
                },
                Update = state =>
                {
                    var select = Mathf.Clamp((int)state["select"], 0, size - 1);
                    state["value"] = state[$"input-{select}"];
                }
            };
        }

        public static BsNode Demultiplex(int size)
        {
            return new BsNode("dmx", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort>
                    {
                        new("select", BsPort.TypeInt, (state, value) => state["select"] = value),
                        new("input", BsPort.TypeAny, (state, value) => state["input"] = value)
                    };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"value-{i}", BsPort.TypeAny, state => state[$"value-{i}"]));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    state["select"] = 0;
                    state["input"] = false;
                    for (var i = 0; i < size; i++)
                    {
                        state[$"value-{i}"] = false;
                    }
                },
                Update = state =>
                {
                    var select = Mathf.Clamp((int)state["select"], 0, size - 1);
                    for (var i = 0; i < size; i++)
                    {
                        state[$"value-{i}"] = false;
                    }
                    state[$"value-{select}"] = state["input"];
                }
            };
        }

        public static BsNode Buffer()
        {
            return new BsNode("buf")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeBool, (state, value) => state["value"] = value),
                    new BsPort("value", BsPort.TypeBool, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = false;
                }
            };
        }

        public static BsNode Not()
        {
            return new BsNode("not")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeBool, (state, value) => state["value"] = !(bool)value),
                    new BsPort("value", BsPort.TypeBool, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = false;
                }
            };
        }

        public static BsNode And(int size)
        {
            return new BsNode("and", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeBool, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeBool,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = false;
                    }
                    state["value"] = false;
                },
                Update = state =>
                {
                    var result = true;
                    for (var i = 0; i < size; i++)
                    {
                        result &= (bool)state[$"input-{i}"];
                    }
                    state["value"] = result;
                }
            };
        }

        public static BsNode Or(int size)
        {
            return new BsNode("lor", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeBool, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeBool,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = false;
                    }
                    state["value"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result |= (bool)state[$"input-{i}"];
                    }
                    state["value"] = result;
                }
            };
        }

        public static BsNode Xor(int size)
        {
            return new BsNode("xor", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeBool, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeBool,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = false;
                    }
                    state["value"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result ^= (bool)state[$"input-{i}"];
                    }
                    state["value"] = result;
                }
            };
        }

        public static BsNode Nand(int size)
        {
            return new BsNode("nan", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeBool, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeBool,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = false;
                    }
                    state["value"] = false;
                },
                Update = state =>
                {
                    var result = true;
                    for (var i = 0; i < size; i++)
                    {
                        result &= (bool)state[$"input-{i}"];
                    }
                    state["value"] = !result;
                }
            };
        }

        public static BsNode Nor(int size)
        {
            return new BsNode("nor", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeBool, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeBool,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = false;
                    }
                    state["value"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result |= (bool)state[$"input-{i}"];
                    }
                    state["value"] = !result;
                }
            };
        }

        public static BsNode Xnor(int size)
        {
            return new BsNode("xnr", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeBool, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeBool,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = false;
                    }
                    state["value"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result ^= (bool)state[$"input-{i}"];
                    }
                    state["value"] = !result;
                }
            };
        }

        public static BsNode Add(int size)
        {
            return new BsNode("add", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeFloat, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeFloat,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = 0f;
                    }
                    state["value"] = 0f;
                },
                Update = state =>
                {
                    var result = 0f;
                    for (var i = 0; i < size; i++)
                    {
                        result += (float)state[$"input-{i}"];
                    }
                    state["value"] = result;
                }
            };
        }

        public static BsNode Subtract(int size)
        {
            return new BsNode("sub", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeFloat, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeFloat,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = 0f;
                    }
                    state["value"] = 0f;
                },
                Update = state =>
                {
                    switch (size)
                    {
                        case > 1:
                        {
                            var result = (float)state["input-0"];
                            for (var i = 1; i < size; i++)
                            {
                                result -= (float)state[$"input-{i}"];
                            }
                            state["value"] = result;
                            break;
                        }
                        case 1:
                            state["value"] = -(float)state["input-0"];
                            break;
                        case 0:
                            state["value"] = 0f;
                            break;
                    }
                }
            };
        }

        public static BsNode Multiply(int size)
        {
            return new BsNode("mul", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeFloat, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeFloat,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = 0f;
                    }
                    state["value"] = 0f;
                },
                Update = state =>
                {
                    var result = 1f;
                    for (var i = 0; i < size; i++)
                    {
                        result *= (float)state[$"input-{i}"];
                    }
                    state["value"] = result;
                }
            };
        }

        public static BsNode Divide(int size)
        {
            return new BsNode("div", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { new("value", BsPort.TypeFloat, state => state["value"]) };
                    for (var i = 0; i < size; i++)
                    {
                        result.Add(new BsPort($"input-{i}", BsPort.TypeFloat,
                            (state, value) => state[$"input-{i}"] = value));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"input-{i}"] = 0f;
                    }
                    state["value"] = 0f;
                },
                Update = state =>
                {
                    switch (size)
                    {
                        case > 1:
                        {
                            var result = (float)state["input-0"];
                            for (var i = 1; i < size; i++)
                            {
                                result /= (float)state[$"input-{i}"];
                            }
                            state["value"] = result;
                            break;
                        }
                        case 1:
                            state["value"] = 1f / (float)state["input-0"];
                            break;
                        case 0:
                            state["value"] = 1f;
                            break;
                    }
                }
            };
        }

        public static BsNode Power()
        {
            return new BsNode("pow")
            {
                GetPorts = () => new[]
                {
                    new BsPort("base", BsPort.TypeFloat, (state, value) => state["base"] = value),
                    new BsPort("power", BsPort.TypeFloat, (state, value) => state["power"] = value),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["base"] = 0f;
                    state["power"] = 0f;
                    state["value"] = 0f;
                },
                Update = state =>
                {
                    state["value"] = Mathf.Pow((float)state["base"], (float)state["power"]);
                }
            };
        }

        public static BsNode Root()
        {
            return new BsNode("rot")
            {
                GetPorts = () => new[]
                {
                    new BsPort("base", BsPort.TypeFloat, (state, value) => state["base"] = value),
                    new BsPort("power", BsPort.TypeFloat, (state, value) => state["power"] = value),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["base"] = 0f;
                    state["power"] = 0f;
                    state["value"] = 0f;
                },
                Update = state =>
                {
                    state["value"] = Mathf.Pow((float)state["base"], 1f / (float)state["power"]);
                }
            };
        }

        public static BsNode Sin()
        {
            return new BsNode("sin")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeFloat, (state, value) => state["value"] = Mathf.Sin((float)value)),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = 0f;
                }
            };
        }

        public static BsNode Cos()
        {
            return new BsNode("cos")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeFloat, (state, value) => state["value"] = Mathf.Cos((float)value)),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = 0f;
                }
            };
        }

        public static BsNode Tan()
        {
            return new BsNode("tan")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeFloat, (state, value) => state["value"] = Mathf.Tan((float)value)),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = 0f;
                }
            };
        }

        public static BsNode Asin()
        {
            return new BsNode("asn")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeFloat, (state, value) => state["value"] = Mathf.Asin((float)value)),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = 0f;
                }
            };
        }

        public static BsNode Acos()
        {
            return new BsNode("acs")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeFloat, (state, value) => state["value"] = Mathf.Acos((float)value)),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = 0f;
                }
            };
        }

        public static BsNode Atan()
        {
            return new BsNode("atn")
            {
                GetPorts = () => new[]
                {
                    new BsPort("input", BsPort.TypeFloat, (state, value) => state["value"] = Mathf.Atan((float)value)),
                    new BsPort("value", BsPort.TypeFloat, state => state["value"])
                },
                Init = state =>
                {
                    state["value"] = 0f;
                }
            };
        }

        public LcsLine _ToLcs()
        {
            var props = new object[Config.Length + 1];
            props[0] = Tag;
            for (var i = 0; i < Config.Length; i++)
            {
                props[i + 1] = Config[i];
            }
            return new LcsLine('$', Tag, props);
        }

        public void _FromLcs(LcsLine line)
        {
            Tag = line.Get<string>(0);
            Config = new object[line.Props.Length - 1];
            for (var i = 0; i < Config.Length; i++)
            {
                Config[i] = line.Props[i + 1];
            }
            var result = Tag switch
            {
                "bol" => Bool((bool)Config[0]),
                "int" => Int((int)Config[0]),
                "flt" => Float((float)Config[0]),
                "str" => String((string)Config[0]),
                "rbl" => RandomBool(),
                "rin" => RandomInt((int)Config[0], (int)Config[1]),
                "rfl" => RandomFloat((float)Config[0], (float)Config[1]),
                "tmr" => Timer(),
                "clk" => Clock((int)Config[0]),
                "dly" => Delay((int)Config[0]),
                "mux" => Multiplex((int)Config[0]),
                "dmx" => Demultiplex((int)Config[0]),
                "buf" => Buffer(),
                "not" => Not(),
                "and" => And((int)Config[0]),
                "lor" => Or((int)Config[0]),
                "xor" => Xor((int)Config[0]),
                "nan" => Nand((int)Config[0]),
                "nor" => Nor((int)Config[0]),
                "xnr" => Xnor((int)Config[0]),
                "add" => Add((int)Config[0]),
                "sub" => Subtract((int)Config[0]),
                "mul" => Multiply((int)Config[0]),
                "div" => Divide((int)Config[0]),
                "pow" => Power(),
                "rot" => Root(),
                "sin" => Sin(),
                "cos" => Cos(),
                "tan" => Tan(),
                "asn" => Asin(),
                "acs" => Acos(),
                "atn" => Atan(),
                _ => throw Errors.InvalidItem("node tag", Tag)
            };
            Config = result.Config;
            GetPorts = result.GetPorts;
            Init = result.Init;
            Update = result.Update;
        }
    }
}
