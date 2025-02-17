﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AnimalGenetics.Assembly;

[StaticConstructorOnStartup]
public static class AnimalGeneticsAssemblyLoader
{
    private static readonly List<PawnColumnDef> _DefaultAnimalsPawnTableDefColumns;
    private static readonly List<PawnColumnDef> _DefaultWildlifePawnTableDefColumns;
    public static readonly bool ColonyManagerLoaded;

    public static List<Type> gatherableTypes;

    static AnimalGeneticsAssemblyLoader()
    {
        var h = new Harmony("AnimalGenetics");
        h.PatchAll();

        DefDatabase<StatDef>.Add(AnimalGenetics.Damage);
        DefDatabase<StatDef>.Add(AnimalGenetics.Health);
        DefDatabase<StatDef>.Add(AnimalGenetics.GatherYield);

        var affectedStats = Constants.affectedStatsToInsert;
        foreach (var stat in affectedStats)
        {
            try
            {
                stat.parts?.Insert(0, new StatPart(stat));
            }
            catch
            {
                Log.Error($"[AnimalGenetics]: {stat} is broken");
            }
        }

        var category = new StatCategoryDef
            { defName = "AnimalGenetics_Category", label = "Genetics", displayAllByDefault = true, displayOrder = 200 };
        DefDatabase<StatCategoryDef>.Add(category);
        foreach (var stat in Constants.affectedStats)
        {
            DefDatabase<StatDef>.Add(new StatDefWrapper
            {
                defName = $"AnimalGenetics_{stat.defName}", label = Constants.GetLabel(stat), Underlying = stat,
                category = category, workerClass = typeof(StatWorker), toStringStyle = ToStringStyle.PercentZero
            });
        }

        StatDefOf.MarketValue.parts.Add(new MarketValueCalculator());

        gatherableTypes = new List<Type>
        {
            typeof(CompShearable),
            typeof(CompMilkable)
        };

        // Compatibility patches
        try
        {
            ColonyManagerLoaded = ModLister.GetActiveModWithIdentifier("Fluffy.ColonyManager") != null;

            if (LoadedModManager.RunningModsListForReading.Any(x =>
                    x.PackageId == "sarg.alphaanimals" || x.Name == "Alpha Animals"))
            {
                Log.Message("Animal Genetics : Alpha Animals is loaded - Patching");
                h.Patch(
                    AccessTools.Method(AccessTools.TypeByName("AlphaBehavioursAndEvents.CompAnimalProduct"),
                        "get_ResourceAmount"),
                    postfix: new HarmonyMethod(typeof(CompatibilityPatches),
                        nameof(CompatibilityPatches.AlphaAnimals_get_ResourceAmount_Patch)));
                gatherableTypes.Add(AccessTools.TypeByName("AlphaBehavioursAndEvents.CompAnimalProduct"));
            }

            if (LoadedModManager.RunningModsListForReading.Any(x =>
                    x.PackageId == "CETeam.CombatExtended" || x.Name == "Combat Extended"))
            {
                //gatherableTypes.Append(AccessTools.TypeByName("CombatExtended.CompMilkableRenameable")); //they all use shearable
                gatherableTypes.Add(AccessTools.TypeByName("CombatExtended.CompShearableRenameable"));
            }

            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "rim.job.world"))
            {
                Log.Message("[AnimalGenetics]: Patched RJW");
                h.Patch(AccessTools.Method(AccessTools.TypeByName("rjw.Hediff_BasePregnancy"), "GenerateBabies"),
                    new HarmonyMethod(typeof(CompatibilityPatches),
                        nameof(CompatibilityPatches.RJW_GenerateBabies_Prefix)),
                    new HarmonyMethod(typeof(CompatibilityPatches),
                        nameof(CompatibilityPatches.RJW_GenerateBabies_Postfix)));
            }
        }
        catch
        {
            // ignored
        }

        _DefaultAnimalsPawnTableDefColumns = new List<PawnColumnDef>(PawnTableDefOf.Animals.columns);
        _DefaultWildlifePawnTableDefColumns = new List<PawnColumnDef>(PawnTableDefOf.Wildlife.columns);

        var placeholderPosition =
            MainTabWindow_AnimalGenetics.PawnTableDefs.Genetics.columns.FindIndex(def =>
                def.defName == "AnimalGenetics_Placeholder");
        MainTabWindow_AnimalGenetics.PawnTableDefs.Genetics.columns.RemoveAt(placeholderPosition);
        MainTabWindow_AnimalGenetics.PawnTableDefs.Genetics.columns.InsertRange(placeholderPosition,
            PawnTableColumnsDefOf.Genetics.columns);

        PatchUI();
    }

    public static void PatchUI()
    {
        if (PatchState.PatchedGenesInAnimalsTab != Settings.UI.showGenesInAnimalsTab)
        {
            PawnTableDefOf.Animals.columns = new List<PawnColumnDef>(_DefaultAnimalsPawnTableDefColumns);
            if (Settings.UI.showGenesInAnimalsTab)
            {
                PawnTableDefOf.Animals.columns.AddRange(PawnTableColumnsDefOf.Genetics.columns);
            }

            PatchState.PatchedGenesInAnimalsTab = Settings.UI.showGenesInAnimalsTab;
        }

        if (PatchState.PatchedGenesInWildlifeTab != Settings.UI.showGenesInWildlifeTab)
        {
            PawnTableDefOf.Wildlife.columns = new List<PawnColumnDef>(_DefaultWildlifePawnTableDefColumns);
            if (Settings.UI.showGenesInWildlifeTab)
            {
                PawnTableDefOf.Wildlife.columns.AddRange(PawnTableColumnsDefOf.Genetics.columns);
            }

            PatchState.PatchedGenesInWildlifeTab = Settings.UI.showGenesInWildlifeTab;
        }

        var mainButton = DefDatabase<MainButtonDef>.GetNamed("AnimalGenetics");
        mainButton.buttonVisible = Settings.UI.showGeneticsTab;
    }

    public static class PatchState
    {
        public static bool PatchedGenesInAnimalsTab;
        public static bool PatchedGenesInWildlifeTab;
    }
}