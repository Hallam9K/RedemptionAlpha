using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.NPCs.Critters;
using Redemption.Projectiles.Hostile;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class DevilsTongue : ModNPC
    {
        public ref float AIState => ref NPC.ai[0];

        public ref float AITimer => ref NPC.ai[1];

        public ref float CrittersConsumed => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Devil's Tongue");
            Main.npcFrameCount[Type] = 12;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<DevilScentedDebuff>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 50;
            NPC.defense = 2;
            NPC.damage = 0;
            NPC.lifeMax = 30;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<DevilsTongueBanner>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Cactus, 1, 6, 10));
            npcLoot.Add(ItemDropRule.Common(ItemID.PinkPricklyPear, 4));
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public int hitCooldown;
        public override void AI()
        {
            NPC.TargetClosest();
            switch (AIState)
            {
                case 0:
                    AITimer++;
                    if (AITimer > 240)
                    {
                        AITimer = 0;
                        AIState = 1;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer % 8 == 0)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<DevilsTongueCloud>(), 0, RedeHelper.Spread(3));
                    }
                    if (AITimer > 60)
                    {
                        AITimer = 0;
                        AIState = 0;
                    }
                    break;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC possibleTarget = Main.npc[i];
                if (!possibleTarget.active || possibleTarget.whoAmI == NPC.whoAmI ||
                    possibleTarget.lifeMax > 5 || !NPCID.Sets.CountsAsCritter[possibleTarget.type])
                    continue;

                if (!NPC.Hitbox.Intersects(possibleTarget.Hitbox))
                    continue;

                BaseAI.DamageNPC(possibleTarget, possibleTarget.lifeMax, 0, NPC, false);
                if (possibleTarget.life <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = .3f }, NPC.position);
                    if (NPC.life < NPC.lifeMax)
                    {
                        NPC.life += 5;
                        NPC.HealEffect(5);
                    }
                    if (NPC.life > NPC.lifeMax)
                        NPC.life = NPC.lifeMax;
                    CrittersConsumed++;
                }
            }
            if (NPC.ai[3] == 0 && CrittersConsumed >= 5)
            {
                SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -.3f }, NPC.position);
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<DevilishResin>());
                NPC.ai[3] = 1;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > 11 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.OverworldDayDesert.Chance * (spawnInfo.Player.ZoneBeach ? 0f : 0.3f);
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.DevilsTongue"))
            });
        }

        public override void OnKill()
        {
            for (int i = 0; i < Main.rand.Next(7, 10); i++)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Fly>());
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                int goreType = ModContent.Find<ModGore>("Redemption/DevilsTongueGore").Type;
                Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.position.Y), RedeHelper.SpreadUp(6), goreType);

                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DesertWater2,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);

                for (int i = 0; i < 12; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Cactus,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Cactus, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}