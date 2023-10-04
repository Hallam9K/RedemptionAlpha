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
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Globals.World;
using Redemption.Items.Usable.Potions;
using Redemption.Biomes;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Accessories.PreHM;
using Terraria.Localization;

namespace Redemption.NPCs.FowlMorning
{
    public class ChickenScratcher : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 34;
            NPC.friendly = false;
            NPC.damage = 13;
            NPC.defense = 1;
            NPC.lifeMax = 12;
            NPC.value = 10;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.5f;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<FowlMorningBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ChickenScratcherBanner>();
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();
            if (NPC.DespawnHandler(3))
                return;

            if (Main.rand.NextBool(3000) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ChickenCluck, NPC.position);

            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform);
            if (NPC.Sight(player, 120, true, true) && NPC.ai[0] <= 0 && BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.velocity.X *= 2.4f;
                NPC.velocity.Y = -Main.rand.NextFloat(2f, 5f);
                NPC.ai[0] = Main.rand.Next(120, 181);
                NPC.netUpdate = true;
            }
            if (NPC.ai[0] > 0)
            {
                NPCHelper.HorizontallyMove(NPC, new Vector2(player.Center.X < NPC.Center.X ? NPC.Center.X + 100 : NPC.Center.X - 100, NPC.Center.Y), 0.13f, 3.8f, 18, 18, NPC.Center.Y > player.Center.Y, player);
                NPC.ai[0]--;
            }
            else
                NPCHelper.HorizontallyMove(NPC, player.Center, 0.13f, 3f, 18, 18, NPC.Center.Y > player.Center.Y, player);
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
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool() && Main.expertMode)
                target.AddBuff(BuffID.Bleeding, Main.rand.Next(60, 121));
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
                FowlMorningWorld.ChickPoints += Main.expertMode ? 2 : 1;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Halbirdhouse>(), 60));
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.ChickenScratcher"))
            });
        }
    }
}
