using System.Collections.Generic;
using JetBrains.Annotations;

namespace Config
{
    public struct ConfigDelta
    {
        public Dictionary<(string, string), object> NewValues { get; }

        public ConfigDelta(ConfigList list)
        {
            NewValues = new Dictionary<(string, string), object>();
            foreach (var section in list.Sections.Values)
            {
                foreach (var option in section.Options.Values)
                {
                    var value = option.GetIfChanged();
                    if (value != null)
                    {
                        NewValues[(section.Id, option.Id)] = value;
                    }
                }
            }
        }

        public object GetOrDefault(string sectionId, string optionId, object defaultValue)
        {
            return Get(sectionId, optionId) ?? defaultValue;
        }

        [CanBeNull]
        public object Get(string sectionId, string optionId)
        {
            return Has(sectionId, optionId) ? NewValues[(sectionId, optionId)] : null;
        }

        public bool Has(string sectionId, string optionId)
        {
            return NewValues.ContainsKey((sectionId, optionId));
        }
    }
}
