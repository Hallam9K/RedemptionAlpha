using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Utilities;
using Redemption.Items.Placeable.Banners;
using Redemption.Globals.NPC;
using System.IO;
using Terraria.Localization;

namespace Redemption.NPCs.PreHM
{
    public class Blobble : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Bounce
        }

        public enum HatState
        {
            None, GodsTophat, Fez, Crown, Flatcap, OldTophat, Serb
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public HatState HatType
        {
            get => (HatState)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blobble");
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.ShimmerSlime;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.NoBlood);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 18;
            NPC.damage = 2;
            NPC.defense = 0;
            NPC.lifeMax = 11;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.6f;
            NPC.value = 60;
            NPC.aiStyle = -1;
            NPC.alpha = 60;
            NPC.rarity = 1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BlobbleBanner>();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, new Color(178, 203, 177), 2);
                    Main.dust[dust].velocity *= 3f;
                    Main.dust[dust].noGravity = true;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, new Color(178, 203, 177), 2);
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
            PickHat();
            if (HatType is HatState.Serb)
                NPC.GivenName = "Serbble";

            TimerRand = Main.rand.Next(30, 120);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            NPC.TargetClosest();

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
                        TimerRand = Main.rand.Next(30, 120);
                        AIState = ActionState.Bounce;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Bounce:
                    NPC.velocity.X = Xvel * NPC.spriteDirection;
                    if (NPC.collideY || NPC.velocity.Y == 0)
                    {
                        AIState = ActionState.Idle;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 7;
            NPC.frame.X = NPC.frame.Width * (int)HatType;
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 2 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.03f;
                NPC.frame.Y = 2 * frameHeight;
            }
        }

        public void PickHat()
        {
            WeightedRandom<HatState> choice = new(Main.rand);
            choice.Add(HatState.None, 10); // 54%
            choice.Add(HatState.Crown, 2); // 11%
            choice.Add(HatState.Fez, 2);
            choice.Add(HatState.Flatcap, 2);
            choice.Add(HatState.GodsTophat, 1); // 5%
            choice.Add(HatState.OldTophat, 1);
            choice.Add(HatState.Serb, 0.3); // 1.6%

            HatType = choice;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 scale = new((NPC.velocity.X * 0.1f) + 1, (NPC.velocity.Y * 0.1f) + 1);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 2), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2,
                new Vector2(MathHelper.Clamp(scale.X, 1, 5), MathHelper.Clamp(scale.Y, 1, 5)), effects, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) => target.AddBuff(BuffID.Slimed, 120);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit) => target.AddBuff(BuffID.Slimed, 120);

        public override void OnKill()
        {
            if (HatType is HatState.Serb)
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Marshmallow);

            if (Main.rand.NextBool(2) && !RedeWorld.blobbleSwarm && RedeWorld.blobbleSwarmCooldown <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.NewText("A blobble swarm has arrived!", Color.PaleGreen);
                RedeWorld.blobbleSwarm = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 100));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDaySlime.Chance * 0.005f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 25);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Blobble"))
            });
        }
    }
}