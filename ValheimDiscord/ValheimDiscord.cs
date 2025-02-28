using BepInEx;
using UnityEngine;
using ValheimDiscord.Model;

namespace ValheimDiscord
{
    [BepInPlugin(_pluginGuid, _pluginName, _pluginVersion)]
    public sealed class ValheimDiscord : BaseUnityPlugin
    {
        private const string _pluginGuid = "com.keirloire.valheimdiscord";
        private const string _pluginName = "ValheimDiscord";
        private const string _pluginVersion = "1.0.0";

        private Configuration _config;

        public void Log(string message) => Debug.Log($"[{_pluginName}] {message}");

        public void Awake()
        {
            _config = new Configuration(Config);

            this.Log("Loaded.");
        }

    }
}
