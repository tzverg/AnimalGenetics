﻿using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics;

public class PawnColumnWorker_DamageGene : PawnColumnWorker
{
    private static readonly StatDef statDef = AnimalGenetics.Damage;

    public override GameFont DefaultHeaderFont => GameFont.Tiny;

    public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
    {
        var gene = Genes.GetGene(pawn, statDef);
        GUI.color = Utilities.TextColor(gene);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect, $"{gene * 100:F0}%");
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
    }

    public override int GetMinWidth(PawnTable table)
    {
        return 80;
    }

    public override int Compare(Pawn a, Pawn b)
    {
        return Genes.GetGene(a, statDef).CompareTo(Genes.GetGene(b, statDef));
    }
}