using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace CustomPieces
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "com.hex.custompieces";
        private const string PluginName = "HexCustomPieces";
        private const string PluginVersion = "1.0.0";

        private const string AssetBundleResourceName =
            "CustomPieces.Assets.AssetBundles.campingpieces";

        private const string FireplaceShelterAssetPath =
            "assets/_customitems/campingpieces/fireplaceshelter.prefab";

        private Harmony _harmonyInstance;
        private AssetBundle _assetBundle;

        internal static ManualLogSource Log;
        internal static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            _assetBundle =
                AssetUtils.LoadAssetBundleFromResources(AssetBundleResourceName);

            if (_assetBundle == null)
            {
                Log.LogError(
                    $"Failed to load asset bundle: {AssetBundleResourceName}");
                return;
            }

            string[] assetNames = _assetBundle.GetAllAssetNames();

            Log.LogInfo(
                $"Assets found in '{AssetBundleResourceName}': {assetNames.Length}");

            foreach (string assetName in assetNames)
            {
                Log.LogInfo($"Bundle asset: {assetName}");
            }

            RegisterFireplaceShelter();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            Log?.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _assetBundle = null;
            Instance = null;
            Log = null;
        }

        private void RegisterFireplaceShelter()
        {
            GameObject fireplaceShelter =
                _assetBundle.LoadAsset<GameObject>(FireplaceShelterAssetPath);

            if (fireplaceShelter == null)
            {
                Log.LogError(
                    $"Failed to load prefab: {FireplaceShelterAssetPath}");
                return;
            }

            PieceConfig pieceConfig = new PieceConfig
            {
                Name = "Fireplace Shelter",
                Description = "A small wooden shelter for a fireplace.",
                PieceTable = PieceTables.Hammer,
                Category = PieceCategories.Misc,
                CraftingStation = null
            };

            pieceConfig.AddRequirement("Wood", 12);

            CustomPiece customPiece = new CustomPiece(
                fireplaceShelter,
                true,
                pieceConfig);

            PieceManager.Instance.AddPiece(customPiece);

            Log.LogInfo(
                $"Registered custom piece: {fireplaceShelter.name}");
        }
    }
}