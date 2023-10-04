using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Buffs;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public abstract class SSBase : ModNPC
    {
        public virtual void SetSafeStaticDefaults() { }
        public override void SetStaticDefaults()
        {
            SetSafeStaticDefaults();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public virtual void SetSafeDefaults() { }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.Redemption().spiritSummon = true;
            SetSafeDefaults();
        }
        public override bool CheckActive() => false;
        public override bool PreAI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            if (!player.active || player.dead || !CheckActive(player))
                NPC.SimpleStrikeNPC(999, 1);

            if (Main.myPlayer == player.whoAmI && NPC.DistanceSQ(player.Center) > 2000 * 2000)
            {
                NPC.Center = player.Center;
                NPC.velocity *= 0.1f;
                NPC.netUpdate = true;
            }
            if (!Main.rand.NextBool(40))
                return true;
            ParticleManager.NewParticle(NPC.RandAreaInEntity(), RedeHelper.Spread(2), new SpiritParticle(), Color.White, 1);
            return true;
        }
        public static bool CheckActive(Player owner)
        {
            if (!owner.HasBuff(ModContent.BuffType<CruxCardBuff>()))
                return false;
            return true;
        }
    }
}