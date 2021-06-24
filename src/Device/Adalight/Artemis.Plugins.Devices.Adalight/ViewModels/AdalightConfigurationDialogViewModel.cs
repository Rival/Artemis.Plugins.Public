﻿using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared;
using RGB.NET.Devices.Adalight;
using Stylet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artemis.Plugins.Devices.Adalight
{

    public class AdalightConfigurationDialogViewModel : PluginConfigurationViewModel
    {
        private readonly IPluginManagementService _pluginManagementService;

        private readonly PluginSetting<List<AdalightDeviceDefinition>> _adalightDeviceDefinitionsSetting;
        private List<AdalightDeviceDefinition> _adalightDeviceDefinitions;

        public BindableCollection<AdalightDeviceDefinition> Definitions { get; }

        public AdalightConfigurationDialogViewModel(Plugin plugin, PluginSettings settings, IPluginManagementService pluginManagementService) : base(plugin)
        {
            _pluginManagementService = pluginManagementService;

            _adalightDeviceDefinitionsSetting = settings.GetSetting("AdalightDeviceDefinitionsSetting", new List<AdalightDeviceDefinition>());
            _adalightDeviceDefinitions = _adalightDeviceDefinitionsSetting.Value;

            Definitions = new BindableCollection<AdalightDeviceDefinition>(_adalightDeviceDefinitions);
        }

        public void AddDefinition()
        {
            Definitions.Add(new AdalightDeviceDefinition());
        }

        public void DeleteRow(object def)
        {
            if (def is AdalightDeviceDefinition deviceDefinition)
            {
                Definitions.Remove(deviceDefinition);
            }
        }

        public BindableCollection<ValueDescription> ScanModes { get; }

        protected override void OnClose()
        {

            _adalightDeviceDefinitionsSetting.Value.Clear();
            _adalightDeviceDefinitionsSetting.Value.AddRange(Definitions.Where(d => !string.IsNullOrWhiteSpace(d.Name)));
            _adalightDeviceDefinitionsSetting.Save();

            Task.Run(() =>
            {
                AdalightDeviceProvider deviceProvider = Plugin.GetFeature<AdalightDeviceProvider>();
                if (deviceProvider == null || !deviceProvider.IsEnabled) return;
                _pluginManagementService.DisablePluginFeature(deviceProvider, false);
                _pluginManagementService.EnablePluginFeature(deviceProvider, false);
            });
            base.OnClose();
        }
    }
}