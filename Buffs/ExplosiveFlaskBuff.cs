using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class ExplosiveFlaskBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Weapon Imbue: Nitroglycerine");
            // Description.SetDefault("Melee attacks gain the Explosive bonus");
            Main.persistentBuff[Type] = true;
            BuffID.Sets.IsAFlaskBuff[Type] = true;
            Main.meleeBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ExplosiveEnchantPlayer>().explosiveWeaponImbue = true;
            player.meleeEnchant = 255;
        }
    }
    public class ExplosiveEnchantPlayer : ModPlayer
    {
        public bool explosiveWeaponImbue = false;

        public override void ResetEffects()
        {
            explosiveWeaponImbue = false;
        }

        // MeleeEffects and EmitEnchantmentVisualsAt apply the visual effects of the weapon imbue to items and projectiles respectively.
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (explosiveWeaponImbue && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Smoke);
                    dust.velocity *= 0.5f;
                }
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.InfernoFork);
                    dust.velocity *= 0.5f;
                }
            }
        }

        public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
        {
            if (explosiveWeaponImbue && (projectile.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.Smoke);
                    dust.velocity *= 0.5f;
                }
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.InfernoFork);
                    dust.velocity *= 0.5f;
                }
            }
        }
    }
}