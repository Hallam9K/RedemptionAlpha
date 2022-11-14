using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class OrePowder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mineral Powder");
            Tooltip.SetDefault("Converts basic ores into hardmode ores");
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.VilePowder);
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(0, 0, 25, 0);
            Item.shoot = ModContent.ProjectileType<OrePowder_Proj>();
        }
    }
    public class OrePowder_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mineral Powder");
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.VilePowder);
        }
        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
                Convert((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 2);

            Projectile.velocity *= 0.98f;
            int[] OreDust = new int[] { DustID.CopperCoin, DustID.SilverCoin, DustID.GoldCoin, DustID.PlatinumCoin };
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Utils.Next(Main.rand, OreDust));
        }
        public static void Convert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size))
                    {
                        int type = Main.tile[k, l].TileType;

                        if (type == TileID.Copper)
                            ConversionHandler.ConvertTile(k, l, TileID.Cobalt);
                        else if (type == TileID.Tin)
                            ConversionHandler.ConvertTile(k, l, TileID.Palladium);
                        else if (type == TileID.Iron)
                            ConversionHandler.ConvertTile(k, l, TileID.Cobalt);
                        else if (type == TileID.Lead)
                            ConversionHandler.ConvertTile(k, l, TileID.Palladium);
                        else if (type == TileID.Silver)
                            ConversionHandler.ConvertTile(k, l, TileID.Mythril);
                        else if (type == TileID.Tungsten)
                            ConversionHandler.ConvertTile(k, l, TileID.Orichalcum);
                        else if (type == TileID.Gold)
                            ConversionHandler.ConvertTile(k, l, TileID.Adamantite);
                        else if (type == TileID.Platinum)
                            ConversionHandler.ConvertTile(k, l, TileID.Titanium);
                    }
                }
            }
        }
    }
}