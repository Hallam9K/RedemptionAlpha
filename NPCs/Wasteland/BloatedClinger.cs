using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Vanity.Intruder;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Redemption.Projectiles.Hostile;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class BloatedClinger : ModNPC
    {
        private static Asset<Texture2D> chainTexture;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            chainTexture = ModContent.Request<Texture2D>("Redemption/NPCs/Wasteland/BloatedClinger_Chain");
        }

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                CustomTexturePath = "Redemption/Textures/Bestiary/BloatedClinger_Bestiary",
                Position = new Vector2(0f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCPoison[Type] = true;
            ElementID.NPCShadow[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 34;
            NPC.friendly = false;
            NPC.damage = 70;
            NPC.defense = 30;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.value = 600f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.knockBackResist = 0.2f;
            NPC.behindTiles = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandCorruptionBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BloatedClingerBanner>();
        }
        private Vector2 tileOrigin;
        private Vector2 vector;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.rotation.SlowRotation((player.Center - NPC.Center).ToRotation() + MathHelper.Pi, MathHelper.Pi / 120);

            if (NPC.ai[0] is 0)
            {
                tileOrigin = BaseTile.GetClosestTile((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 4) * 16;
                if (!Framing.GetTileSafely((int)tileOrigin.X / 16, (int)tileOrigin.Y / 16).HasUnactuatedTile)
                    NPC.active = false;
                NPC.ai[0] = 1;
            }

            int maxDist = 200;
            if (NPC.ai[1]++ >= 200)
            {
                maxDist = 400;

                if (NPC.ai[1] >= 260 && NPC.ai[1] % 2 == 0 && NPC.ai[1] < 340 && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    NPC.Shoot(NPC.Center + (NPC.Center.DirectionTo(player.Center) * 4), ModContent.ProjectileType<BloatedClinger_Gas>(), (int)(NPC.damage * .64f), RedeHelper.PolarVector(-Main.rand.Next(10, 26), NPC.rotation + Main.rand.NextFloat(-.1f, .1f)), SoundID.NPCDeath13);
            }
            if (NPC.ai[1] >= 500)
            {
                NPC.ai[1] = Main.rand.Next(-120, 61);
                NPC.netUpdate = true;
            }
            if (++NPC.ai[2] % 30 == 0)
            {
                vector = new(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                NPC.ai[2] = 1;
            }
            NPC.Move(player.Center + vector, 4, 8);
            if (NPC.DistanceSQ(tileOrigin) > maxDist * maxDist)
                NPC.velocity -= tileOrigin.DirectionTo(NPC.position);

            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10, 10);
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -10, 10);

            if (!Framing.GetTileSafely(tileOrigin).HasTile)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 4 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (tileOrigin == Vector2.Zero)
                return false;
            Vector2 anchorPos = tileOrigin;
            Vector2 HeadPos = NPC.Center;
            Rectangle sourceRectangle = new(0, 0, chainTexture.Value.Width, chainTexture.Value.Height);
            Vector2 origin = new(chainTexture.Value.Width * 0.5f, chainTexture.Value.Height * 0.5f);
            float num1 = chainTexture.Value.Height;
            Vector2 vector2_4 = anchorPos - HeadPos;
            float rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(HeadPos.X) && float.IsNaN(HeadPos.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            Color color = Lighting.GetColor((int)HeadPos.X / 16, (int)(HeadPos.Y / 16));
            Main.EntitySpriteDraw(chainTexture.Value, tileOrigin - screenPos, new Rectangle?(sourceRectangle), NPC.GetAlpha(color), rotation, origin, 1, SpriteEffects.None, 0);
            while (flag)
            {
                if (vector2_4.Length() < num1 + 1.0)
                    flag = false;
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    HeadPos += vector2_1 * (num1 - 2);
                    vector2_4 = anchorPos - HeadPos;
                    color = Lighting.GetColor((int)HeadPos.X / 16, (int)(HeadPos.Y / 16));
                    Main.EntitySpriteDraw(chainTexture.Value, HeadPos - screenPos, new Rectangle?(sourceRectangle), NPC.GetAlpha(color), rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center + RedeHelper.PolarVector(8, (HeadPos - NPC.Center).ToRotation()) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.BeatAnyMechBoss(), ModContent.ItemType<Xenomite>(), 4, 3, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 4, 3, 6));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Clinger, false);
            foreach (var dropRule in dropRules)
            {
                npcLoot.Add(dropRule);
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(200, 700));
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 30; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.2f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                int steps = (int)NPC.Distance(tileOrigin) / 20;
                for (int i = 0; i < steps; i++)
                {
                    for (int j = 0; j < 20; j++)
                        Dust.NewDust(Vector2.Lerp(NPC.Center, tileOrigin, (float)i / steps) - new Vector2(12, 12), 24, 24, DustID.GreenBlood, Scale: 1.5f);
                }
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BloatedClingerGore1").Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BloatedClingerGore2").Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.BloatedClinger"))
            });
        }
    }
}