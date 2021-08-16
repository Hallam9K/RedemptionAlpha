using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.PreHM.Druid.Staves;
using Redemption.Items.Weapons.PreHM.Magic;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.PreHM
{
    public class AncientGladestoneGolem : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 12;
        }
        public override void SetDefaults()
        {
            npc.width = 54;
            npc.height = 80;
            npc.damage = 35;
            npc.friendly = false;
            npc.defense = 20;
            npc.lifeMax = 125;
            npc.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            npc.DeathSound = SoundID.NPCDeath3;
            npc.value = 5000;
            npc.knockBackResist = 0.2f;
            npc.aiStyle = -1;
            npc.lavaImmune = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AncientGladestoneGolemBanner>();
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.Stone, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f);
                Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.Stone, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f);
                Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.Stone, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f);
                Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.Stone, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f);
                Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.Stone, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore1"), 1);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore2"), 1);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore3"), 1);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore4"), 1);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore5"), 1);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore6"), 1);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore7"), 1);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hostile/AncientGolemGore8"), 1);
            }
            Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.Stone, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f);
            if (npc.ai[0] == 0)
            {
                Main.PlaySound(SoundID.Zombie, npc.position, 63);
                npc.ai[0] = 1;
            }
        }
        public override void NPCLoot()
        {
            RedePlayer redePlayer = Main.LocalPlayer.GetModPlayer<RedePlayer>();
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<AncientStone>(), Main.rand.Next(4, 13));
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<AncientDirt>(), Main.rand.Next(2, 7));
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<AncientWorldStave>());
            if (Main.rand.NextBool(30))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.NightVisionHelmet);
            }
            if (Main.rand.NextBool(redePlayer.bloomingLuck ? 150 : 200))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<VictorBattletome>());
            }
            if (Main.hardMode)
            {
                if (Main.rand.NextBool(redePlayer.bloomingLuck ? 150 : 200))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BlindJustice>());
                }
            }
        }
        public int aniType;
        public int aniFrame;
        public int frameCounter;
        public float[] customAI = new float[4];
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(customAI[0]);
            writer.Write(customAI[1]);
            writer.Write(customAI[2]);
            writer.Write(customAI[3]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            customAI[0] = reader.ReadFloat();
            customAI[1] = reader.ReadFloat();
            customAI[2] = reader.ReadFloat();
            customAI[3] = reader.ReadFloat();
        }
        public override void AI()
        {
            Player player = Main.player[npc.target];
            if (npc.collideY || npc.velocity.Y == 0)
            {
                npc.frameCounter += npc.velocity.X * 0.5f;
                if (npc.frameCounter >= 3 || npc.frameCounter <= -3)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += 88;
                    if (npc.frame.Y >= 1056)
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y = 0;
                    }
                }
            }
            else
            {
                npc.frame.Y = 88 * 3;
            }
            switch (npc.ai[0])
            {
                case 0:
                    if (npc.velocity.X > 0) { npc.spriteDirection = 1; }
                    else { npc.spriteDirection = -1; }
                    bool sight = (npc.Center.X > player.Center.X && npc.spriteDirection == -1) || (npc.Center.X < player.Center.X && npc.spriteDirection == 1);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && sight && player.active && !player.dead)
                    {
                        Main.PlaySound(SoundID.Zombie, npc.position, 63);
                        npc.ai[0] = 1;
                    }
                    if (npc.lavaWet && Main.rand.NextBool(250) && npc.velocity.Y == 0)
                    {
                        npc.ai[0] = 3;
                    }
                    break;
                case 1:
                    if (npc.velocity.X > 0) { npc.spriteDirection = 1; }
                    else { npc.spriteDirection = -1; }
                    npc.ai[1]++;
                    if (Main.rand.NextBool(200) && npc.velocity.Y == 0)
                    {
                        int tilePosY = BaseWorldGen.GetFirstTileFloor((int)player.Center.X / 16, (int)player.Center.Y / 16);
                        int dist = (tilePosY * 16) - (int)player.Center.Y;
                        if (npc.Distance(player.Center) < 300 && dist < 140 && player.active && !player.dead) { npc.ai[0] = 2; }
                        else { npc.ai[0] = 3; }
                    }
                    if (npc.ai[1] > 300)
                    {
                        npc.ai[1] = 0;
                        npc.ai[0] = 0;
                    }
                    break;
                case 2:
                    npc.velocity.X = 0;
                    aniType = 1;
                    npc.ai[1]++;
                    frameCounter++;
                    if (frameCounter > 5)
                    {
                        aniFrame++;
                        frameCounter = 0;
                        if (aniFrame == 1)
                        {
                            Main.PlaySound(SoundID.Zombie, npc.position, 64);
                            int tilePosY = BaseWorldGen.GetFirstTileFloor((int)player.Center.X / 16, (int)player.Center.Y / 16);
                            npc.Shoot(new Vector2(player.Center.X, (tilePosY * 16) + 55), ModContent.ProjectileType<AncientStonePillar_Pro>(), npc.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        }
                    }
                    if (aniFrame >= 10)
                    {
                        aniType = 0;
                        aniFrame = 0;
                        npc.ai[0] = 1;
                    }
                    break;
                case 3:
                    aniType = 1;
                    npc.ai[1]++;
                    frameCounter++;
                    if (aniFrame < 6) { npc.velocity.X = 0; }
                    if (frameCounter > 5)
                    {
                        aniFrame++;
                        frameCounter = 0;
                        if (aniFrame == 1)
                        {
                            Main.PlaySound(SoundID.Zombie, npc.position, 64);
                            int tilePosY = BaseWorldGen.GetFirstTileFloor((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                            npc.Shoot(new Vector2(npc.Center.X, (tilePosY * 16) + 55), ModContent.ProjectileType<AncientStonePillar_Pro>(), npc.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                        }
                        if (aniFrame == 6)
                        {
                            npc.velocity.Y -= Main.rand.Next(10, 20);
                            npc.velocity.X += npc.spriteDirection == 1 ? Main.rand.Next(2, 7) : Main.rand.Next(-7, -2);
                        }
                    }
                    if (aniFrame >= 10)
                    {
                        aniType = 0;
                        aniFrame = 0;
                        npc.ai[2] = 0;
                        npc.ai[0] = 1;
                    }
                    break;
            }
            if (npc.ai[0] < 2)
            {
                bool jumpDownPlatforms = false;
                npc.JumpDownPlatform(ref jumpDownPlatforms, 20);
                if (jumpDownPlatforms) { npc.noTileCollide = true; }
                else { npc.noTileCollide = false; }
                BaseAI.AIZombie(npc, ref customAI, false, true, -1, 0.1f, npc.ai[0] == 1 ? 3 : 1, 10, 1, jumpUpPlatforms: npc.Center.Y > player.Center.Y ? true : false);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            Texture2D attackAni = mod.GetTexture("NPCs/PreHM/AncientStoneGolem_Attack");
            var effects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            switch (aniType)
            {
                case 0:
                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2, npc.scale, npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                    break;
                case 1:
                    Vector2 drawCenter = new Vector2(npc.Center.X, npc.Center.Y);
                    int num214 = attackAni.Height / 10;
                    int y6 = num214 * aniFrame;
                    Main.spriteBatch.Draw(attackAni, drawCenter - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, attackAni.Width, num214)), drawColor, npc.rotation, new Vector2(attackAni.Width / 2f, num214 / 2f), npc.scale, effects, 0);
                    break;
            }
            return false;
        }
    }
}