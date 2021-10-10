using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Lore
{
    public class Datalog : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Data Log #1");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I have successfully reached the outer atmosphere and escaped my world's Reset."
                + "\nI realise there is no going back now, a normal human's lifespan sounds miniscule"
                + "\ncompared to the time I must travel through space, but it is my goal to withstand this infinite voyage."
                + "\nI just hope it'll all be worth it when I return to the new world. I've decided to write these"
                + "\nlogs every day until I return, and preserve my encounters for when I get back."
                + "\nBut that's a million years from now. I just hope I won't regret it.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #2");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'If I ever forget why I'm doing this, I will write it down here."
                + "\nWhen a Great Era ends, all life dies and the world resets. I am from a previous era"
                + "\nand successfully escaped into space, as the reset won't affect me outside of the world."
                + "\nA reset apparently takes a million years, so I must travel through space during that time period,"
                + "\nand with luck, I should come back here a million years from now, and see the new world in all it's beauty."
                + "\nAs far as I know, I am the sole survivor, and the first living thing to ever escape."
                + "\nI transferred my human mind into a robotic body so I can save an infinite number of memories with ease,"
                + "\nand I won't need to worry about thirst, hunger, or sleep.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #3");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Strange."
                + "\nI'm starting to get symptoms of thirst and tiredness..."
                + "\nThis shouldn't be happening, as a robot, I shouldn't require these basic human needs!"
                + "\nIf for whatever reason these needs still affect me, this could make this voyage even worse."
                + "\nI can't go to sleep without eyes, I can't drink without a mouth, I can't eat without a digestive system..."
                + "\nSo I have no way of stopping these symptoms."
                + "\nI'm still 2 years from reaching the nearest planet - Nabu III"
                + "\nActually, I should come up with a new name for my robotic body... Something other than Survival Robot Mk. 78.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #6");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'My worst fear is coming true."
                + "\nI have a strong feeling of tiredness and thirst, and my hunger has begun to take hold."
                + "\nIt's only been 6 days damn it! Roughly 359,000,000 days to go..."
                + "\nI've tried all I can, but these painful feelings can't go away."
                + "\nThe human mind is more complicated than I imagined, and combined with all this technical stuff"
                + "\nonly makes it harder for me to look into it!"
                + "\nIf only I had more time back then! I could've looked through this body's code and easily discovered the error!"
                + "\nGuess I'll just have to deal with it.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #335");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'About a year to go until I reach Nabu III."
                + "\nI am STILL dealing with these damn feelings of hunger and tiredness,"
                + "\nand have only been getting worse from here. Humans can last 11 days without sleep,"
                + "\nthe only thing that stopped them from feeling worse was death."
                + "\nI can't even bloody die from fatigue, I can't starve to death either,"
                + "\nI'm just stuck like this... For a million damn years!'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #722");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Finally!"
                + "\nI am close enough to Nabu III to get a scan of the planet."
                + "\nSeems to be a standard ocean planet with a radius of 5605.77km."
                + "\n35.3% iron, 32.0% oxygen, 15.1% silicon, 6.1% sodium, 4.2% aluminum, 2.7% other metals, 4.7% other elements."
                + "\nGravitational pull is 7.84 m/s², less than my planet, but whatever..."
                + "\nA cycle lasts 23.89 hours with an axis tilt of 28.37°."
                + "\n80% ice sheets, 19.8% ocean, 0.2% land. Atmospheric pressure of 91.83 kPa."
                + "\nThe atmosphere is 78.4% sulphur dioxide, and the average temperature is -10°C."
                + "\nBasically, an uninhabitable frozen planet... Great.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #919");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Alright. I have constructed a temporary base on Nabu III."
                + "\nThe amount of iron and sulfur here has come in handy."
                + "\nI mean, if I was a human I'd be dead with the lack of proper air."
                + "\nMy blueprint for a country sized spaceship is also finished, now begins the long construction."
                + "\nThe design will be a crescent moon shape, not sure why..."
                + "\nProbably because I used to look up at the moon of my world a lot"
                + "\nwhen I was human... I wish I still was.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #180499");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'My god how have I not died from this pain yet,"
                + "\nit just keeps growing. Whenever I think it can't get any worse the next day it does!"
                + "\nOn brighter news my bigass spaceship is finished. Now I can leave this planet."
                + "\nI'm getting real sick of snow, the old world was nothing but snow as well, I just want some greenery for once."
                + "\nUnfortunately my next planet is even further from the sun so I'm not really hopeful...'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #182500");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Today marks 500 years away from home."
                + "\nIt took 5.5 years but I'm at the next planet - Alkonost. I expected to get there faster,"
                + "\nbut I REALLY underestimated the amount of fuel this giant spaceship would consume."
                + "\nI'm certain I wouldn't have made that mistake if I just didn't feel so terrible!"
                + "\nLiving like this is absolute hell. I'll write down this planet's statistics next data log.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #182501");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Alright. Alkonost. And of course, it's ANOTHER DAMN ICE PLANET!"
                + "\nRadius: 6059.58km, Composition: 36.1% titanium, 35.6% iron, 17.5% oxygen,"
                + "\n7.4% silicon, 3.4% other metals, trace other elements. High amounts of titanium, huh?"
                + "\nThat's gonna be useful, Nabu III had barely any titanium."
                + "\nGravity is 11.13 m/s². A cycle is 32.65 hours, with an axis tilt of 11.58°."
                + "\nOh god. 100% of the surface is just ice. The atmosphere is toxic, with a pressure of 91.63 kPa."
                + "\nThe temperature is -223°C... I don't think even my robotic body can handle that! Oh whatever.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #182573");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Holy..."
                + "\nAfter exploring Alkonost's surface, I've finally found something other than ice!"
                + "\nTook so long since I can't last down there for more than half a minute."
                + "\nFrom the looks of things, it looks man-made. Or I guess alien-made... Hehe."
                + "\nFirst time I've felt this amused in forever, but anyways, the structure."
                + "\nIt was under the thick ice sheet so I had to drill quite far down."
                + "\nThe water under there must be freezing, but curiosity is getting the better of me here."
                + "\nI have found an entrance, inside is just as cold though, so I should go back up into my ship before exploring further.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #184753");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I have basically harvested this planet's titanium dry."
                + "\nThe alien tech I found in that strange structure has come in handy,"
                + "\nI've augmented it into my spaceship's thrusters, so I can reach planets much faster."
                + "\nHmm... I should give the ship a name... Well, I called robot-self King Slayer III,"
                + "\nso the ship must be just as cool. How about: Ship of the Slayer! Or SoS for short?"
                + "\nWell, it's finally time to explore beyond the Vorti Star System."
                + "\nI have 999,493.8 years to go... And my overwhelming pain still hasn't settled."
                + "\nI'll just have to live with it forever now.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #184989");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'On my journey to the nearest solar system, I decided to dabble with AI."
                + "\nI have set up blueprints for a simple android with the purpose of"
                + "\nmaintaining the SoS while I'm away. It's something for me to do so why not."
                + "\nIt's estimated to take 770 years to reach the next solar system,"
                + "\nand I haven't encountered another moving thing for 506 years."
                + "\nHaving robots going about the SoS would be nice, and I'll be less lonely.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #466105");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Welp, I've reached the next solar system."
                + "\n3 planets have been scanned, which is quite a disappointment..."
                + "\nI was hoping for there to be more so I can have more to do."
                + "\nBut it's fine I guess, the androids I've created have been keeping me company."
                + "\nI'll go to the planet nearest the habitable zone, 'cos robots have become pretty boring now,"
                + "\nand I'm dying to see actual greenery, not some dull frozen wasteland.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #466476");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I have named this planet Asherah, it appears to be iron/silicate-based."
                + "\nA big radius of 8845.27 km, 40.8% iron, 32.9% oxygen, 15.6% silicon, 4.2% carbon, 2.9% magnesium..."
                + "\nQuite strong gravity, 34.70 hour cycle, an axis tilt of 53.09°..."
                + "\nOnly 1% is water, the rest looks like... boring stone and sand... Damn."
                + "\nOh! The scanner has found life! Microbes, fungi, sentient animals... What is that?"
                + "\nWell I've found life here, only problem is they look ugly as hell."
                + "\n2.01 million of these intelligent creatures have been scanned, so they've been around for a while.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #500198");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I'm done with Asherah, the aliens there attacked me so I had to make some weaponry."
                + "\nI decided to make a new android, one for military purposes, I've named it the Prototype Silver."
                + "\nDespite its name, it's mainly composed of the spare titanium from Alkonost."
                + "\nI did find a metric ton of coal from Asherah's caverns, so that's nice."
                + "\nWell... Onto the next planet, I just hope THIS one will be lush and green."
                + "\nAll the planets I've been to were either frozen or barren.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #545675");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Wow, this planet blew my expectations away..."
                + "\nI have named it Alatar V. It's very small, and on the surface it just looks barren."
                + "\nHowever, it's cave systems are beautiful. Like, there's so many colours and valuable ores."
                + "\nI've been exploring them overtime for probably years now. But that's fine,"
                + "\nNot like I got anything better to do."
                + "\nI'll be leaving this planet soon and moving onto the next solar system.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #999735");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I haven't done one of these in forever, but I should explain what happened."
                + "\nI was travelling to the next solar system, when suddenly some wormhole appeared."
                + "\nThe SoS couldn't turn fast enough so it got sucked in. Wormholes are like portals of the universe,"
                + "\nso I expected to just reach the end instantly, but no, I was stuck in the wormhole for almost 1000 years."
                + "\nGod it was boring, but I had the androids to keep me company. Unfortunately, I don't know where I am now."
                + "\nI can't tell how far away I am from home, but I see a nearby star, so hopefully there's some planets.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #1000000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Today is the millionth day in space. When I was writing that down,"
                + "\nI had a dumb moment where I thought it was a million years... But no."
                + "\nIt has only been 2739.7 years, so... 364,000,000? days left... It feels like forever,"
                + "\nand yet it's only been 0.27% of a million. Why am I still doing this. What's the point anymore?"
                + "\nEvery day is a pain, I just want to eat, I want to sleep..."
                + "\nI would say I want to be human again, but to be honest... I don't even want to be alive anymore.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #1012875");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'About damn time. I've finally found a green planet. But am I happy about this?"
                + "\nNot really. I thought this would make me feel something, but it's hopeless."
                + "\nI can't remember the last time I felt happy, the memory of my home is starting to get foggy."
                + "\nBut anyway, it looks like this planet has intelligent life, so I'll land and see if they're friendly."
                + "\nIf they're not, I'll just shoot them. Simple.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #3650000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Today is the 10,000th year in space, only 1% of a million..."
                + "\nI feel like living beings shouldn't be allowed to live for this long."
                + "\nA hundred years for a human is forever, and I've been around for 100x that!"
                + "\nI've redesigned my robotic body again, but I still haven't figured out how to get into my"
                + "\nhuman mind and get rid of this STUPID HUNGER. I DON'T HAVE A STOMACH, WHY AM I HUNGRY!?"
                + "\nI DON'T HAVE EYES, WHY AM I SO TIRED!? WHY DO I HAVE TO LIVE THROUGH THIS DAMN IT!?'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #5385430");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'The SoS got attacked by some Space Pirates."
                + "\nNot like I care, I destroyed their ships so what can they do now?"
                + "\nThe SoS's scanner picked up a lifeform in the engine room, so I should probably check it out."
                + "\nI can't be asked to do anything really. But whatever.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #25338300");
        }
        public override void SetDefaults() => base.SetDefaults();


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Nice'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #36500001");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'It's been 10% of a million years now. Yay."
                + "\nI accidently skipped a day in the data logs so 10% was really yesterday, but not like I care."
                + "\nI have explored... 2853 planets now, and they are starting to all look the same."
                + "\nI'm sick of ice planets, sick of lush green ones, sick of barren ones... I guess I'm not satisfied anymore.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #164550614");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I haven't felt like this in forever."
                + "\nI upgraded my robotic self again, this time with more attack power. Xehito let me test it on him,"
                + "\nso we had a fight. The intensity of it was almost exhilarating, lasers firing everywhere,"
                + "\nexplosions all around, it was generally a fun time. But something tells me he only let me"
                + "\nfight him to cheer me up, and I'm sorry Xehito, but that moment didn't last, I still feel empty.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #364635000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Only 1000 years until a million, and I can return home."
                + "\nI've already set my course, but the problem is, because of that wormhole,"
                + "\nI don't know which way is home... All I can do now is go to a random direction"
                + "\nand hope for the best. But the galaxy is vast, I fear by the time I reach home again,"
                + "\nThe next reset would've already started, and I'd have to wait another million years..."
                + "\nIf that happens, I won't try anymore, I'll just give up.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #365000000");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Today is the millionth year in space!"
                + "\nUnfortunately, I won't be able to see the new Great Era for some time."
                + "\nI'm still lost in space.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
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
            DisplayName.SetDefault("Data Log #389035250");
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I've made it back, I'm home again.'")
                {
                    overrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    overrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}