using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Ranged;
using Redemption.Globals;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class CursedGrassBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Slain enemies burst into seeds that sprout brambles of thorns");
            Item.ResearchUnlockCount = 1;
            ElementID.ItemNature[Type] = true;
            ElementID.ItemPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(0, 0, 44, 0);
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Green;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            if (target.life <= 0 && target.lifeMax > 5)
            {
                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), ModContent.ProjectileType<ThornArrowSeed>(), hit.Damage, 3, player.whoAmI, 1);
            }
        }
    }
}
