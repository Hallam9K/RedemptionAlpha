using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
	public class StarlightPrismDebuff : ModBuff
	{
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prism");
			Description.SetDefault("You shouldn't see this!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
		public override void Update(NPC npc, ref int buffIndex)
		{
			Dust val = Main.dust[Dust.NewDust(((Entity)npc).position, ((Entity)npc).width, ((Entity)npc).height, 261, 0f, 0f, 0, default(Color), 1f)];
		}
	}
}
