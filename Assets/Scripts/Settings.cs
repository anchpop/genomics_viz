using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tomlyn;
using UnityEngine;

// Probably will run before other scripts
public class Settings : MonoBehaviour
{
    public Material backboneMat;
    public Material geneMat;
    public Material geneMatFocused;


    public static string dataUrl;
    string settingsUrl;
    string themeUrl;

    public static Color BackboneColor;
    public static Color SegmentColor;
    public static Color SegmentFocusedColor;
    public static Color ConnectionColor;

    void Start()
    {
        Debug.Log("Getting settings");
        dataUrl = Path.Combine(Application.streamingAssetsPath, "Chromosomal");
        settingsUrl = Path.Combine(dataUrl, "settings.toml");
        themeUrl = Path.Combine(dataUrl, "theme.toml");

        var themeText = File.ReadAllText(themeUrl);
        var themeDoc = Toml.Parse(themeText);
        var themeTable = themeDoc.ToModel();

        var ui = ((Tomlyn.Model.TomlTable)themeTable["ui"]);
        var backboneColorS = (string)(ui["backbone_color"]);
        var segmentColorS = (string)(ui["segment_color"]);
        var segmentFocusedColorS = (string)(ui["segment_focused_color"]);
        var connectionColorS = (string)(ui["connection_color"]);

        ColorUtility.TryParseHtmlString(backboneColorS, out BackboneColor);
        ColorUtility.TryParseHtmlString(segmentColorS, out SegmentColor);
        ColorUtility.TryParseHtmlString(segmentFocusedColorS, out SegmentFocusedColor);
        ColorUtility.TryParseHtmlString(connectionColorS, out ConnectionColor);
    }

    void Update()
    {

    }
    
}
