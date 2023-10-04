using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Items.Placeable.Banners;
using Redemption.NPCs.Friendly;
using Terraria.ModLoader.Utilities;
using Redemption.Globals;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Redemption.Base;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Redemption.Items.Usable.Potions;
using Redemption.BaseExtension;
using Redemption.Globals.NPC;
using Terraria.Localization;
using System;

namespace Redemption.NPCs.PreHM
{
    public class VagrantSpirit : ModNPC
    {
        public enum ActionState
        {
            Wander,
            Vanish
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public float[] oldrot = new float[10];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 62;
            NPC.height = 60;
            NPC.friendly = false;
            NPC.damage = 22;
            NPC.defense = 0;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.alpha = 100;
            NPC.chaseable = false;
            NPC.rarity = 1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<VagrantSpiritBanner>();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        private int vanishCounter;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(180, 420);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();

            NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 40);

            switch (AIState)
            {
                case ActionState.Wander:
                    if (NPC.velocity.X == 0)
                    {
                        NPC.LookAtEntity(player);
                        NPC.velocity.X = 1 * NPC.spriteDirection;
                    }
                    NPC.alpha -= 2;
                    NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 100, 255);

                    NPC.velocity.X *= 1.02f;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -1.4f, 1.4f);

                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(180, 420);
                        AIState = ActionState.Vanish;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Vanish:
                    NPC.alpha++;
                    if (NPC.alpha >= 255)
                    {
                        if (((player.ZoneOverworldHeight || player.ZoneSkyHeight) && !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive) || vanishCounter > 4)
                            NPC.active = false;

                        vanishCounter++;
                        NPC.position = new Vector2(player.Center.X + (600 * NPC.spriteDirection), player.Center.Y + Main.rand.Next(-400, 401));
                        NPC.LookAtEntity(player);
                        NPC.velocity.X = 1 * NPC.spriteDirection;
                        AIState = ActionState.Wander;
                    }
                    break;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.alpha < 150;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => target.noKnockback = true;

        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item) => RedeHelper.CanHitSpiritCheck(player, item);
        public override bool? CanBeHitByProjectile(Projectile projectile) => RedeHelper.CanHitSpiritCheck(projectile);

        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(1f, 2f));
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<Soulshake>(), 20));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 25);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.VagrantSpirit"))
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D Trail = ModContent.Request<Texture2D>(Texture + "_Trail").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive();

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(Trail, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.White) * 0.4f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Glow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
            {
                Texture2D LightGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
                float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 0.8f, 1.2f);
                Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(255, 246, 182), new Color(234, 133, 66), new Color(255, 246, 182));
                Rectangle rect = new(0, 0, LightGlow.Width, LightGlow.Height);
                Vector2 position1 = NPC.Center - screenPos + new Vector2(26 * NPC.spriteDirection, 14);
                Vector2 position2 = NPC.Center - screenPos + new Vector2(12 * NPC.spriteDirection, 30);
                Vector2 drawOrigin = new(LightGlow.Width / 2, LightGlow.Height / 2);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive();

                spriteBatch.Draw(LightGlow, position1, new Rectangle?(rect), NPC.GetAlpha(color) * 1.5f, MathHelper.PiOver2, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(LightGlow, position2, new Rectangle?(rect), NPC.GetAlpha(color) * 1.5f, MathHelper.PiOver2, drawOrigin, scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.RedemptionAbility().SpiritwalkerActive && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
                return 0.6f;

            return SpawnCondition.Cavern.Chance * 0.007f;
        }
    }
}