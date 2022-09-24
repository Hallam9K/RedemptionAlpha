using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Items.Materials.PreHM;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Redemption.BaseExtension;
using ParticleLibrary;
using Redemption.Particles;

namespace Redemption.NPCs.Friendly
{
    public class LostSoulNPC : ModNPC
    {
        public ref float Scale => ref NPC.ai[0];

        public ref float AITimer => ref NPC.ai[1];

        public ref float ThrowTimer => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lost Soul");
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 10;
            NPC.height = 10;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.lavaImmune = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.alpha = 200;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.catchItem = (short)ModContent.ItemType<LostSoul>();
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2 + Scale);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return player.RedemptionAbility().SpiritwalkerActive || ItemLists.Arcane.Contains(item.type) || ItemLists.Celestial.Contains(item.type) || ItemLists.Holy.Contains(item.type) || ItemLists.Psychic.Contains(item.type) || RedeConfigClient.Instance.ElementDisable ? null : false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            return player.RedemptionAbility().SpiritwalkerActive || ProjectileLists.Arcane.Contains(projectile.type) || ProjectileLists.Celestial.Contains(projectile.type) || ProjectileLists.Holy.Contains(projectile.type) || ProjectileLists.Psychic.Contains(projectile.type) || RedeConfigClient.Instance.ElementDisable;
        }
        public override void AI()
        {
            ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(NPC) + (NPC.velocity * 10), Vector2.Zero, new SpiritParticle(), Color.White, 0.6f * NPC.scale, 0, 1);

            NPC.scale = 1 + Scale;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;

            if (NPC.velocity.Length() > 3)
                NPC.velocity *= 0.98f;

            if (--ThrowTimer <= 0)
            {
                if (NPC.velocity.Length() < 2)
                    NPC.velocity = RedeHelper.PolarVector(3, Main.rand.NextFloat(0, MathHelper.TwoPi));

                NPC.velocity = NPC.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            }

            if (++AITimer > 600 && !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath39 with { Volume = .3f }, NPC.position);
                NPC.active = false;
            }
        }
        public override void OnKill()
        {
            int dropAmount = (int)(Scale / 2 * 10);
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<LostSoul>(), 1 + dropAmount);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.RedemptionAbility().SpiritwalkerActive && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
                return 2f;
            return 0;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(
                    "Lost Souls search around the world to look for corpses to infuse with. They roam catacombs and graveyards, sometimes taking many days to find a compatible vessel.")
            });
        }
    }
}