﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Bo_Underwater
{
    //pixelwizardy
    public class GameCondition_UnderWaterCondition : GameCondition
    {
        private Color SkyColor = new Color(1f, 1f, 1f); // color of the sky
        private Color ShadowColor = new Color(0.4f, 0, 0, 0.2f); // color of any shadows
        private Color OverlayColor = new Color(0.5f, 0.5f, 0.5f); // strength (opacity) of the sky color
        private float Saturation = 0.75f; // saturation of the sky color (0 = grayscale)
        private float Glow = 1.25f; // strength of any actual light in each cell on the map

        public override int TransitionTicks => 120; // how quickly the sky changes color (120 ticks = 2 seconds)
        public override void Init()
        {
            base.Init();

            UnderWaterGameConditionDef def = (UnderWaterGameConditionDef)this.def;

            this.SkyColor = def.SkyColor;
            this.ShadowColor = def.ShadowColor;
            this.OverlayColor = def.OverlayColor;
            this.Saturation = def.SkyColorSaturation;
            this.Glow = def.OverallGlowIntensityMultiplier;
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
        }

        public SkyColorSet TestSkyColors =>
           new SkyColorSet(SkyColor, ShadowColor, OverlayColor, Saturation);

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget(Glow, TestSkyColors, 1f, 1f);
        }
    }


    public class UnderWaterGameConditionDef : GameConditionDef
    {
        public Color SkyColor;
        public Color ShadowColor;
        public Color OverlayColor;

        public float SkyColorSaturation;
        public float OverallGlowIntensityMultiplier;

        public UnderWaterGameConditionDef()
        {
            conditionClass = typeof(GameCondition_UnderWaterCondition);
        }
    }
}
