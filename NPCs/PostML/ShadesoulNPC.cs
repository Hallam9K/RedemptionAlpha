using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Materials.PostML;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.PostML
{
    public class ShadesoulNPC : ModNPC
    {
        public ref float Scale => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];
        public ref float ThrowTimer => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadesoul");
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new ()
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 1;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.lavaImmune = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.catchItem = (short)ModContent.ItemType<Shadesoul>();
        }
        public override bool? CanBeCaughtBy(Item item, Player player)
        {
            if (player.RedemptionAbility().SpiritwalkerActive)
                return null;
            return false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2 + Scale);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }
        public override void AI()
        {
            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(),
                0, 0, Scale: 2 + Scale);
            Main.dust[dust].velocity *= 0;
            Main.dust[dust].noGravity = true;

            NPC.scale = 1 + Scale;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;

            if (NPC.velocity.Length() > 3)
                NPC.velocity *= 0.98f;

            if (--ThrowTimer <= 0)
            {
                if (NPC.velocity.Length() < 2)
                    NPC.velocity = RedeHelper.PolarVector(3, RedeHelper.RandomRotation());

                NPC.velocity = NPC.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            }

            if (AITimer < 30)
                NPC.dontTakeDamage = true;
            else
                NPC.dontTakeDamage = false;

            if (++AITimer > 600)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath39 with { Volume = .3f }, NPC.position);
                NPC.active = false;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 7 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 15);
        }
        public override bool? CanBeHitByItem(Player player, Item item) => RedeHelper.CanHitSpiritCheck(player, item);
        public override bool? CanBeHitByProjectile(Projectile projectile) => RedeHelper.CanHitSpiritCheck(projectile);
        public override void OnKill()
        {
            int dropAmount = (int)(Scale / 2 * 10);
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Shadesoul>(), 1 + dropAmount);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement("Willpower exists in Epidotra - the happier or more you desire life, the stronger and bigger your soul can become, and vice versa. If someone loses their will to live, their soul can become so small it inverts and they become soulless. The soulless only emerged after the Age of the False Gods.")
            });
        }
    }
}