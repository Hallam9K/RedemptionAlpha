using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    public class CoopCrate : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 0),
                PortraitPositionYOverride = 28
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 22;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 2;
            NPC.lifeMax = 80;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.Dig;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
        }
        private Vector2 launch;
        public override bool? CanBeHitByItem(Player player, Item item) => NPC.scale <= 1.1f ? null : false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => NPC.scale <= 1.1f ? null : false;
        public override void AI()
        {
            NPC fowl = Main.npc[(int)NPC.ai[3]];
            Player player = Main.player[fowl.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();

            NPC.rotation += NPC.velocity.X * 0.07f;
            glowRot += 0.03f;

            switch (NPC.ai[0])
            {
                case 0:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.scale = 1.5f;
                            launch = new Vector2(player.Center.X < NPC.Center.X ? -Main.rand.Next(2, 5) : Main.rand.Next(2, 5), -Main.rand.Next(9, 12));
                            NPC.ai[1] = 1;
                            break;
                        case 1:
                            NPC.velocity = launch;
                            if (player.Center.Y > NPC.Center.Y)
                                NPC.ai[1] = 2;
                            break;
                        case 2:
                            if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            {
                                NPC.noGravity = false;
                                NPC.noTileCollide = false;
                                NPC.ai[1] = 3;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 3:
                            NPC.scale -= 0.01f;
                            if (NPC.ai[2]++ > 0 && BaseAI.HitTileOnSide(NPC, 3))
                            {
                                NPC.scale = 1;
                                NPC.ai[0] = 1;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case 1:
                    NPC.rotation = 0;
                    NPC.velocity.X *= .9f;
                    break;
            }
            NPC.scale = MathHelper.Max(1, NPC.scale);
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int g = 0; g < 3; g++)
                    {
                        int goreIndex = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                        Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                        Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.WoodFurniture, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                }

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Hay, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.WoodFurniture, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        private float glowRot = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive();

                spriteBatch.Draw(glow, NPC.Center - screenPos, new Rectangle(0, 0, glow.Width, glow.Height), Color.IndianRed, glowRot, origin, NPC.scale, 0, 0f);
                spriteBatch.Draw(glow, NPC.Center - screenPos, new Rectangle(0, 0, glow.Width, glow.Height), Color.IndianRed, -glowRot, origin, NPC.scale, 0, 0f);

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new EggCrackerCondition(), ModContent.ItemType<EggCracker>()));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<FowlEmperor>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.CoopCrate"))
            });
        }
    }
}