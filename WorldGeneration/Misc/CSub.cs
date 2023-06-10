using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.IO;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Globals;
using Redemption.NPCs.Critters;
using Redemption.BaseExtension;

namespace Redemption.WorldGeneration.Misc
{
    public class CSub : Subworld
    {
        public override int Width => 200;
        public override int Height => 200;
        public override bool NormalUpdates => false;
        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => false;
        public override List<GenPass> Tasks => new()
        {
            new CPass("???", 1),
        };
        public override void OnLoad()
        {
            Vector2 Pos;
            for (int i = 0; i < 100; i++)
            {
                Pos = NPCHelper.FindGroundVector(new Vector2(100, 100) * 16, 20);
                LabArea.SpawnNPCInWorld(Pos, ModContent.NPCType<Chicken>());
            }
            Pos = NPCHelper.FindGroundVector(new Vector2(100, 100) * 16, 20);
            LabArea.SpawnNPCInWorld(Pos, ModContent.NPCType<JohnSnail>());

            SubworldSystem.hideUnderworld = true;
            Main.cloudAlpha = 0;
            Main.cloudBGAlpha = 0;
            Main.numClouds = 0;
            Main.rainTime = 0;
            Main.raining = false;
            Main.maxRaining = 0f;
            Main.slimeRain = false;
        }
        public override void OnUnload()
        {
            Main.LocalPlayer.RedemptionScreen().interpolantTimer = 0;
        }
    }
    public class CPass : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "???";
            Main.spawnTileY = 110;
            Main.spawnTileX = 100;
            Main.worldSurface = Main.maxTilesY - 42;
            Main.rockLayer = Main.maxTilesY + 42;

            Mod mod = Redemption.Instance;
            Point16 dims = Point16.Zero;
            StructureHelper.Generator.GetDimensions("WorldGeneration/Misc/C", mod, ref dims);
            StructureHelper.Generator.GenerateStructure("WorldGeneration/Misc/C", new Point16(100 - (dims.X / 2), 100 - (dims.Y / 2)), mod);
        }
        public CPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
}