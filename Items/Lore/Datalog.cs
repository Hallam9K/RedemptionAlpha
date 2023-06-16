using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.Items.Lore
{
    public class Datalog : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Data Log #1");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog1"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
    public class Datalog2 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #2");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog2"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog3 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #3");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog3"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog4 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #6");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog4"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog5 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #335");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog5"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog6 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #722");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog6"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog7 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #919");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog7"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog8 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #180499");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog8"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog9 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #182500");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog9"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog10 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #182501");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog10"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog11 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #182573");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog11"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog12 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #184753");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog12"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog13 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #184989");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog13"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog14 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #466105");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog14"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog15 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #466476");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog15"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog16 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #500198");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog16"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog17 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #545675");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog17"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog18 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #999735");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog18"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog19 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #1000000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog19"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog20 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #1012875");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog20"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog21 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #3650000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog21"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog22 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #5385430");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog22"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog23 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #25338300");
        }
        public override void SetDefaults() => base.SetDefaults();


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog23"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog24 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #36500001");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog24"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog25 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #164550614");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog25"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog26 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #364635000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog26"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog27 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #365000000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog27"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class Datalog28 : Datalog
    {
        public override string Texture => "Redemption/Items/Lore/Datalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Data Log #389035250");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.Datalog28"))
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.KS3Datalog.DatalogViewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}
