using System.Collections.Generic;
using Controllers.Base;
using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Config;
using Utils.Lcs;

namespace Controllers.Gui
{
    public class GuiCfController : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "cf";

        // External references
        private VisualTreeAsset _eConfigHeader;
        private VisualTreeAsset _eConfigOption;

        // Local variables
        private readonly Dictionary<(string, string), TextField> _configOptionFields = new();

        // Init functions
        protected override void OnLoad()
        {
            _eConfigHeader = Resources.Load<VisualTreeAsset>("Gui/Elements/ConfigHeader");
            _eConfigOption = Resources.Load<VisualTreeAsset>("Gui/Elements/ConfigOption");

            GuiSystem._.RegisterPane(PaneId, this, GuiPmController.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back", () =>
            {
                GuiSystem._.DeactivatePane(PaneId);
            });
            GuiSystem._.RegisterButton(PaneId, "save", () =>
            {
                var cfg = ConfigSystem._.List;
                foreach (var key in _configOptionFields.Keys)
                {
                    var option = cfg[key.Item1][key.Item2];
                    var optionField = _configOptionFields[key];
                    try
                    {
                        var newValue = LcsInfo.TypeTable[option.Type].Parse(optionField.value);
                        option.Value = newValue;
                    }
                    catch
                    {
                        //
                    }
                    optionField.value = LcsInfo.TypeTable[option.Type].Stringify(option.Value);
                }
                ConfigSystem._.SaveFile();
            });
            GuiSystem._.RegisterButton(PaneId, "rset", () =>
            {
                var cfg = ConfigSystem._.List;
                foreach (var key in _configOptionFields.Keys)
                {
                    var option = cfg[key.Item1][key.Item2];
                    _configOptionFields[key].value = LcsInfo.TypeTable[option.Type].Stringify(option.Value);
                }
            });
        }

        protected override void OnLink()
        {
            foreach (var section in ConfigSystem._.List.Sections.Values)
            {
                AddConfigSection(section);
            }
        }

        // Module functions
        private void AddConfigSection(ConfigSection section)
        {
            // Create new config section element
            var configSectionBox = new VisualElement();
            configSectionBox.AddToClassList("bs");
            configSectionBox.AddToClassList("bs-box");

            // Set up config header label
            var configHeader = _eConfigHeader.Instantiate();
            configHeader.Q<Label>("title").text = ConfigSystem._.NameTable[(section.Id, "")];
            configSectionBox.Add(configHeader);

            // Set up config option fields
            foreach (var option in section.Options.Values)
            {
                var configOption = _eConfigOption.Instantiate();
                configOption.Q<Label>("title").text = ConfigSystem._.NameTable[(section.Id, option.Id)];
                var valueField = configOption.Q<TextField>("value");
                valueField.value = LcsInfo.TypeTable[option.Type].Stringify(option.Value);
                configSectionBox.Add(configOption);
                _configOptionFields[(section.Id, option.Id)] = valueField;
            }

            // Add map tile to map picker view
            var mapTileContainer = GuiSystem._.GetPane(PaneId).Element.Q<VisualElement>("unity-content-container");
            mapTileContainer.Add(configSectionBox);
        }

        private void ClearConfigSections()
        {
            // Clear all config value text field references
            _configOptionFields.Clear();

            // Remove the config headers and options from the ui
            var configListContainer = GuiSystem._.GetPane(PaneId).Element.Q<VisualElement>("unity-content-container");
            configListContainer.Clear();
        }
    }
}
