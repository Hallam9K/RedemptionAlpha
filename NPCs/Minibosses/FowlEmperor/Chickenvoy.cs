using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Base;
using Redemption.Items.Usable.Potions;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    public class Chickenvoy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chick Envoy");
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f,
                Position = new Vector2(0, 0),
                PortraitPositionYOverride = 28
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 34;
            NPC.friendly = false;
            NPC.damage = 13;
            NPC.defense = 1;
            NPC.lifeMax = 22;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.5f;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>(), 4));
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0 && Main.rand.NextBool(4))
            {
                if (item.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0 && Main.rand.NextBool(4))
            {
                if (projectile.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
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

            if (Main.rand.NextBool(1000) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ChickenCluck, NPC.position);

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
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform);
                    NPCHelper.HorizontallyMove(NPC, player.Center, 0.15f, 2.6f, 14, 14, NPC.Center.Y > player.Center.Y, player);
                    break;
            }
            NPC.scale = MathHelper.Max(1, NPC.scale);
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 8 * frameHeight)
                            NPC.frame.Y = frameHeight;
                    }
                }
            }
            else
            {
                if (NPC.ai[0] == 0)
                    NPC.rotation += NPC.velocity.X * 0.07f;
                else
                    NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 2 * frameHeight;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.ai[0] > 0;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<FowlEmperor>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Chickenvoy"))
            });
        }
    }
}