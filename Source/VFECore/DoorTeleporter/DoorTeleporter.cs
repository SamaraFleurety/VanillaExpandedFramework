﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace VFECore
{
    [StaticConstructorOnStartup]
    public class DoorTeleporter : ThingWithComps
    {
        public Material backgroundMat;
        public RenderTexture background1;
        public RenderTexture background2;
        public float rotation;
        public float distortAmount = 1.5f;
        public Vector2 backgroundOffset;
        public Sustainer sustainer;
        public string Name { get; set; }

        public static Dictionary<ThingDef, DoorTeleporterMaterials> doorTeleporterMaterials = new();
        static DoorTeleporter()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (typeof(DoorTeleporter).IsAssignableFrom(def.thingClass))
                {
                    var doorMaterials = doorTeleporterMaterials[def] = new DoorTeleporterMaterials();
                    doorMaterials.Init(def);
                }
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            WorldComponent_DoorTeleporterManager.Instance.DoorTeleporters.Add(this);
            var mat = doorTeleporterMaterials[this.def];
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                background1 = new RenderTexture(mat.backgroundTex.width, mat.backgroundTex.height, 0);
                background2 = new RenderTexture(mat.backgroundTex.width, mat.backgroundTex.height, 0);
                backgroundMat = new Material(ShaderDatabase.TransparentPostLight);
                this.RecacheBackground();
            });
        }

        public override void Tick()
        {
            base.Tick();
            this.rotation = (this.rotation + 0.5f) % 360f;
            this.distortAmount += 0.01f;
            if (this.distortAmount >= 3f) this.distortAmount = 1.5f;
            this.backgroundOffset += Vector2.one * 0.001f;
            this.RecacheBackground();
            var extension = this.def.GetModExtension<DoorTeleporterExtension>();
            if (extension.sustainer != null)
            {
                PlaySustainer(extension.sustainer);
            }
        }

        protected virtual void PlaySustainer(SoundDef sustainer)
        {
            this.sustainer ??= sustainer.TrySpawnSustainer(this);
            this.sustainer.Maintain();
        }

        public void RecacheBackground()
        {
            if (this.backgroundMat == null) return;
            var doorMaterials = doorTeleporterMaterials[def];
            Graphics.Blit(doorMaterials.backgroundTex, this.background1, Vector2.one, this.backgroundOffset, 0, 0);
            Graphics.Blit(this.background1, this.background2, doorMaterials.maskMat);
            this.backgroundMat.mainTexture = this.background2;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            this.sustainer?.End();
            this.sustainer = null;
            base.DeSpawn(mode);
            WorldComponent_DoorTeleporterManager.Instance.DoorTeleporters.Remove(this);
            Object.Destroy(this.background1);
            Object.Destroy(this.background2);
            Object.Destroy(this.backgroundMat);
        }

        public virtual void DoTeleportEffects(JobDriver_UseDoorTeleporter jobDriver)
        {

        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            var doorMaterials = doorTeleporterMaterials[def];
            var drawSize = new Vector3(this.def.size.x, 1, this.def.size.z);
            Graphics.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(drawLoc, Quaternion.AngleAxis(this.rotation, Vector3.up), drawSize * 1.5f), 
                doorMaterials.MainMat, 0);
            if (this.backgroundMat != null)
                Graphics.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(drawLoc - Altitudes.AltIncVect / 2, Quaternion.identity, drawSize * 1.5f),
                                  this.backgroundMat, 0);
            Graphics.DrawMesh(MeshPool.plane10,
                              Matrix4x4.TRS(drawLoc.Yto0() + Vector3.up * AltitudeLayer.MoteOverhead.AltitudeFor(), Quaternion.identity,
                                            drawSize * this.distortAmount * 2f),
                              doorMaterials.DistortionMat, 0);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos()) 
                yield return gizmo;
            foreach (Gizmo gizmo in GetDoorTeleporterGismoz())
                yield return gizmo;
        }
        public virtual IEnumerable<Gizmo> GetDoorTeleporterGismoz()
        {
            var extension = def.GetModExtension<DoorTeleporterExtension>();
            var doorMaterials = doorTeleporterMaterials[def];
            if (doorMaterials.DestroyIcon != null)
            {
                yield return new Command_Action
                {
                    defaultLabel = extension.destroyLabelKey.Translate(),
                    defaultDesc = extension.destroyDescKey.Translate(),
                    icon = doorMaterials.DestroyIcon,
                    action = () => this.Destroy()
                };
            }

            if (doorMaterials.RenameIcon != null) 
            {
                yield return new Command_Action
                {
                    defaultLabel = extension.renameLabelKey.Translate(),
                    defaultDesc = extension.renameDescKey.Translate(),
                    icon = doorMaterials.RenameIcon,
                    action = () => Find.WindowStack.Add(new Dialog_RenameDoorTeleporter(this))
                };
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption option in base.GetFloatMenuOptions(selPawn)) yield return option;

            foreach (DoorTeleporter doorTeleporter in WorldComponent_DoorTeleporterManager.Instance.DoorTeleporters.Except(this))
            {
                yield return new FloatMenuOption("VEF.TeleportTo".Translate(doorTeleporter.Name), () =>
                {
                    Job job = JobMaker.MakeJob(VFEDefOf.VEF_UseDoorTeleporter, this);
                    job.globalTarget = doorTeleporter;
                    selPawn.jobs.StartJob(job, JobCondition.InterruptForced, canReturnCurJobToPool: true);
                });
            }

        }

        public override void ExposeData()
        {
            base.ExposeData();
            string name = this.Name;
            Scribe_Values.Look(ref name, nameof(name));
            this.Name = name;
        }
    }
}