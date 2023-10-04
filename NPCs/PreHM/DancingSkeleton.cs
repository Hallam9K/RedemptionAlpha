using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.NPCs.Friendly;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.PreHM
{
    public class DancingSkeleton : SkeletonBase
    {
        private static Asset<Texture2D> soulless;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            soulless = ModContent.Request<Texture2D>("Redemption/Textures/Misc/TheSoulless");
        }
        public override void Unload()
        {
            soulless = null;
        }
        public override string Texture => "Redemption/NPCs/PreHM/RaveyardSkeleton";
        public enum ActionState
        {
            Begin,
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

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 46;
            NPC.damage = 0;
            NPC.friendly = false;
            NPC.defense = 66;
            NPC.lifeMax = 108;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 95;
            NPC.knockBackResist = 0f;
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
                    Gore.NewGore(NPC.GetSource_OnHit(NPC), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EpidotrianSkeletonGore2").Type, 1);

                Gore.NewGore(NPC.GetSource_OnHit(NPC), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EpidotrianSkeletonGore").Type, 1);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DanceType);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DanceType = reader.ReadInt32();
        }
        public override void AI()
        {
            Player player = Main.LocalPlayer;
            NPC.TargetClosest();
            switch (AIState)
            {
                case ActionState.Begin:
                    if (NPC.ai[3] == 0)
                        NPC.active = false;
                    SetStats();
                    DanceType = Main.rand.Next(6);

                    AIState = ActionState.Dancing;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Dancing:
                    if (NPC.life < NPC.lifeMax)
                    {
                        if (NPC.DistanceSQ(player.Center) < 800 * 800)
                            player.AddBuff(ModContent.BuffType<IslandDebuff>(), 30);
                        if (NPC.DistanceSQ(player.Center) < 200 * 200)
                            player.AddBuff(BuffID.OnFire, 30);
                        if (Main.rand.NextBool(200))
                        {
                            string s = "";
                            switch (Main.rand.Next(4))
                            {
                                case 0:
                                    s = "Ka dosmok cul', ut yai hu roma,";
                                    break;
                                case 1:
                                    s = "Acett'nin jugh, il noka voe yu commu,";
                                    break;
                                case 2:
                                    s = "Cult'nin un yei ruk', consu'nin yei min',";
                                    break;
                                case 3:
                                    s = "Ot I cun, jugh niqui tie.";
                                    break;
                            }
                            CombatText.NewText(NPC.getRect(), Color.Gray, s, false, false);
                        }
                    }
                    break;
            }
            if (NPC.velocity.Y == 0)
                NPC.velocity.X = 0;
        }
        private int StartFrame;
        private int EndFrame;
        private int AniFrameY;
        private int AniCounter;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
                NPC.rotation = 0;
            else
                NPC.rotation = NPC.velocity.X * 0.05f;

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
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > EndFrame * frameHeight)
                    NPC.frame.Y = StartFrame * frameHeight;
            }
            if (AniCounter++ >= 2)
            {
                AniCounter = 0;
                if (AniFrameY++ > 27)
                    AniFrameY = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.VoidDye);

            if (NPC.life < NPC.lifeMax)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (NPC.life < NPC.lifeMax)
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }

            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, NPC.life < NPC.lifeMax ? Color.Red : Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (NPC.life < NPC.lifeMax)
            {
                float opacity = (float)NPC.life / NPC.lifeMax;

                int Height = soulless.Value.Height / 28;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, soulless.Value.Width, Height);
                Vector2 origin = new(soulless.Value.Width / 2f, Height / 2f);
                spriteBatch.Draw(soulless.Value, NPC.Center - screenPos - new Vector2(0, 12), new Rectangle?(rect), drawColor * MathHelper.Lerp(0.6f, 0f, opacity), NPC.rotation, origin, NPC.scale, effects, 0);
                spriteBatch.Draw(soulless.Value, NPC.Center - screenPos - new Vector2(0, 12), new Rectangle?(rect), drawColor * MathHelper.Lerp(0.1f, 0f, opacity), NPC.rotation, origin, NPC.scale * 10, effects, 0);
                spriteBatch.Draw(soulless.Value, NPC.Center - screenPos - new Vector2(0, 12), new Rectangle?(rect), drawColor * MathHelper.Lerp(0.05f, 0f, opacity), NPC.rotation, origin, NPC.scale * 20, effects, 0);
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.2f));
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EpidotrianSkull>(), 100));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>()));
        }
    }
}