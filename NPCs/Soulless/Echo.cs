using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Redemption.Globals;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;

namespace Redemption.NPCs.Soulless
{
    public class Echo : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Echo");
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f, };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.width = 16;
            NPC.height = 32;
            NPC.rarity = 2;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 0;
            NPC.chaseable = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            NPC.LookByVelocity();
            NPC.Move(player.Center, 2, 40);
            if (NPC.Distance(player.Center) < 200)
                NPC.ai[0] = 1;

            if (NPC.ai[0] == 1)
            {
                NPC.alpha += 4;
                if (NPC.alpha >= 255)
                    NPC.active = false;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= 9 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return ItemTags.Arcane.Has(item.type) || ItemTags.Celestial.Has(item.type) || ItemTags.Holy.Has(item.type) ||
                ItemTags.Psychic.Has(item.type) || RedeConfigClient.Instance.ElementDisable ? null : false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return ProjectileTags.Arcane.Has(projectile.type) || ProjectileTags.Celestial.Has(projectile.type) || ProjectileTags.Holy.Has(projectile.type) ||
                ProjectileTags.Psychic.Has(projectile.type) || RedeConfigClient.Instance.ElementDisable;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "The origin of these spirits is unknown. Despite the other soulless' hunger for essence, they seldom notice these miniscule echos.")
            });
        }
    }
}