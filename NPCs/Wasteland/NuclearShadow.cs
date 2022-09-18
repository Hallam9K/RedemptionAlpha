using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Placeable.Banners;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Wasteland
{
    public class NuclearShadow : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander
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
            Main.npcFrameCount[NPC.type] = 17;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 38;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCHit54;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 150;
            NPC.rarity = 2;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<NuclearShadowBanner>();
        }
        private Vector2 moveTo;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 120);
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            NPC.LookByVelocity();

            NPC.alpha += Main.rand.Next(-10, 11);
            NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 150, 230);

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }
                    break;

                case ActionState.Wander:
                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 120);
                        AIState = ActionState.Idle;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1.2f, 8, 8, NPC.Center.Y > player.Center.Y);
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            Point point = NPC.Center.ToTileCoordinates();
            if (Main.tile[point.X, point.Y].WallType == 0)
            {
                if (NPC.collideY || NPC.velocity.Y == 0)
                    NPC.frame.Y = 0;
                else
                    NPC.frame.Y = 17 * frameHeight;
                return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = frameHeight;
                else
                {
                    if (NPC.frame.Y < 3 * frameHeight)
                        NPC.frame.Y = 3 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 15 * frameHeight)
                            NPC.frame.Y = 3 * frameHeight;
                    }
                }
            }
            else
                NPC.frame.Y = 2 * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, 2), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return player.RedemptionAbility().SpiritwalkerActive || ItemTags.Arcane.Has(item.type) || ItemTags.Celestial.Has(item.type) || ItemTags.Holy.Has(item.type) || ItemTags.Psychic.Has(item.type) || RedeConfigClient.Instance.ElementDisable ? null : false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            return player.RedemptionAbility().SpiritwalkerActive || ProjectileTags.Arcane.Has(projectile.type) || ProjectileTags.Celestial.Has(projectile.type) || ProjectileTags.Holy.Has(projectile.type) || ProjectileTags.Psychic.Has(projectile.type) || RedeConfigClient.Instance.ElementDisable;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "A Human Shadow Etched in Stone, all that remains of someone who was vaporized by a nuclear blast. Also known as a Human Shadow of Death.")
            });
        }
    }
}