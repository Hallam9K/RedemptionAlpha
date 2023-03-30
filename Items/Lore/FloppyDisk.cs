using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Lore
{
    public class FloppyDisk1 : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Floppy Disk");
            // Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class FloppyDisk2 : FloppyDisk1
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(1/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }
    }
    public class FloppyDisk2_1 : FloppyDisk1
    {
        public override string Texture => "Redemption/Items/Lore/FloppyDisk2";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(2/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }
    }
    public class FloppyDisk3 : FloppyDisk1
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(1/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }
    }
    public class FloppyDisk3_1 : FloppyDisk1
    {
        public override string Texture => "Redemption/Items/Lore/FloppyDisk3";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(2/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }
    }
    public class FloppyDisk5 : FloppyDisk1
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(1/4)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightRed;
        }
    }
    public class FloppyDisk5_1 : FloppyDisk1
    {
        public override string Texture => "Redemption/Items/Lore/FloppyDisk5";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(2/4)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightRed;
        }
    }
    public class FloppyDisk5_2 : FloppyDisk1
    {
        public override string Texture => "Redemption/Items/Lore/FloppyDisk5";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'" +
                "\n(3/4)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightRed;
        }
    }
    public class FloppyDisk5_3 : FloppyDisk1
    {
        public override string Texture => "Redemption/Items/Lore/FloppyDisk5";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'" +
                "\n(4/4)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightRed;
        }
    }
    public class FloppyDisk6 : FloppyDisk1
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(1/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
    }
    public class FloppyDisk6_1 : FloppyDisk1
    {
        public override string Texture => "Redemption/Items/Lore/FloppyDisk6";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(2/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
    }
    public class FloppyDisk7 : FloppyDisk1
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(1/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
        }
    }
    public class FloppyDisk7_1 : FloppyDisk1
    {
        public override string Texture => "Redemption/Items/Lore/FloppyDisk7";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            /* Tooltip.SetDefault("'A very old floppy disk. A T-Bot might be able to decode the data...'"
            + "\n(2/2)"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
        }
    }
}
