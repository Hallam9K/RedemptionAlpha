using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Vanity.Intruder;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Redemption.NPCs.Lab.MACE;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class NuclearSlime : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Wasteland/RadioactiveSlime";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.ShimmerSlime;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.NoBlood);
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
            ElementID.NPCPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 30;
            NPC.friendly = false;
            NPC.damage = 60;
            NPC.defense = 0;
            NPC.lifeMax = 300;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = 400f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 1;
            NPC.alpha = 40;
            AIType = NPCID.IlluminantSlime;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<NuclearSlimeBanner>();
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(200, 500));
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<XenomiteShard>(), 2, 6, 8));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 4, 1, 3));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 3, 8));
            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 10000));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
        }
        public override void PostAI()
        {
            Lighting.AddLight(NPC.Center, NPC.Opacity * 0.1f, NPC.Opacity, NPC.Opacity * 0.1f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.NuclearSlime"))
            });
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.frameCounter++ >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D NukeTex = ModContent.Request<Texture2D>(Texture + "_Overlay").Value;
            int Height = NukeTex.Height / 5;
            int y = Height * (NPC.frame.Y / 42);
            Rectangle rect = new(0, y, NukeTex.Width, Height);
            Vector2 origin = new(NukeTex.Width / 2f, Height / 2f);
            spriteBatch.Draw(NukeTex, NPC.Center - screenPos + new Vector2(14, -12), new Rectangle?(rect), drawColor, NPC.rotation, origin, NPC.scale, 0, 0);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 2), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, 0, 0);
            return false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                CombatText.NewText(NPC.getRect(), Color.Orange, "BOOM!", true, false);
                for (int i = 0; i < 15; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), Scale: 3f);
                    Main.dust[dustIndex2].velocity *= 5f;
                }
                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -8 + Main.rand.Next(0, 17), -3 + Main.rand.Next(-11, 0), ModContent.ProjectileType<MACE_Miniblast>(), NPCHelper.HostileProjDamage(100), 3, Main.myPlayer, 1);
                        Main.projectile[p].timeLeft = 300;
                        Main.projectile[p].friendly = true;
                        Main.projectile[p].netUpdate = true;
                    }
                }
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex3 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, Scale: 5f);
                    Main.dust[dustIndex3].velocity *= 2f;
                }
                // Fire Dust spawn
                for (int i = 0; i < 40; i++)
                {
                    int dustIndex4 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, Scale: 3f);
                    Main.dust[dustIndex4].noGravity = true;
                    Main.dust[dustIndex4].velocity *= 5f;
                    dustIndex4 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, Scale: 2f);
                    Main.dust[dustIndex4].velocity *= 3f;
                }
                // Large Smoke Gore spawn
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int g = 0; g < 8; g++)
                {
                    int goreIndex = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].scale = 1.5f;
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                }
            }
            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), Scale: 2f);
            Main.dust[dustIndex].velocity *= 2f;
        }
    }
}