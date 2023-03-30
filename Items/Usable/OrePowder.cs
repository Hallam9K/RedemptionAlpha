using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class OrePowder : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magic Mineral Powder");
            // Tooltip.SetDefault("Converts basic ores into hardmode ores");
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.VilePowder);
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(0, 0, 25, 0);
            Item.shoot = ModContent.ProjectileType<OrePowder_Proj>();
            Item.shootSpeed = 11;
        }
    }
    public class OrePowder_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magic Mineral Powder");
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.VilePowder);
            Projectile.width += 8;
            Projectile.height += 8;
            Projectile.timeLeft = 60;
        }
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
                Convert((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 2);

            int[] OreDust = new int[] { DustID.CopperCoin, DustID.SilverCoin, DustID.GoldCoin, DustID.PlatinumCoin };
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Utils.Next(Main.rand, OreDust), Scale: 1);
                Main.dust[d].velocity *= 0.3f;
                Main.dust[d].noGravity = true;
            }
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