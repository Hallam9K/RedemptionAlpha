using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals.Player
{
    public class RedePlayer : ModPlayer
    {
        public int spiritLevel = 0;
        public int maxSpiritLevel = 3;
        public bool foundHall;

        public override void ResetEffects()
        {
        }
        public override void Initialize()
        {
            foundHall = false;
        }
        public override void PostUpdateMiscEffects()
        {
            if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
            {
                ReLogic.Content.Asset<Texture2D> rain = ModContent.Request<Texture2D>("Redemption/Textures/RainOriginal", ReLogic.Content.AssetRequestMode.ImmediateLoad);
                ReLogic.Content.Asset<Texture2D> rainWasteland = ModContent.Request<Texture2D>("Redemption/Textures/Rain2", ReLogic.Content.AssetRequestMode.ImmediateLoad);

                if (Main.bloodMoon)
                    TextureAssets.Rain = rain;
                else if (Main.raining && Player.InModBiome(ModContent.GetInstance<WastelandPurityBiome>()))
                    TextureAssets.Rain = rainWasteland;
                else
                    TextureAssets.Rain = rain;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            var saveS = new List<string>();
            if (foundHall) saveS.Add("foundHall");

            tag["saveS"] = saveS;
        }

        public override void LoadData(TagCompound tag)
        {
            var saveS = tag.GetList<string>("saveS");
            foundHall = saveS.Contains("foundHall");
        }
    }
}