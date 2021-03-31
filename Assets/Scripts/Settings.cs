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
        var geneColorS = (string)(ui["gene_color"]);
        var geneFocusedColorS = (string)(ui["gene_focused_color"]);

        Color backboneColor;
        Color geneColor;
        Color geneFocusedColor;

        if (ColorUtility.TryParseHtmlString(backboneColorS, out backboneColor))
        {
            backboneMat.SetColor("_Color", backboneColor);
        }
        if (ColorUtility.TryParseHtmlString(geneColorS, out geneColor))
        {
            geneMat.SetColor("_Color", geneColor);
        }
        if (ColorUtility.TryParseHtmlString(geneFocusedColorS, out geneFocusedColor))
        {
            geneMatFocused.SetColor("_Color", geneFocusedColor);
        }
    }

    void Update()
    {

    }
}
