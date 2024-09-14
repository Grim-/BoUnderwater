﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BoUnderwater
{
    //pixelwizardy
    public class GameCondition_UnderWaterCondition : GameCondition
    {
        private Color SkyColor = new Color(1f, 1f, 1f); // color of the sky
        private Color SkyColorNight = Color.white;
        private Color ShadowColor = new Color(0.4f, 0, 0, 0.2f); // color of any shadows
        private Color OverlayColor = new Color(0.5f, 0.5f, 0.5f); // strength (opacity) of the sky color
        private float Saturation = 0.75f; // saturation of the sky color (0 = grayscale)
        private float Glow = 1; // strength of any actual light in each cell on the map

        private CausticsOverlay CausticsOverlay;

        private List<SkyOverlay> ConditionOverlays = new List<SkyOverlay>();

        public override int TransitionTicks => 120; // how quickly the sky changes color (120 ticks = 2 seconds)
        public override void Init()
        {
            base.Init();

            UnderWaterGameConditionDef def = (UnderWaterGameConditionDef)this.def;

            this.SkyColor = def.SkyColor;
            this.SkyColorNight = def.SkyColorNight;
            this.ShadowColor = def.ShadowColor;
            this.OverlayColor = def.OverlayColor;
            this.Saturation = def.SkyColorSaturation;
            this.Glow = def.OverallGlowIntensityMultiplier;
        }
        public override void GameConditionTick()
        {
            base.GameConditionTick();
            List<Map> affectedMaps = base.AffectedMaps;
            foreach (var map in affectedMaps)
            {
                foreach (var item in SkyOverlays(map))
                {
                    item.TickOverlay(map);
                }
            }
        }

        public override void GameConditionDraw(Map map)
        {
            base.GameConditionDraw(map);

            if (map == null)
            {
                return;
            }

            foreach (var item in this.SkyOverlays(map))
            {
                item.DrawOverlay(map);

                if (item is CausticsOverlay causticsOverlay)
                {
                    causticsOverlay.UpdateZoom();
                    causticsOverlay.UpdateMaterial();
                }
            }
        }

        public override List<SkyOverlay> SkyOverlays(Map map)
        {
            return new List<SkyOverlay>() { new CausticsOverlay() };
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
        }

        public SkyColorSet TestSkyColors
        {
            get
            {
                float dayPercent = GenCelestial.CurCelestialSunGlow(Find.CurrentMap);
                Color lerpedColor = Color.Lerp(SkyColorNight, SkyColor, dayPercent);
                return new SkyColorSet(lerpedColor, ShadowColor, OverlayColor, Saturation);
            }
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget(Glow, TestSkyColors, 1f, 1f);
        }
    }

    public class CausticsOverlay : SkyOverlay
    {
        public Shader Shader;
        public Texture2D MainTex;
        public Texture2D DistortTex;
        public Material Material;

        private UnderwaterBiomeSettings Settings = null;

        public CausticsOverlay()
        {
            this.MainTex = ContentFinder<Texture2D>.Get("Noise8");
            this.DistortTex = ContentFinder<Texture2D>.Get("DistortionNoise4");
            this.Shader = (Shader)LoadedModManager.GetMod<UnderwaterBiome>().Content.assetBundles.loadedAssetBundles[0].LoadAsset("causticsshader");
            this.Material = new Material(this.Shader);
            this.Material.SetTexture("_MainTex", this.MainTex);
            this.Material.SetTexture("_DistortMap", this.DistortTex);


            this.Material.SetFloat("_Opacity", 0.14f);
            this.Material.SetFloat("_ScrollSpeed", 0.3f);
      
            this.Material.SetFloat("_DistortionSpeed", 0.04f);
            this.Material.SetFloat("_DistortionStrR", 0.06f);
            this.Material.SetFloat("_DistortionStrG", 0.06f);


            this.Material.SetColor("_Color", new Color(1, 1, 1));
            this.Material.SetColor("_node_9748", new Color(1, 1,1));


            this.worldOverlayMat = this.Material;
        }

        public void UpdateZoom()
        {

            float scaleAtMaxZoom = 10f;
            float baseScale = 4f;
    

            // Get the current zoom value
  
            // Calculate the final zoom scale
            float zoomScale = Mathf.Lerp(baseScale, scaleAtMaxZoom, GetZoom());

            this.Material.SetFloat("_ZoomScale", zoomScale);
        }

        private float GetZoom()
        {
            float minZoom = 10f;
            float maxZoom = 60f;
            float currentZoom = Find.CameraDriver.ZoomRootSize;



            var Settings = LoadedModManager.GetMod<UnderwaterBiome>().GetSettings<UnderwaterBiomeSettings>();
            float normalizedZoom = Mathf.InverseLerp(Settings.MinScale, Settings.ScaleAtMaxZoom, currentZoom);
            return normalizedZoom;
        }

        public void UpdateMaterial()
        {

           var Settings = LoadedModManager.GetMod<UnderwaterBiome>().GetSettings<UnderwaterBiomeSettings>();


            float Opact = Find.CameraDriver.CurrentZoom == CameraZoomRange.Close | Find.CameraDriver.CurrentZoom == CameraZoomRange.Closest ? 0 : Settings.Opacity;


            this.Material.SetFloat("_Opacity", Opact);
            this.Material.SetFloat("_ScrollSpeed", Settings.ScrollSpeed);
            this.Material.SetFloat("_DistortionSpeed", Settings.DistortionSpeed);
            this.Material.SetFloat("_DistortionStrR", Settings.DistortionStrR);
            this.Material.SetFloat("_DistortionStrG", Settings.DistortionStrG);
            this.Material.SetColor("_Color", Settings.Color);
            this.Material.SetColor("_node_9748", Color.Lerp(Settings.Color, Settings.Node9748Color, GetZoom()));
        }
    }
}
