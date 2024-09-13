﻿using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using BiomesCore.DefModExtensions;

namespace BoUnderwater
{
    [StaticConstructorOnStartup]
    public static class TerrainDefModifier
    {
        static TerrainDefModifier()
        {
            LongEventHandler.QueueLongEvent(ModifyTerrainDefs, "ModifyingTerrainDefs", false, null);
        }

        private static void ModifyTerrainDefs()
        {
            Log.Message("BoUnderwater: Starting terrain def modifications");
            var terrainDefs = DefDatabase<TerrainDef>.AllDefs
                .Where(def => def.IsNaturalStone());

            foreach (var terrainDef in terrainDefs)
            {
                Log.Message($"BoUnderwater: Adding ModExtensions to {terrainDef.defName}");
                if (terrainDef.modExtensions == null)
                {
                    terrainDef.modExtensions = new List<DefModExtension>();
                }

                if (!terrainDef.modExtensions.Any(ext => ext is Biomes_PlantControl))
                {
                    terrainDef.modExtensions.Add(new Biomes_PlantControl
                    {
                        terrainTags = new List<string> { "IGrowOnStone" }
                    });
                }

                if (!terrainDef.modExtensions.Any(ext => ext is ConditionalFertilityExtension))
                {
                    terrainDef.modExtensions.Add(new ConditionalFertilityExtension
                    {
                        stoneFertilityOverride = 0.1f,
                        applicableBiomes = new List<BiomeDef> { UnderWaterDefOf.UB_ShallowsTropical }
                    });
                }
            }
            Log.Message("BoUnderwater: Finished terrain def modifications");
        }
    }

    //[HarmonyPatch(typeof(PlantUtility), "CanEverPlantAt",
    //        new[] { typeof(ThingDef), typeof(IntVec3), typeof(Map), typeof(Thing), typeof(bool), typeof(bool) },
    //        new[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
    //public static class Patch_PlantUtility_CanEverPlantAt
    //{
    //    public static bool Prefix(ThingDef plantDef, IntVec3 c, Map map, ref Thing blockingThing, bool canWipePlantsExceptTree, bool writeNoReason, ref AcceptanceReport __result)
    //    {
    //        if (map.Biome != UnderWaterDefOf.UB_ShallowsTropical)
    //        {
    //            return true;
    //        }

    //        if (plantDef == UnderWaterDefOf.UB_Plant_Anemone)
    //        {
    //            TerrainDef terrain = map.terrainGrid.TerrainAt(c);
    //            if (IsSpecialStonePlant(plantDef))
    //            {
    //                if (terrain != null && (terrain.defName.EndsWith("_Rough") || terrain.defName.EndsWith("_Smooth") || terrain.defName.EndsWith("_RoughHewn")))
    //                {
    //                    // Check for ConditionalFertilityExtension and apply fertility if applicable
    //                    var fertilityExt = terrain.GetModExtension<ConditionalFertilityExtension>();
    //                    if (fertilityExt != null && fertilityExt.applicableBiomes.Contains(map.Biome))
    //                    {
    //                       // Log.Message($"BoUnderwater: Modifying terrain def fertility to {fertilityExt.fertility}");
    //                        //terrain.fertility = fertilityExt.fertility;
    //                    }

    //                    Log.Message($"Allowing {plantDef.defName} on {terrain.defName}");
    //                    __result = AcceptanceReport.WasAccepted;
    //                    return false;
    //                }
    //            }
    //        }
    //        return true;
    //    }

    //    private static bool IsSpecialStonePlant(ThingDef plantDef)
    //    {
    //        return plantDef == UnderWaterDefOf.UB_Plant_Anemone;
    //    }
    //}



}
