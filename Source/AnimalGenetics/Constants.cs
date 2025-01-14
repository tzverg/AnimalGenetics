﻿using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AnimalGenetics;

public static class Constants
{
    // order here dictates order displayed in game.
    public static List<StatDef> affectedStats = new List<StatDef>
    {
        AnimalGenetics.Health,
        AnimalGenetics.Damage,
        StatDefOf.MoveSpeed,
        StatDefOf.CarryingCapacity,
        StatDefOf.MeatAmount,
        StatDefOf.LeatherAmount,
        AnimalGenetics.GatherYield
    };

    public static List<StatDef> affectedStatsToInsert = new List<StatDef>
    {
        StatDefOf.MoveSpeed,
        StatDefOf.LeatherAmount,
        StatDefOf.MeatAmount,
        StatDefOf.CarryingCapacity
    };

    private static readonly Dictionary<StatDef, string> _labelOverrides = new Dictionary<StatDef, string>
    {
        { StatDefOf.MoveSpeed, "AG.Speed".Translate() },
        { AnimalGenetics.Health, "AG.Health".Translate() },
        { AnimalGenetics.Damage, "AG.Damage".Translate() },
        { StatDefOf.CarryingCapacity, "AG.Capacity".Translate() },
        { StatDefOf.MeatAmount, "AG.Meat".Translate() },
        { StatDefOf.LeatherAmount, "AG.Leather".Translate() },
        { AnimalGenetics.GatherYield, "AG.GatherYield".Translate() }
    };

    private static readonly Dictionary<StatDef, string> _descriptionOverrides = new Dictionary<StatDef, string>
    {
        { StatDefOf.MoveSpeed, "AG.SpeedDesc".Translate() },
        { StatDefOf.CarryingCapacity, "AG.CapacityDesc".Translate() }
    };

    public static Dictionary<int, string> sortMode = new Dictionary<int, string>
    {
        { 0, "AG.None".Translate() },
        { 1, "AG.Asc".Translate() },
        { 2, "AG.Desc".Translate() }
    };

    public static string GetLabel(StatDef stat)
    {
        if (!_labelOverrides.ContainsKey(stat))
        {
            return stat.label;
        }

        return _labelOverrides[stat];
    }

    public static string GetDescription(StatDef stat)
    {
        if (!_descriptionOverrides.ContainsKey(stat))
        {
            return stat.description;
        }

        return _descriptionOverrides[stat];
    }
}