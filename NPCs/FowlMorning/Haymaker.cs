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
            NPC.lifeMax = 80;
            NPC.value = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.2f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.localAI[0] = Main.rand.Next(240, 401);
            NPC.netUpdate = true;
        }
        private bool laid;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();

            if (Main.rand.NextBool(3000))
                SoundEngine.PlaySound(CustomSounds.ChickenCluck with { Pitch = -.1f }, NPC.position);

            if (!laid && NPC.ai[1]++ >= NPC.localAI[0] && BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.velocity.X *= .2f;
                NPC.ai[2] = 1;
                if (NPC.ai[1] >= NPC.localAI[0] + 120)
                {
                    laid = true;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.localAI[0] = Main.rand.Next(240, 401);
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
        public override bool PreKill()
        {
            if (FowlMorningWorld.FowlMorningActive)
            {
                FowlMorningWorld.ChickPoints += Main.expertMode ? 8 : 4;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Halbirdhouse>(), 60));
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>(), 4));
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (NPC.life <= 0 && Main.rand.NextBool(4))
            {
                if (ItemLists.Fire.Contains(item.type))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (NPC.life <= 0 && Main.rand.NextBool(4))
            {
                if (ProjectileLists.Fire.Contains(projectile.type))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.velocity.Y != 0;
        public override void HitEffect(int hitDirection, double damage)
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

                new FlavorTextBestiaryInfoElement(
                    "Bwark!")
            });
        }
    }
}