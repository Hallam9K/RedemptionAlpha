using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;
using Redemption.Rarities;
using Redemption.Textures;
using Redemption.Tiles.Furniture.Shade;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class CelestineDreamsong : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases vision within the Soulless Caverns" +
                "\nImmunity to the 'Soulless' debuff" +
                "\nAn aura of light surrounds you, damaging soulless enemies\n" +
                "'Those of us who hide a darkness see a different kind of light'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 15, 50, 0);
            Item.accessory = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<CelestineDreamsongTile>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Lighting.AddLight(player.Center, 1.1f, 1.1f, 1f);
            player.buffImmune[ModContent.BuffType<BlackenedHeartDebuff>()] = true;
            player.AddBuff(ModContent.BuffType<DreamsongBuff>(), 10);
            if (Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                int index = Dust.NewDust(new Vector2(player.position.X - player.velocity.X * 2f, player.position.Y - 2f - player.velocity.Y * 2f), player.width, player.height, DustID.AncientLight);
                Main.dust[index].noGravity = true;
                Dust dust = Main.dust[index];
                dust.velocity.X -= player.velocity.X * 0.5f;
                dust.velocity.Y -= player.velocity.Y * 0.5f;
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DreamsongLight_Visual>()] == 0)
                Projectile.NewProjectile(player.GetProjectileSource_Accessory(Item), player.position, Vector2.Zero, ModContent.ProjectileType<DreamsongLight_Visual>(), 0, 0, player.whoAmI);
        }
    }
}