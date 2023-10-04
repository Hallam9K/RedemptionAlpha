using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Redemption.Globals.World;
using Redemption.Items.Usable.Potions;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.PreHM.Magic;

namespace Redemption.NPCs.FowlMorning
{
    public class HeadlessChicken : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 24;
            NPC.friendly = false;
            NPC.damage = 18;
            NPC.defense = 0;
            NPC.lifeMax = 10;
            NPC.value = 20;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.3f;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<FowlMorningBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HeadlessChickenBanner>();
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[1] = Main.rand.Next(180, 221);
        }
        private bool spawnFire;
        public override void AI()
        {
            if (!spawnFire)
            {
                NPC.Shoot(NPC.Center + new Vector2(5 * NPC.spriteDirection, -20), ModContent.ProjectileType<HeadlessChicken_Fire>(), NPC.damage, Vector2.Zero, NPC.whoAmI);
                spawnFire = true;
            }

            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();
            if (NPC.DespawnHandler(3))
                return;

            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform);
            if (NPC.ai[0]++ == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int dust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1f);
                    Main.dust[dust2].velocity *= 0;
                    Main.dust[dust2].noGravity = true;
                    Color dustColor2 = new(217, 84, 155) { A = 0 };
                    Main.dust[dust2].color = dustColor2;
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.2f);
                    Main.dust[dust].velocity *= .1f;
                    Main.dust[dust].noGravity = true;
                    Color dustColor = new(251, 151, 146) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
                NPC.Center = NPC.FindGroundPlayer(40) - new Vector2(0, 14);
                for (int i = 0; i < 20; i++)
                {
                    int dust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1f);
                    Main.dust[dust2].velocity *= 0;
                    Main.dust[dust2].noGravity = true;
                    Color dustColor2 = new(217, 84, 155) { A = 0 };
                    Main.dust[dust2].color = dustColor2;
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.2f);
                    Main.dust[dust].velocity *= .1f;
                    Main.dust[dust].noGravity = true;
                    Color dustColor = new(251, 151, 146) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
            }
            if (NPC.ai[0] <= 120)
                NPCHelper.HorizontallyMove(NPC, player.Center, 0.15f, 0.7f, 18, 18, NPC.Center.Y > player.Center.Y, player);
            else
                NPC.velocity.X *= 0.4f;

            if (NPC.ai[0] == NPC.ai[1] - 60)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse with { Pitch = .2f }, NPC.position);
                FlareTimer = 0;
                Flare = true;
            }

            if (NPC.ai[0] == NPC.ai[1])
                NPC.ai[2] = 1;
            if (NPC.ai[2] == 2)
            {
                NPC.ai[0] = 0;
                NPC.ai[2] = 0;
                NPC.ai[1] = Main.rand.Next(180, 221);
            }
            if (Flare)
            {
                FlareTimer++;
                if (FlareTimer > 60)
                {
                    Flare = false;
                    FlareTimer = 0;
                }
            }
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
                    if (NPC.frame.Y < frameHeight)
                        NPC.frame.Y = frameHeight;

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
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 2 * frameHeight;
            }
        }
        private float FlareTimer;
        private bool Flare;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flare)
            {
                Vector2 position = NPC.Center + new Vector2(5 * NPC.spriteDirection, -20);
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position - screenPos, Color.IndianRed, NPC.rotation, tex: ModContent.Request<Texture2D>("Redemption/Textures/WhiteEyeFlare").Value);
            }
        }
        public override bool PreKill()
        {
            if (FowlMorningWorld.FowlMorningActive && Main.netMode != NetmodeID.MultiplayerClient)
            {
                FowlMorningWorld.ChickPoints += Main.expertMode ? 10 : 5;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChickendWand>(), 60));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Grain>(), 200));
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
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(
                    "...")
            });
        }
    }
}
