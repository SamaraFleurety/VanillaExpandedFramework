﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Collections;

namespace ItemProcessor
{
    [StaticConstructorOnStartup]
    public static class GraphicsCache
    {

        //This class holds cached graphics so they can be accessed by the Building_ItemProcessor class

        public static readonly Color BarZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);

        public static readonly Color BarFermentedColor = new Color(0.9f, 0.85f, 0.2f);

        public static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);

    }
}

