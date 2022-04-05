using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using Terraria.DataStructures;

namespace Redemption.NPCs.Soulless
{
    public class SoullessMarionette_Doll : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulless Marionette");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 4950;
            NPC.damage = 90;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 36;
            NPC.height = 92;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.NPCDeath50;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), Scale: 2);
                    Main.dust[dustIndex2].velocity *= 2.6f;
                }
            }
        }
        public override bool? CanBeHitByItem(Player player, Item item) => !NPC.AnyNPCs(ModContent.NPCType<SoullessMarionette_Cross>());
        public override bool? CanBeHitByProjectile(Projectile projectile) => !NPC.AnyNPCs(ModContent.NPCType<SoullessMarionette_Cross>());
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest(true);

            NPC.LookAtEntity(player);
            float speed = 14;
            if (NPC.AnyNPCs(ModContent.NPCType<SoullessMarionette_Cross>()))
                speed = 2;

            NPC.Move(player.Center, speed, 40);
            if (NPC.ai[1] == 0)
            {
                RedeHelper.SpawnNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 20, ModContent.NPCType<SoullessMarionette_Cross>(), NPC.whoAmI);
                NPC.ai[1] = 1;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 7)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("A puppet on strings, bound by a cross. It wants to be free. Will you give it freedom? Will you cut the master's strings that hold their souls in stasis?"),
                //new DynamicFlavorTextBestiaryInfoElement("Now with the strings cut, these souls are no longer bound by the Ventriloquist. They are free to roam beyond the prison.")
            });
        }
    }
    public class DynamicFlavorTextBestiaryInfoElement : FlavorTextBestiaryInfoElement, IBestiaryInfoElement
    {
        public DynamicFlavorTextBestiaryInfoElement(string languageKey) : base(languageKey)
        {
        }

        public new UIElement ProvideUIElement(BestiaryUICollectionInfo info)
        {
            if (Main.rand.NextBool(2)) // TODO: Extra entry for defeat of Void Ventriloquist
            {
                return base.ProvideUIElement(info);
            }
            return null;
        }
    }
}