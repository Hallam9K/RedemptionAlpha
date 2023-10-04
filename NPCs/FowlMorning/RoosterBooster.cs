using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria.GameContent.ItemDropRules;
using Redemption.Globals.World;
using Redemption.Items.Usable.Potions;
using Terraria.DataStructures;
using Redemption.Buffs.NPCBuffs;
using Redemption.Biomes;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Weapons.PreHM.Summon;
using Terraria.Localization;

namespace Redemption.NPCs.FowlMorning
{
    public class RoosterBooster : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 44;
            NPC.friendly = false;
            NPC.damage = 15;
            NPC.defense = 3;
            NPC.lifeMax = 46;
            NPC.value = 20;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.3f;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<FowlMorningBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<RoosterBoosterBanner>();
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.localAI[0] = Main.rand.Next(240, 401);
            NPC.netUpdate = true;
        }
        private bool rawr;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();
            if (NPC.DespawnHandler(3))
                return;

            if (Main.rand.NextBool(3000) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ChickenCluck with { Pitch = -.1f }, NPC.position);

            if (NPC.ai[1]++ >= NPC.localAI[0] && BaseAI.HitTileOnSide(NPC, 3))
            {
                if (!rawr)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.RoosterRoar with { Volume = .3f }, NPC.position);
                    rawr = true;
                }
                NPC.velocity.X *= .8f;
                NPC.ai[2] = 1;
                if (NPC.ai[1] % 20 == 0)
                {
                    RedeDraw.SpawnCirclePulse(NPC.Center, Color.IndianRed, .75f, NPC);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC target = Main.npc[i];
                        if (!target.active || target.whoAmI == NPC.whoAmI)
                            continue;

                        if (target.type != ModContent.NPCType<ChickenScratcher>() && target.type != ModContent.NPCType<ChickenBomber>() && target.type != ModContent.NPCType<Haymaker>() && target.type != ModContent.NPCType<Cockatrice>() && target.type != ModContent.NPCType<HeadlessChicken>() && target.type != ModContent.NPCType<Basan>())
                            continue;

                        if (NPC.DistanceSQ(target.Center) > 300 * 300)
                            continue;

                        target.AddBuff(ModContent.BuffType<RoosterBoostBuff>(), 300);
                    }
                }
                if (NPC.ai[1] >= NPC.localAI[0] + 120)
                {
                    rawr = false;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.localAI[0] = Main.rand.Next(240, 401);
                    NPC.netUpdate = true;
                }
                return;
            }
            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
            NPCHelper.HorizontallyMove(NPC, player.Center, 0.15f, 2f, 18, 18, NPC.Center.Y > player.Center.Y, player);

            if (NPC.Sight(player, 120, true, true) && NPC.ai[0] <= 0 && BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.velocity.X *= 2.4f;
                NPC.velocity.Y = -Main.rand.NextFloat(3f, 5f);
                NPC.ai[0] = Main.rand.Next(120, 181);
                NPC.netUpdate = true;
            }
            if (NPC.ai[0] > 0)
                NPC.ai[0]--;
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
                    if (NPC.frame.Y > 11 * frameHeight)
                        NPC.frame.Y = 10 * frameHeight;
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
        public override bool PreKill()
        {
            if (FowlMorningWorld.FowlMorningActive && Main.netMode != NetmodeID.MultiplayerClient)
            {
                FowlMorningWorld.ChickPoints += Main.expertMode ? 6 : 3;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DawnHerald>(), 60));
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
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 40; i++)
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.RoosterBooster"))
            });
        }
    }
}
