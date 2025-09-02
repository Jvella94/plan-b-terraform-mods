using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LibCommon;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LibCommon.GUITools;

namespace TreasureMap;

[BepInPlugin("averax.TreasureMap", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private static ManualLogSource logger;

    private static ConfigEntry<bool> modEnabled;
    private static ConfigEntry<int> fontSize;
    private static ConfigEntry<int> panelTop;
    private static ConfigEntry<KeyCode> keyCode;
    private static ConfigEntry<VisibilityTypes> visibilityType;
    private static ConfigEntry<bool> hideSecretNames;
    private static ConfigEntry<bool> autoScale;
    private static ConfigEntry<bool> ClickToSecretEnabled;

    private static int[] alienCoords = [-9999, -9999];
    private static int[] statueCoords = [-9999, -9999];
    private static int[] crabCoords = [-9999, -9999];
    private static string planetName = "";

    private static Color selectionColor = Color.blue;
    private static Color textColor = Color.black;
    private static List<PoiInfo> lastFoundSecrets = [];

    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        modEnabled = Config.Bind("General", "ModEnabled", true, "Enable/Disable this mod");
        fontSize = Config.Bind("General", "FontSize", 20, "The font size of the panel text");
        panelTop = Config.Bind("General", "PanelTop", 620, "The top position of the panel relative to the top of the screen");
        keyCode = Config.Bind("General", "TogglePanelKey", KeyCode.T, "The key to show/hide the panel");
        autoScale = Config.Bind("General", "AutoScale", true, "Scale the position and size of the button with the UI scale of the game?");
        hideSecretNames = Config.Bind("General", "HideSecretNames", true, "The names of the secrets are hidden by default.");
        ClickToSecretEnabled = Config.Bind("General", "ClickToSecretEnabled", false, "Allows the user to click the notification to go straight to the secret.");
        visibilityType = Config.Bind("General", "VisiblityType", VisibilityTypes.Default,
            "How close the mouse has to be for the notification to pop up. Check Mod Info for detailed explanation of choices.");
        
        logger = Logger;

        var h = Harmony.CreateAndPatchAll(typeof(Plugin));
        GUIScalingSupport.TryEnable(h);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SSceneHud), "OnUpdate")]
    private static bool SSceneHud_OnUpdate()
    {
        if (modEnabled.Value)
        {
            UpdatePanel();
            //return false;
        }
        else
        {
            if (poiPanel != null)
            {
                Destroy(poiPanel);
                poiPanel = null;
                poiPanelBackground = null;
                poiPanelBackground2 = null;
            }
        }
        return true;
    }

    private static GameObject poiPanel;
    private static GameObject poiPanelBackground;
    private static GameObject poiPanelBackground2;
    private static bool panelIsNotHidden = true;
    private static bool uiNotVisible;

    private static void UpdatePanel()
    {
        if (poiPanel == null)
        {
            poiPanel = new GameObject("TreasureMap");
            var canvas = poiPanel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 51;

            poiPanelBackground2 = new GameObject("TreasureMap_BackgroundBorder");
            poiPanelBackground2.transform.SetParent(poiPanel.transform);

            var img = poiPanelBackground2.AddComponent<Image>();
            img.color = new Color(118f / 255, 66f / 255, 32f / 255, 1f);

            poiPanelBackground = new GameObject("TreasureMap_Background");
            poiPanelBackground.transform.SetParent(poiPanel.transform);

            img = poiPanelBackground.AddComponent<Image>();
            img.color = new Color(254f / 255, 209f / 255, 108f / 255, 1f);
        }
        if (SSceneSingleton<SSceneHud>.Inst.isActiveAndEnabled && uiNotVisible) 
        { 
            uiNotVisible = true; 
            poiPanel.SetActive(false); 
            return; 
        }
        else if (!SSceneSingleton<SSceneHud>.Inst.isActiveAndEnabled && uiNotVisible) 
        { 
            uiNotVisible = false; 
        }
        if (planetName != GPlanet.name)
        {
            planetName = GPlanet.name;
            UpdateCoords();
        }
        int2 mouseoverCoords = GScene3D.mouseoverCoords;
        if (mouseoverCoords.x >= 0 && mouseoverCoords.y >= 0)
        {
            lastFoundSecrets = GetSecrets();
        }
        if (IsKeyDown(keyCode.Value))
        {
            poiPanel.SetActive(!poiPanel.activeSelf);
            panelIsNotHidden = poiPanel.activeSelf;
        }
        if (panelIsNotHidden)
        {
            poiPanel.SetActive(lastFoundSecrets.Count > 0);
        }
        
        if (!poiPanel.activeSelf)
        {
            return;
        }
        if (hideSecretNames.Value)
        {
            foreach (var poi in lastFoundSecrets)
            {
                poi.name = SLoc.Get("TreasureMap.Secret");
            }
        }
        lastFoundSecrets.Sort((a, b) => a.name.CompareTo(b.name));

        for (int i = poiPanelBackground.transform.childCount - 1; i >= 0; i--)
        {
               
            Destroy(poiPanelBackground.transform.GetChild(i).gameObject);
        }

        float theScale = autoScale.Value ? GUIScalingSupport.currentScale : 1f;

        float padding = 5 * theScale;
        float border = 5;
        float maxWidth = 0;
        float sumHeight = padding;
        List<GameObject> eachLine = [];
        List<PoiInfo> eachLinePoiInfo = [];

        float rollingY = -padding;

        var mp = GetMouseCanvasPos();


        for (int i = 0; i < lastFoundSecrets.Count && i < 3; i++)
        {

            PoiInfo poi = lastFoundSecrets[i];
            var textGo = new GameObject("TreasureMap_Poi_" + i);
            textGo.transform.SetParent(poiPanelBackground.transform);

            var txt = textGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = Mathf.RoundToInt(fontSize.Value * theScale);
            txt.color = textColor;
            txt.resizeTextForBestFit = false;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.alignment = TextAnchor.MiddleCenter;

            string title = poi.name;
            txt.text = "<b>" + title + "</b>";

            maxWidth = Math.Max(maxWidth, txt.preferredWidth);
            sumHeight += txt.preferredHeight + padding;

            var rectLine = textGo.GetComponent<RectTransform>();

            rectLine.localPosition = new Vector2(txt.preferredWidth, rollingY - txt.preferredHeight / 2);
            rectLine.sizeDelta = new Vector2(txt.preferredWidth, txt.preferredHeight);

            rollingY -= txt.preferredHeight + padding;

            eachLine.Add(textGo);
            eachLinePoiInfo.Add(poi);
        }

        var panelRect = poiPanelBackground.GetComponent<RectTransform>();

        var maxWidthWithPad = maxWidth + padding + padding;

        panelRect.localPosition = new Vector2(Screen.width / 2 - maxWidthWithPad / 2 - border * theScale, Screen.height / 2 - panelTop.Value * theScale - sumHeight / 2);
        panelRect.sizeDelta = new Vector2(maxWidthWithPad, sumHeight);

        var panelRect2 = poiPanelBackground2.GetComponent<RectTransform>();
        panelRect2.localPosition = panelRect.localPosition;
        panelRect2.sizeDelta = new Vector2(panelRect.sizeDelta.x + 2 * border * theScale, panelRect.sizeDelta.y + 2 * border * theScale);


        var panelX = Screen.width / 2 - maxWidthWithPad;

        for (int i = 0; i < eachLine.Count; i++)
        {
            GameObject ln = eachLine[i];
            var rectLine = ln.GetComponent<RectTransform>();

            var lp = rectLine.localPosition;
            rectLine.localPosition = new Vector2(-(maxWidth - lp.x) / 2, lp.y + sumHeight / 2);

            var txt = ln.GetComponent<Text>();

            if (Within(panelRect, rectLine, mp) && ClickToSecretEnabled.Value)
            {
                txt.color = selectionColor;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ShowCoords(eachLinePoiInfo[i].coords);
                }
            }
            else
            {
                txt.color = textColor;
              
            }
        }

    }

    private static void UpdateCoords()
    {
        foreach (var hex in GHexes.indexesRandom)
        {
            var coord = SMisc.IntToCoord(hex);
            var contentId = GHexes.contentId[coord.x, coord.y];
            switch (contentId)
            {
                case 79:
                    alienCoords = [coord.x, coord.y];
                    break;
                case 80:
                    statueCoords = [coord.x, coord.y];
                    break;
                case 81:
                    crabCoords = [coord.x, coord.y];
                    break;
            }
        }
    }

    private static List<PoiInfo> GetSecrets()
    {
        return visibilityType.Value switch
        {
            VisibilityTypes.Default => GetSecretVisibilityWithRange(26, 21),
            VisibilityTypes.FarAboveGround => GetSecretVisibilityWithRange(111, 71),
            VisibilityTypes.MagnifyingGlass => GetSecretVisibilityWithRange(6, 6),
            VisibilityTypes.PlanetWide => GetAllSecrets(),
            _ => [],
        };
    }

    private static List<PoiInfo> GetSecretVisibilityWithRange(int xRange, int yRange)
    {
        int2 mouseoverCoords = GScene3D.mouseoverCoords;
        List<PoiInfo> secrets = [];
        GetSecretLocation(xRange, yRange, mouseoverCoords, secrets, alienCoords, "TreasureMap.SpaceShip");
        GetSecretLocation(xRange, yRange, mouseoverCoords, secrets, statueCoords, "TreasureMap.Statue");
        GetSecretLocation(xRange, yRange, mouseoverCoords, secrets, crabCoords, "TreasureMap.MadCrab");
        return secrets;
    }

    private static void GetSecretLocation(int xRange, int yRange, int2 mouseoverCoords, List<PoiInfo> secrets, int[]secretCoords, string secretName)
    {
        bool isNearX = GetIsNearX(xRange, mouseoverCoords.x, secretCoords[0]);
        if (isNearX && Math.Abs(mouseoverCoords.y - secretCoords[1]) <= yRange)
        {
            secrets.Add(new PoiInfo { coords = new(secretCoords[0], secretCoords[1]), name = SLoc.Get(secretName, secretCoords[0], secretCoords[1]) });
        }
    }

    private static bool GetIsNearX(int xRange, int mouseoverXCoord, int secretXCoord)
    {
        int xColumns = 2048; // Current range from 0 to 2047 inclusive means 2048 positions total
        int dist = Math.Abs(mouseoverXCoord - secretXCoord);

        int wrappedDist = xColumns - dist;

        int shortestDist = Math.Min(dist, wrappedDist);

        return shortestDist <= xRange;
    }

    private static List<PoiInfo> GetAllSecrets()
    {
        return
        [
            new PoiInfo { coords = new(statueCoords[0], statueCoords[1]), name = SLoc.Get("TreasureMap.Statue") },
            new PoiInfo { coords = new(alienCoords[0], alienCoords[1]), name = SLoc.Get("TreasureMap.SpaceShip") },
            new PoiInfo { coords = new(crabCoords[0], crabCoords[1]), name = SLoc.Get("TreasureMap.MadCrab") },
        ];
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SMouse), nameof(SMouse.Update))]
    private static bool SMouse_Update()
    {
        if (poiPanel != null && poiPanel.activeSelf && poiPanelBackground2 != null)
        {
            var scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta != 0)
            {
                var mp = GetMouseCanvasPos();

                if (Within(poiPanelBackground2.GetComponent<RectTransform>(), mp))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Prevent click-through the panel
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SMouse), nameof(SMouse.IsCursorOnGround))]
    private static void SMouse_IsCursorOnGround(ref bool __result)
    {
        if (poiPanel != null && poiPanel.activeSelf
            && Within(poiPanelBackground2.GetComponent<RectTransform>(), GetMouseCanvasPos()))
        {
            __result = false;
        }
    }

    private static void ShowCoords(int2 coords)
    {
        SSceneSingleton<SSceneCinematic>.Inst.cameraMovement.SetDestination(coords);
        SSceneSingleton<SSceneCinematic>.Inst.cameraMovement.Play();
    }

    internal class PoiInfo
    {
        internal int2 coords;
        internal string name;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SLoc), nameof(SLoc.Load))]
    private static void SLoc_Load()
    {
        Translation.UpdateTranslations("English", new()
            {
                { "TreasureMap.SpaceShip", "Secret UFO is Nearby!" },
                { "TreasureMap.Statue", "Secret Statue is Nearby!" },
                { "TreasureMap.MadCrab", "Mad Crab is Nearby!" },
                { "TreasureMap.Secret", "A Secret is Nearby!" },
            });
        //Translation.UpdateTranslations("English", new()
        //        {
        //            { "TreasureMap.SpaceShip", "Secret UFO is Nearby at {0} {1}!" },
        //            { "TreasureMap.MadCrab", "Mad Crab is Nearby at {0} {1}!" },
        //            { "TreasureMap.Statue", "Secret Statue is Nearby at {0} {1}!!" },
        //            { "TreasureMap.Secret", "A Secret is Nearby!" },
        //        });
    }
}
