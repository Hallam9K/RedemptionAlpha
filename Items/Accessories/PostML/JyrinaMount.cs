using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Mounts;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Accessories.PostML
{
    public class JyrinaMount : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukon Ratsu");
            // Tooltip.SetDefault("Summons Jyrina and a ridable Lightning Chariot");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.UseSound = SoundID.Item79;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<JyrinaMount_Chariot>();
        }
    }
    public class JyrinaMount_Chariot : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = DustID.Sandnado;
            MountData.buff = ModContent.BuffType<JyrinaMountBuff>();
            MountData.heightBoost = 26;
            MountData.fallDamage = 0f;
            MountData.runSpeed = 17f;
            MountData.dashSpeed = 17f;
            MountData.flightTimeMax = 99999999;
            MountData.fatigueMax = 99999999;
            MountData.jumpHeight = 10;
            MountData.acceleration = 0.5f;
            MountData.jumpSpeed = 8f;
            MountData.blockExtraJumps = true;
            MountData.totalFrames = 5;
            MountData.usesHover = true;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 7;
            }
            MountData.playerYOffsets = array;
            MountData.xOffset = 1;
            MountData.bodyFrame = 3;
            MountData.yOffset = 20;
            MountData.playerHeadOffset = 18;
            MountData.standingFrameCount = 5;
            MountData.standingFrameDelay = 5;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 5;
            MountData.runningFrameDelay = 5;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 5;
            MountData.flyingFrameDelay = 5;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 5;
            MountData.inAirFrameDelay = 5;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 5;
            MountData.idleFrameDelay = 5;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }
        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            playerDrawData.Add(new DrawData(texture, drawPosition, frame, Color.White, rotation, drawOrigin, drawScale, spriteEffects, 0));
            return false;
        }
    }
}