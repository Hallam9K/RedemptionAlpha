using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Projectiles.Misc;

namespace Redemption.Buffs
{
    public class CrystalKnowledgeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Knowledge");
            // Description.SetDefault("Increased damage and resistance to the crystal's elements");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            int projType = ModContent.ProjectileType<ElementalCrystal>();
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[projType] > 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (!proj.active || proj.owner != player.whoAmI || proj.type != projType)
                            continue;

                        if (proj.localAI[0] is -1)
                            continue;

                        player.RedemptionPlayerBuff().ElementalDamage[(int)proj.ai[1]] += .04f;
                        player.RedemptionPlayerBuff().ElementalResistance[(int)proj.ai[1]] += .04f;
                    }
                }
                else
                {
                    player.DelBuff(buffIndex);
                    buffIndex--;
                }
            }
        }
    }
}