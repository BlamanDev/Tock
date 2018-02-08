using DFTGames.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesLocalize : Localize {
    const string RULES_SUFFIX = "_rules";

    public override void UpdateLocale()
    {
        if (!text) return; // catching race condition
        TextAsset rulesInFile = Resources.Load(Locale.STR_LOCALIZATION_PREFIX + Locale.currentLanguage + "_rules", typeof(TextAsset)) as TextAsset;
            text.text = rulesInFile.text;
    }
}
