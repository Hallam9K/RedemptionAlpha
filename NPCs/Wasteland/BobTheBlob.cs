using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity.Intruder;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.HM.Ranged;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class BobTheBlob : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Bounce
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bob the Blob");
            Main.npcFrameCount[NPC.type] = 5;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.NoBlood);
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
            ElementID.NPCPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 46;
            NPC.friendly = false;
            NPC.damage = 140;
            NPC.defense = 0;
            NPC.takenDamageMultiplier = 5f;
            NPC.lifeMax = 25000;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.knockBackResist = 0;
            NPC.rarity = 1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BobTheBlobBanner>();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 40; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), Scale: 3f);
                    Main.dust[dustIndex2].velocity *= 5f;
                }
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EpidotrianSkeletonGore2").Type, 1);

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EpidotrianSkeletonGore").Type, 1);
            }
            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), Scale: 2f);
            Main.dust[dustIndex].velocity *= 2f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DAN>(), 2));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.BeatAnyMechBoss(), ModContent.ItemType<Xenomite>(), 1, 12, 24));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 1, 6, 12));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HazmatSuit>(), 10));
            npcLoot.Add(ItemDropRule.OneFromOptions(3, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 20, 40));
            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 1000));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Xvel);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Xvel = reader.ReadInt32();
        }
        public int Xvel;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(10, 60);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            NPC.TargetClosest();

            Lighting.AddLight(NPC.Center, NPC.Opacity * 0.1f, NPC.Opacity, NPC.Opacity * 0.1f);

            switch (AIState)
            {
                case ActionState.Idle:
                    NPC.LookAtEntity(player);
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;

                    AITimer++;
                    if (AITimer >= TimerRand && (NPC.collideY || NPC.velocity.Y == 0))
                    {
                        Xvel = Main.rand.Next(3, 7);
                        NPC.velocity.X = Xvel * NPC.spriteDirection;
                        NPC.velocity.Y -= Main.rand.Next(4, 7);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(10, 60);
                        AIState = ActionState.Bounce;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Bounce:
                    NPC.velocity.X = Xvel * NPC.spriteDirection;
                    if (NPC.collideY || NPC.velocity.Y == 0)
                        AIState = ActionState.Idle;
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.03f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 scale = new((NPC.velocity.X * 0.1f) + 1, (NPC.velocity.Y * 0.1f) + 1);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 2), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, new Vector2(MathHelper.Clamp(scale.X, 1, 1.2f), MathHelper.Clamp(scale.Y, 1, 1.2f)), effects, 0);

            return false;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(200, 2400));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 10);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.BobTheBlob"))
            });
        }
    }
}