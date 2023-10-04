using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.PreHM
{
    public class RaveyardSkeleton : SkeletonBase
    {
        public enum ActionState
        {
            Trumpet,
            Dancing
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public int DanceType;

        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Dancing Skeleton");
            Main.npcFrameCount[NPC.type] = 36;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 46;
            NPC.damage = 18;
            NPC.friendly = false;
            NPC.defense = 7;
            NPC.lifeMax = 54;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 95;
            NPC.knockBackResist = 0.1f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<EpidotrianSkeletonBanner>();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EpidotrianSkeletonGore2").Type, 1);

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EpidotrianSkeletonGore").Type, 1);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (Main.rand.NextBool(3))
            {
                int life = NPC.life;
                NPC.Transform(ModContent.NPCType<EpidotrianSkeleton>());
                NPC.life = life;
                (Main.npc[NPC.whoAmI].ModNPC as EpidotrianSkeleton).HasEyes = HasEyes;
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
                TimerRand = Main.rand.Next(80, 280);
                NPC.netUpdate = true;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DanceType);
            writer.Write(DanceSpeed);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DanceType = reader.ReadInt32();
            DanceSpeed = reader.ReadInt32();
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.rand.NextBool(4))
                HasEyes = true;
            SetStats();
            DanceType = Main.rand.Next(6);
            DanceSpeed = Main.rand.Next(4, 11);

            AIState = TimerRand == 0 ? ActionState.Trumpet : ActionState.Dancing;
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            NPC.TargetClosest();

            if (Main.rand.NextBool(800) && !Main.dedServ)
                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/SkeletonAmbient"), NPC.position);

            switch (AIState)
            {
                case ActionState.Trumpet:
                    if (Main.rand.NextBool(500) && !Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Doot, NPC.position);
                    break;
                case ActionState.Dancing:
                    int gotNPC2 = GetNearestNPC();
                    if (gotNPC2 == -1)
                    {
                        int life = NPC.life;
                        NPC.Transform(ModContent.NPCType<EpidotrianSkeleton>());
                        NPC.life = life;
                        (Main.npc[NPC.whoAmI].ModNPC as EpidotrianSkeleton).HasEyes = HasEyes;
                        TimerRand = Main.rand.Next(80, 280);
                        NPC.alpha = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            if (NPC.velocity.Y == 0)
                NPC.velocity.X = 0;

            int gotNPC = GetNearestNPC();
            if (gotNPC != -1)
                NPC.LookAtEntity(Main.npc[gotNPC]);
        }
        private int StartFrame;
        private int EndFrame;
        private int DanceSpeed = 10;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
                NPC.rotation = 0;
            else
                NPC.rotation = NPC.velocity.X * 0.05f;

            if (AIState is ActionState.Trumpet)
            {
                if (++NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 1 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
                }
                return;
            }
            switch (DanceType)
            {
                case 0:
                    StartFrame = 2;
                    EndFrame = 5;
                    break;
                case 1:
                    StartFrame = 6;
                    EndFrame = 9;
                    break;
                case 2:
                    StartFrame = 10;
                    EndFrame = 15;
                    break;
                case 3:
                    StartFrame = 16;
                    EndFrame = 21;
                    break;
                case 4:
                    StartFrame = 22;
                    EndFrame = 27;
                    break;
                case 5:
                    StartFrame = 28;
                    EndFrame = 35;
                    break;
            }

            if (NPC.frame.Y < StartFrame * frameHeight)
                NPC.frame.Y = StartFrame * frameHeight;
            if (++NPC.frameCounter >= DanceSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > EndFrame * frameHeight)
                    NPC.frame.Y = StartFrame * frameHeight;
            }
        }
        public int GetNearestNPC()
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.type != NPC.type || target.frame.Y >= 60)
                    continue;

                if (nearestNPCDist != -1 && !(target.Distance(NPC.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(NPC.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.5f));
            else if (Main.rand.NextBool(7))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.3f));
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Trumpet>(), 15));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EpidotrianSkull>(), 100));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 7));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.RaveyardSkeleton"))
            });
        }
    }
}