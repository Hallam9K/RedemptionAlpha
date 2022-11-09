using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class SneezyFlinx : ModNPC
    {
        public enum ActionState
        {
            Buildup,
            Sneeze
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sneezy Snow Flinx");
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    ModContent.BuffType<PureChillDebuff>(),
                    ModContent.BuffType<IceFrozen>(),
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>()
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 40;
            NPC.damage = 56;
            NPC.friendly = false;
            NPC.defense = 6;
            NPC.lifeMax = 370;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 400f;
            NPC.knockBackResist = 1.1f;
            NPC.aiStyle = 3;
            AIType = NPCID.SnowFlinx;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandSnowBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SneezyFlinxBanner>();
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 25; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood);

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SneezyFlinxGore").Type, 1);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "Looks like its big nose has caught a cold! Seems like it wasn't prepared for weather as cold as one made by a nuclear winter.")
            });
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(400, 1000));
        }
        public override bool PreAI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookByVelocity();
            if (NPC.localAI[0] == 0 && NPC.velocity.Y == 0 && Main.rand.NextBool(500) && NPC.DistanceSQ(player.Center) < 600 * 600)
            {
                NPC.knockBackResist = 0f;
                NPC.velocity.X = 0;
                NPC.localAI[0] = 1;
                NPC.netUpdate = true;
            }
            if (NPC.localAI[0] > 0)
                return false;
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.localAI[0] != 0)
            {
                switch (AIState)
                {
                    case ActionState.Buildup:
                        if (NPC.frame.Y < 10 * frameHeight)
                            NPC.frame.Y = 10 * frameHeight;
                        if (NPC.frameCounter++ >= 10)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > 14 * frameHeight)
                            {
                                SoundEngine.PlaySound(SoundID.Item50, NPC.position);
                                NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
                                for (int i = 0; i < 25; i++)
                                {
                                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke);
                                    Main.dust[dustIndex2].velocity *= 5f;
                                }
                                for (int i = 0; i < 10; i++)
                                {
                                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenFairy);
                                    Main.dust[dustIndex2].velocity *= 5f;
                                }
                                AIState = ActionState.Sneeze;
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                    case ActionState.Sneeze:
                        if (NPC.frameCounter++ >= 180)
                        {
                            NPC.knockBackResist = 1.1f;
                            NPC.HitSound = SoundID.NPCHit1;
                            SoundEngine.PlaySound(SoundID.Item50, NPC.position);
                            for (int i = 0; i < 25; i++)
                            {
                                int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke);
                                Main.dust[dustIndex2].velocity *= 5f;
                            }
                            for (int i = 0; i < 10; i++)
                            {
                                int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenFairy);
                                Main.dust[dustIndex2].velocity *= 5f;
                            }
                            NPC.localAI[0] = 0;
                            AIState = ActionState.Buildup;
                            NPC.netUpdate = true;
                        }
                        break;
                }
                return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                NPC.frameCounter += NPC.velocity.X * 0.5f;
                if (NPC.frameCounter is >= 3 or <= -3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 9 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation += NPC.velocity.X * 0.05f;
                NPC.frame.Y = 15 * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sneezeTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Sneeze").Value;
            Vector2 SneezeOrigin = new(sneezeTex.Width / 2, sneezeTex.Height / 2);
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (NPC.localAI[0] != 0 && AIState == ActionState.Sneeze)
            {
                spriteBatch.Draw(sneezeTex, NPC.Center - screenPos - new Vector2(0, 6), null, NPC.GetAlpha(drawColor), NPC.rotation, SneezeOrigin, NPC.scale, effects, 0);
                return false;
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<XenomiteShard>(), 4, 4, 8));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 2, 2, 5));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.SnowFlinx, false);
            foreach (var dropRule in dropRules)
            {
                npcLoot.Add(dropRule);
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (AIState is ActionState.Sneeze)
            {
                if (item.pick > 0)
                    damage = item.damage + item.pick * 2;

            }
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (AIState is ActionState.Sneeze)
                damage *= 0.1;

            return true;
        }
    }
}