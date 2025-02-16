using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class BileFlaskBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Weapon Imbue: Bile");
            // Description.SetDefault("Melee attacks inflict Burning Acid");
            Main.persistentBuff[Type] = true;
            Main.meleeBuff[Type] = true;
            BuffID.Sets.IsAFlaskBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BileEnchantPlayer>().bileWeaponImbue = true;
            player.meleeEnchant = 255;
        }
    }
    public class BileEnchantPlayer : ModPlayer
    {
        public bool bileWeaponImbue = false;

        public override void ResetEffects()
        {
            bileWeaponImbue = false;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (bileWeaponImbue && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(BuffType<BileDebuff>(), 60 * Main.rand.Next(3, 7));
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (bileWeaponImbue && (proj.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments)
            {
                target.AddBuff(BuffType<BileDebuff>(), 60 * Main.rand.Next(3, 7));
            }
        }

        // MeleeEffects and EmitEnchantmentVisualsAt apply the visual effects of the weapon imbue to items and projectiles respectively.
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (bileWeaponImbue && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GreenTorch);
                    dust.velocity *= 0.5f;
                }
            }
        }

        public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
        {
            if (bileWeaponImbue && (projectile.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.GreenTorch);
                    dust.velocity *= 0.5f;
                }
            }
        }
    }
}