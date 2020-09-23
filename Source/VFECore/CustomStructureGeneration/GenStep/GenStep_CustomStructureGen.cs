﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using UnityEngine;
using Verse;

namespace KCSG
{
    class GenStep_CustomStructureGen : GenStep
    {
		public override int SeedPart
		{
			get
			{		   
				return 916516155;
			}
		}

		public override void Generate(Map map, GenStepParams parms)
		{
			StructureLayoutDef structureLayoutDef = structureLayoutDefs.RandomElement();

			int w, h;
			KCSG_Utilities.HeightWidthFromLayout(structureLayoutDef, out h, out w);
			CellRect cellRect = CellRect.CenteredOn(map.Center, w, h);

			if (structureLayoutDef.terrainGrid != null)
			{
				KCSG_Utilities.GenerateTerrainFromLayout(cellRect, map, structureLayoutDef);

				if (KCSG_Mod.settings.enableLog) Log.Message("Terrain generation - PASS");

			}
			int count = 0;
			foreach (List<String> item in structureLayoutDef.layouts)
			{
				KCSG_Utilities.GenerateRoomFromLayout(item, cellRect, map, structureLayoutDef);
				count++;
				if (KCSG_Mod.settings.enableLog) Log.Message("Layout " + count.ToString() + " generation - PASS");

			}
		}

		public List<StructureLayoutDef> structureLayoutDefs = new List<StructureLayoutDef>();
	}
}