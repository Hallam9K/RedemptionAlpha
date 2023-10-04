using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Base;
using Terraria.GameContent.ItemDropRules;
using Redemption.Globals.World;
using Redemption.Items.Usable.Potions;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Redemption.NPCs.Minibosses.FowlEmperor;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Biomes;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Placeable.Banners;
using Terraria.Localization;

namespace Redemption.NPCs.FowlMorning
{
    public class Haymaker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 15;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.friendly = false;
            NPC.damage = 17;
            NPC.defense = 4;
            NPC.lifeMax = 60;
            NPC.value = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.2f;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<FowlMorningBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HaymakerBanner>();
        }
        public override void OnSpawn(IEntitySource source)
        {
            nestPos = NPC.Center;
            NPC.localAI[0] = Main.rand.Next(240, 801);
            NPC.netUpdate = true;
        }
        private bool laid;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();
            if (NPC.DespawnHandler(3))
                return;

            if (Main.rand.NextBool(3000) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ChickenCluck with { Pitch = -.1f }, NPC.position);

            if (!laid && NPC.ai[1]++ >= NPC.localAI[0] && BaseAI.HitTileOnSide(NPC, 3))
            {
                nestPos = NPC.Center + new Vector2(1, 10);
                nestOpacity += 0.02f;
                NPC.velocity.X *= .2f;
                NPC.ai[2] = 1;
                if (nestOpacity >= 1 && NPC.ai[1] >= NPC.localAI[0] + 100)
                {
                    nestOpacity = 1;
                    laid = true;
                    NPC.velocity.Y = -Main.rand.NextFloat(5f, 8f);
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.localAI[0] = Main.rand.Next(60, 301);
                    NPC.netUpdate = true;
                }
                return;
            }
            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
            NPCHelper.HorizontallyMove(NPC, player.Center, 0.1f, 1.2f, 18, 18, NPC.Center.Y > player.Center.Y, player);

            if (NPC.Sight(player, 80, true, true) && NPC.ai[0] <= 0 && BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.velocity.X *= 2.4f;
                NPC.velocity.Y = -Main.rand.NextFloat(3f, 5f);
                NPC.ai[0] = Main.rand.Next(120, 181);
                NPC.netUpdate = true;
            }
            if (NPC.ai[0] > 0)
                NPC.ai[0]--;

            if (nestOpacity >= 1 && NPC.ai[3]++ >= NPC.localAI[0])
            {
                float speed = MathHelper.Distance(player.Center.X, nestPos.X) / 100;
                speed = MathHelper.Clamp(speed, 1, 7);
                NPC.Shoot(nestPos, ModContent.ProjectileType<Rooster_EggBomb>(), (int)(NPC.damage * 1.1f), new Vector2(speed * player.Center.RightOfDir(nestPos), -Main.rand.Next(9, 10)).RotatedBy(Main.rand.NextFloat(-.2f, .2f)), SoundID.Item1);
                NPC.localAI[0] = Main.rand.Next(60, 301);
                NPC.ai[3] = 0;
            }
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead || !FowlMorningWorld.FowlMorningActive)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || !FowlMorningWorld.FowlMorningActive)
                {
                    NPC.alpha += 2;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[2] == 1)
            {
                NPC.rotation = 0;
                if (NPC.frame.Y < 10 * frameHeight)
                    NPC.frame.Y = 10 * frameHeight;

                if (++NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 14 * frameHeight)
                        NPC.frame.Y = 14 * frameHeight;
                }
                return;
            }
            if (NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 2 * frameHeight)
                        NPC.frame.Y = 2 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 9 * frameHeight)
                            NPC.frame.Y = 2 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = frameHeight;
            }
        }
        private float nestOpacity;
        private Vector2 nestPos;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D nestTex = ModContent.Request<Texture2D>(Texture + "_Nest").Value;
            Texture2D nestBack = ModContent.Request<Texture2D>(Texture + "_Nest_Back").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = Lighting.GetColor(nestPos.ToTileCoordinates());

            spriteBatch.Draw(nestBack, nestPos - screenPos, null, NPC.GetAlpha(color) * nestOpacity, 0, new Vector2(nestBack.Width / 2, nestBack.Height / 2), NPC.scale, 0, 0f);
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            spriteBatch.Draw(nestTex, nestPos - screenPos, null, NPC.GetAlpha(color) * nestOpacity, 0, new Vector2(nestTex.Width / 2, nestTex.Height / 2), NPC.scale, 0, 0f);
            return false;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.ai[2] is 1)
                modifiers.Knockback *= 0f;
        }
        public override bool PreKill()
        {
            if (FowlMorningWorld.FowlMorningActive && Main.netMode != NetmodeID.MultiplayerClient)
            {
                FowlMorningWorld.ChickPoints += Main.expertMode ? 8 : 4;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NestWand>(), 60));
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
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.velocity.Y != 0;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (nestOpacity >= 1)
                {
                    for (int i = 0; i < 30; i++)
                        Dust.NewDust(nestPos - new Vector2(27, 12), 54, 24, DustID.Hay, 0, -2, Scale: 2);
                }
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 40; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 4f;
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

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Haymaker"))
            });
        }
    }
}
