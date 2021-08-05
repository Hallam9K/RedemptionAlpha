using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.NPCs.Critters;

namespace Redemption
{
    public class BuffPlayer : ModPlayer
    {
        public bool infested;
        public int infestedTime;

        public override void ResetEffects()
        {
            infested = false;
            infestedTime = 0;
        }
        public override void UpdateBadLifeRegen()
        {
            if (infested)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= infestedTime / 60;
                Player.statDefense -= infestedTime / 120;
                if (infestedTime > 120)
                    Player.moveSpeed *= 0.8f;
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (infested)
            {
                r = Color.GreenYellow.R * 0.4f;
                g = Color.GreenYellow.G * 0.4f;
                b = Color.GreenYellow.B * 0.4f;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (infested && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " burst into larva!");
            }
            if (infested && infestedTime >= 60)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19, Player.position);
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex4 = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y), Player.width, Player.height, DustID.GreenBlood, Scale: 3f);
                    Main.dust[dustIndex4].velocity *= 5f;
                }
                int larvaCount = infestedTime / 120 + 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < MathHelper.Clamp(larvaCount, 1, 8); i++)
                        Projectile.NewProjectile(Player.GetProjectileSource_Buff(Player.FindBuffIndex(ModContent.BuffType<InfestedDebuff>())), Player.Center, RedeHelper.Spread(6), ModContent.ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
                }
            }
            return true;
        }
    }
}