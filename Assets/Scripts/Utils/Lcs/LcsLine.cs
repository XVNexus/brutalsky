using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Utils.Lcs
{
    public class LcsLine
    {
        public char Prefix { get; set; }
        public LcsProp[] Props { get; set; }
        public List<LcsLine> Children { get; set; }

        public LcsLine(char prefix, LcsProp[] props, [CanBeNull] List<LcsLine> children = null)
        {
            Prefix = prefix;
            Props = props;
            Children = children ?? new List<LcsLine>();
        }

        public string Stringify()
        {
            return Children.Aggregate(
                $"{Prefix}{Stringifier.CompressProps(Props.Select(prop => prop.Stringify()).ToArray())}\n",
                (current, child) => current + child.Stringify());
        }

        public static LcsLine Parse(string raw)
        {
            return new LcsLine(raw[0],
                Stringifier.ExpandProps(raw[1..]).Select(prop => LcsProp.Parse(prop)).ToArray());
        }

        public override string ToString()
        {
            return Stringify().TrimEnd();
        }
    }
}
