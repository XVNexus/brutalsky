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
                GetPorts = () => new[] { new BsPort("val", BsPort.TypeBool, _ => value) }
            };
        }

        public static BsNode Int(int value)
        {
            return new BsNode("int", value)
            {
                GetPorts = () => new[] { new BsPort("val", BsPort.TypeInt, _ => value) }
            };
        }

        public static BsNode Float(float value)
        {
            return new BsNode("flt", value)
            {
                GetPorts = () => new[] { new BsPort("val", BsPort.TypeFloat, _ => value) }
            };
        }

        public static BsNode String(string value)
        {
            return new BsNode("str", value)
            {
                GetPorts = () => new[] { new BsPort("val", BsPort.TypeString, _ => value) }
            };
        }

        public static BsNode RandomBool()
        {
            return new BsNode("rbl")
            {
                GetPorts = () => new[]
                {
                    BsPort.Input("gen", BsPort.TypeBool),
                    BsPort.Output("out", BsPort.TypeBool)
                },
                Init = state =>
                {
                    state["gen"] = false;
                    state["out"] = Random.Range(0, 2) == 1;
                },
                Update = state =>
                {
                    if ((bool)state["gen"])
                    {
                        state["out"] = Random.Range(0, 2) == 1;
                    }
                }
            };
        }

        public static BsNode RandomInt(int min, int max)
        {
            return new BsNode("rin", min, max)
            {
                GetPorts = () => new[]
                {
                    BsPort.Input("gen", BsPort.TypeBool),
                    BsPort.Input("min", BsPort.TypeInt),
                    BsPort.Input("max", BsPort.TypeInt),
                    BsPort.Output("out", BsPort.TypeInt)
                },
                Init = state =>
                {
                    state["gen"] = false;
                    state["min"] = min;
                    state["max"] = max;
                    state["out"] = Random.Range(min, max + 1);
                },
                Update = state =>
                {
                    if ((bool)state["gen"])
                    {
                        state["out"] = Random.Range((int)state["min"], (int)state["max"] + 1);
                    }
                }
            };
        }

        public static BsNode RandomFloat(float min, float max)
        {
            return new BsNode("rfl", min, max)
            {
                GetPorts = () => new[]
                {
                    BsPort.Input("gen", BsPort.TypeBool),
                    BsPort.Input("min", BsPort.TypeFloat),
                    BsPort.Input("max", BsPort.TypeFloat),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["gen"] = false;
                    state["min"] = min;
                    state["max"] = max;
                    state["out"] = Random.Range(min, max);
                },
                Update = state =>
                {
                    if ((bool)state["gen"])
                    {
                        state["out"] = Random.Range((float)state["min"], (float)state["max"] + 1);
                    }
                }
            };
        }

        public static BsNode Timer()
        {
            return new BsNode("tmr")
            {
                GetPorts = () => new[]
                {
                    BsPort.Output("out", BsPort.TypeInt)
                },
                Init = state =>
                {
                    state["out"] = 0;
                },
                Update = state =>
                {
                    state["out"] = (int)state["out"] + 1;
                }
            };
        }

        public static BsNode Clock(int interval)
        {
            return new BsNode("clk", interval)
            {
                GetPorts = () => new[]
                {
                    BsPort.Output("out", BsPort.TypeBool)
                },
                Init = state =>
                {
                    state["con"] = 0;
                    state["out"] = false;
                },
                Update = state =>
                {
                    var counter = (int)state["con"];
                    if (++counter >= interval)
                    {
                        state["out"] = !(bool)state["out"];
                        counter = 0;
                    }
                    state["con"] = counter;
                }
            };
        }

        public static BsNode Delay(int duration)
        {
            return new BsNode("dly", duration)
            {
                GetPorts = () => new[]
                {
                    BsPort.Input("inp", BsPort.TypeAny),
                    BsPort.Output("out", BsPort.TypeAny)
                },
                Init = state =>
                {
                    state["inp"] = false;
                    state["buf"] = new object[duration + 1];
                    state["out"] = false;
                },
                Update = state =>
                {
                    var buffer = (object[])state["buf"];
                    for (var i = 1; i <= duration; i++)
                    {
                        buffer[i] = buffer[i - 1];
                    }
                    buffer[0] = state["inp"];
                    state["buf"] = buffer;
                    state["out"] = buffer[^1];
                }
            };
        }

        public static BsNode Monostable()
        {
            return new BsNode("mst")
            {
                GetPorts = () => new[]
                {
                    BsPort.Input("inp", BsPort.TypeAny),
                    BsPort.Output("out", BsPort.TypeBool)
                },
                Init = state =>
                {
                    state["inp"] = false;
                    state["buf"] = false;
                    state["out"] = false;
                },
                Update = state =>
                {
                    if (state["inp"] != state["buf"])
                    {
                        state["buf"] = state["inp"];
                        state["out"] = true;
                    }
                    else
                    {
                        state["out"] = false;
                    }
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
                        BsPort.Input("sel", BsPort.TypeInt),
                        BsPort.Output("out", BsPort.TypeAny)
                    };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeAny));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    state["sel"] = 0;
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = false;
                    }
                    state["out"] = false;
                },
                Update = state =>
                {
                    state["out"] = state[$"i{Mathf.Clamp((int)state["sel"], 0, size - 1):00}"];
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
                        new("sel", BsPort.TypeInt, (state, value) => state["sel"] = value),
                        new("inp", BsPort.TypeAny, (state, value) => state["inp"] = value)
                    };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"o{i:00}";
                        result.Add(BsPort.Output(id, BsPort.TypeAny));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    state["sel"] = 0;
                    state["inp"] = false;
                    for (var i = 0; i < size; i++)
                    {
                        state[$"o{i:00}"] = false;
                    }
                },
                Update = state =>
                {
                    var select = Mathf.Clamp((int)state["sel"], 0, size - 1);
                    for (var i = 0; i < size; i++)
                    {
                        state[$"o{i:00}"] = false;
                    }
                    state[$"o{select:00}"] = state["inp"];
                }
            };
        }

        public static BsNode Buffer()
        {
            return new BsNode("buf")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeBool, (state, value) => state["out"] = value),
                    BsPort.Output("out", BsPort.TypeBool)
                },
                Init = state =>
                {
                    state["out"] = false;
                }
            };
        }

        public static BsNode Not()
        {
            return new BsNode("not")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeBool, (state, value) => state["out"] = !(bool)value),
                    BsPort.Output("out", BsPort.TypeBool)
                },
                Init = state =>
                {
                    state["out"] = false;
                }
            };
        }

        public static BsNode And(int size)
        {
            return new BsNode("and", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeBool) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeBool));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = false;
                    }
                    state["out"] = false;
                },
                Update = state =>
                {
                    var result = true;
                    for (var i = 0; i < size; i++)
                    {
                        result &= (bool)state[$"i{i:00}"];
                    }
                    state["out"] = result;
                }
            };
        }

        public static BsNode Or(int size)
        {
            return new BsNode("lor", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeBool) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeBool));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = false;
                    }
                    state["out"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result |= (bool)state[$"i{i:00}"];
                    }
                    state["out"] = result;
                }
            };
        }

        public static BsNode Xor(int size)
        {
            return new BsNode("xor", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeBool) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeBool));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = false;
                    }
                    state["out"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result ^= (bool)state[$"i{i:00}"];
                    }
                    state["out"] = result;
                }
            };
        }

        public static BsNode Nand(int size)
        {
            return new BsNode("nan", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeBool) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeBool));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = false;
                    }
                    state["out"] = false;
                },
                Update = state =>
                {
                    var result = true;
                    for (var i = 0; i < size; i++)
                    {
                        result &= (bool)state[$"i{i:00}"];
                    }
                    state["out"] = !result;
                }
            };
        }

        public static BsNode Nor(int size)
        {
            return new BsNode("nor", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeBool) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeBool));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = false;
                    }
                    state["out"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result |= (bool)state[$"i{i:00}"];
                    }
                    state["out"] = !result;
                }
            };
        }

        public static BsNode Xnor(int size)
        {
            return new BsNode("xnr", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeBool) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeBool));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = false;
                    }
                    state["out"] = false;
                },
                Update = state =>
                {
                    var result = false;
                    for (var i = 0; i < size; i++)
                    {
                        result ^= (bool)state[$"i{i:00}"];
                    }
                    state["out"] = !result;
                }
            };
        }

        public static BsNode Add(int size)
        {
            return new BsNode("add", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeFloat) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeFloat));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = 0f;
                    }
                    state["out"] = 0f;
                },
                Update = state =>
                {
                    var result = 0f;
                    for (var i = 0; i < size; i++)
                    {
                        result += (float)state[$"i{i:00}"];
                    }
                    state["out"] = result;
                }
            };
        }

        public static BsNode Subtract(int size)
        {
            return new BsNode("sub", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeFloat) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeFloat));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = 0f;
                    }
                    state["out"] = 0f;
                },
                Update = state =>
                {
                    switch (size)
                    {
                        case > 1:
                        {
                            var result = (float)state["i00"];
                            for (var i = 1; i < size; i++)
                            {
                                result -= (float)state[$"i{i:00}"];
                            }
                            state["out"] = result;
                            break;
                        }
                        case 1:
                            state["out"] = -(float)state["i00"];
                            break;
                        case 0:
                            state["out"] = 0f;
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
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeFloat) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeFloat));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = 0f;
                    }
                    state["out"] = 0f;
                },
                Update = state =>
                {
                    var result = 1f;
                    for (var i = 0; i < size; i++)
                    {
                        result *= (float)state[$"i{i:00}"];
                    }
                    state["out"] = result;
                }
            };
        }

        public static BsNode Divide(int size)
        {
            return new BsNode("div", size)
            {
                GetPorts = () =>
                {
                    var result = new List<BsPort> { BsPort.Output("out", BsPort.TypeFloat) };
                    for (var i = 0; i < size; i++)
                    {
                        var id = $"i{i:00}";
                        result.Add(BsPort.Input(id, BsPort.TypeFloat));
                    }
                    return result.ToArray();
                },
                Init = state =>
                {
                    for (var i = 0; i < size; i++)
                    {
                        state[$"i{i:00}"] = 0f;
                    }
                    state["out"] = 0f;
                },
                Update = state =>
                {
                    switch (size)
                    {
                        case > 1:
                        {
                            var result = (float)state["i00"];
                            for (var i = 1; i < size; i++)
                            {
                                result /= (float)state[$"i{i:00}"];
                            }
                            state["out"] = result;
                            break;
                        }
                        case 1:
                            state["out"] = 1f / (float)state["i00"];
                            break;
                        case 0:
                            state["out"] = 1f;
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
                    BsPort.Input("bas", BsPort.TypeFloat),
                    BsPort.Input("pow", BsPort.TypeFloat),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["bas"] = 0f;
                    state["pow"] = 0f;
                    state["out"] = 0f;
                },
                Update = state =>
                {
                    state["out"] = Mathf.Pow((float)state["bas"], (float)state["pow"]);
                }
            };
        }

        public static BsNode Root()
        {
            return new BsNode("rot")
            {
                GetPorts = () => new[]
                {
                    BsPort.Input("bas", BsPort.TypeFloat),
                    BsPort.Input("pow", BsPort.TypeFloat),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["bas"] = 0f;
                    state["pow"] = 0f;
                    state["out"] = 0f;
                },
                Update = state =>
                {
                    state["out"] = Mathf.Pow((float)state["bas"], 1f / (float)state["pow"]);
                }
            };
        }

        public static BsNode Sin()
        {
            return new BsNode("sin")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeFloat, (state, value) => state["out"] = Mathf.Sin((float)value)),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["out"] = 0f;
                }
            };
        }

        public static BsNode Cos()
        {
            return new BsNode("cos")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeFloat, (state, value) => state["out"] = Mathf.Cos((float)value)),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["out"] = 0f;
                }
            };
        }

        public static BsNode Tan()
        {
            return new BsNode("tan")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeFloat, (state, value) => state["out"] = Mathf.Tan((float)value)),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["out"] = 0f;
                }
            };
        }

        public static BsNode Asin()
        {
            return new BsNode("asn")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeFloat, (state, value) => state["out"] = Mathf.Asin((float)value)),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["out"] = 0f;
                }
            };
        }

        public static BsNode Acos()
        {
            return new BsNode("acs")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeFloat, (state, value) => state["out"] = Mathf.Acos((float)value)),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["out"] = 0f;
                }
            };
        }

        public static BsNode Atan()
        {
            return new BsNode("atn")
            {
                GetPorts = () => new[]
                {
                    new BsPort("inp", BsPort.TypeFloat, (state, value) => state["out"] = Mathf.Atan((float)value)),
                    BsPort.Output("out", BsPort.TypeFloat)
                },
                Init = state =>
                {
                    state["out"] = 0f;
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
            return new LcsLine('$', props);
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
                "mst" => Monostable(),
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

        public override string ToString()
        {
            return $"NODE: {Tag}";
        }
    }
}
