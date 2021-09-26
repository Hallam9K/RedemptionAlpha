using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Lore
{
    public class Datalog : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Data Log #1");
            Tooltip.SetDefault("It reads - [c/aee6f3:'I have successfully reached the outer atmosphere and escaped my world's Reset.]"
                + "\n[c/aee6f3:I realise there is no going back now, a normal human's lifespan sounds miniscule]"
                + "\n[c/aee6f3:compared to the time I must travel through space, but it is my goal to withstand this infinite voyage.]"
                + "\n[c/aee6f3:I just hope it'll all be worth it when I return to the new world. I've decided to write these]"
                + "\n[c/aee6f3:logs every day until I return, and preserve my encounters for when I get back.]"
                + "\n[c/aee6f3:But that's a million years from now. I just hope I won't regret it.']");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.Cyan;
        }

        public class Datalog2 : ModItem
        {
            public override string Texture => "Redemption/Items/Lore/Datalog";

            public override void SetStaticDefaults()
            {
                DisplayName.SetDefault("Data Log #2");
                Tooltip.SetDefault("It reads - [c/aee6f3:'If I ever forget why I'm doing this, I will write it down here.]"
                    + "\n[c/aee6f3:When a Great Era ends, all life dies and the world resets. I am from a previous era]"
                    + "\n[c/aee6f3:and successfully escaped into space, as the reset won't affect me outside of the world.]"
                    + "\n[c/aee6f3:A reset apparently takes a million years, so I must travel through space during that time period,]"
                    + "\n[c/aee6f3:and with luck, I should come back here a million years from now, and see the new world in all it's beauty.]"
                    + "\n[c/aee6f3:As far as I know, I am the sole survivor, and the first living thing to ever escape.]"
                    + "\n[c/aee6f3:I transferred my human mind into a robotic body so I can save an infinite number of memories with ease,]"
                    + "\n[c/aee6f3:and I won't need to worry about thirst, hunger, or sleep.']");
                Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
            }

            public override void SetDefaults()
            {
                Item.width = 34;
                Item.height = 30;
                Item.maxStack = 1;
                Item.value = 0;
                Item.rare = ItemRarityID.Cyan;
            }
        }

        public class Datalog3 : ModItem
        {
            public override string Texture => "Redemption/Items/Lore/Datalog";

            public override void SetStaticDefaults()
            {
                DisplayName.SetDefault("Data Log #3");
                Tooltip.SetDefault("It reads - [c/aee6f3:'Strange.]"
                    + "\n[c/aee6f3:I'm starting to get symptoms of thirst and tiredness...]"
                    + "\n[c/aee6f3:This shouldn't be happening, as a robot, I shouldn't require these basic human needs!]"
                    + "\n[c/aee6f3:If for whatever reason these needs still affect me, this could make this voyage even worse.]"
                    + "\n[c/aee6f3:I can't go to sleep without eyes, I can't drink without a mouth, I can't eat without a digestive system...]"
                    + "\n[c/aee6f3:So I have no way of stopping these symptoms.]"
                    + "\n[c/aee6f3:I'm still 2 years from reaching the nearest planet - Nabu III]"
                    + "\n[c/aee6f3:Actually, I should come up with a new name for my robotic body... Something other than Survival Robot Mk. 78.']");
                Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
            }

            public override void SetDefaults()
            {
                Item.width = 34;
                Item.height = 30;
                Item.maxStack = 1;
                Item.value = 0;
                Item.rare = ItemRarityID.Cyan;
            }

            public class Datalog4 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #6");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'My worst fear is coming true.]"
                        + "\n[c/aee6f3:I have a strong feeling of tiredness and thirst, and my hunger has begun to take hold.]"
                        + "\n[c/aee6f3:It's only been 6 days damn it! Roughly 359,000,000 days to go...]"
                        + "\n[c/aee6f3:I've tried all I can, but these painful feelings can't go away.]"
                        + "\n[c/aee6f3:The human mind is more complicated than I imagined, and combined with all this technical stuff]"
                        + "\n[c/aee6f3:only makes it harder for me to look into it!]"
                        + "\n[c/aee6f3:If only I had more time back then! I could've looked through this body's code and easily discovered the error!]"
                        + "\n[c/aee6f3:Guess I'll just have to deal with it.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog5 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #335");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'About a year to go until I reach Nabu III.]"
                        + "\n[c/aee6f3:I am STILL dealing with these damn feelings of hunger and tiredness,]"
                        + "\n[c/aee6f3:and have only been getting worse from here. Humans can last 11 days without sleep,]"
                        + "\n[c/aee6f3:the only thing that stopped them from feeling worse was death.]"
                        + "\n[c/aee6f3:I can't even bloody die from fatigue, I can't starve to death either,]"
                        + "\n[c/aee6f3:I'm just stuck like this... For a million damn years!']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog6 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #722");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Finally!]"
                        + "\n[c/aee6f3:I am close enough to Nabu III to get a scan of the planet.]"
                        + "\n[c/aee6f3:Seems to be a standard ocean planet with a radius of 5605.77km.]"
                        + "\n[c/aee6f3:35.3% iron, 32.0% oxygen, 15.1% silicon, 6.1% sodium, 4.2% aluminum, 2.7% other metals, 4.7% other elements.]"
                        + "\n[c/aee6f3:Gravitational pull is 7.84 m/s², less than my planet, but whatever...]"
                        + "\n[c/aee6f3:A cycle lasts 23.89 hours with an axis tilt of 28.37°.]"
                        + "\n[c/aee6f3:80% ice sheets, 19.8% ocean, 0.2% land. Atmospheric pressure of 91.83 kPa.]"
                        + "\n[c/aee6f3:The atmosphere is 78.4% sulphur dioxide, and the average temperature is -10°C.]"
                        + "\n[c/aee6f3:Basically, an uninhabitable frozen planet... Great.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog7 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #919");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Alright. I have constructed a temporary base on Nabu III.]"
                        + "\n[c/aee6f3:The amount of iron and sulfur here has come in handy.]"
                        + "\n[c/aee6f3:I mean, if I was a human I'd be dead with the lack of proper air.]"
                        + "\n[c/aee6f3:My blueprint for a country sized spaceship is also finished, now begins the long construction.]"
                        + "\n[c/aee6f3:The design will be a crescent moon shape, not sure why...]"
                        + "\n[c/aee6f3:Probably because I used to look up at the moon of my world a lot]"
                        + "\n[c/aee6f3:when I was human... I wish I still was.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog8 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #180499");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'My god how have I not died from this pain yet,]"
                        + "\n[c/aee6f3:it just keeps growing. Whenever I think it can't get any worse the next day it does!]"
                        + "\n[c/aee6f3:On brighter news my bigass spaceship is finished. Now I can leave this planet.]"
                        + "\n[c/aee6f3:I'm getting real sick of snow, the old world was nothing but snow as well, I just want some greenery for once.]"
                        + "\n[c/aee6f3:Unfortunately my next planet is even further from the sun so I'm not really hopeful...']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog9 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #182500");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Today marks 500 years away from home.]"
                        + "\n[c/aee6f3:It took 5.5 years but I'm at the next planet - Alkonost. I expected to get there faster,]"
                        + "\n[c/aee6f3:but I REALLY underestimated the amount of fuel this giant spaceship would consume.]"
                        + "\n[c/aee6f3:I'm certain I wouldn't have made that mistake if I just didn't feel so terrible!]"
                        + "\n[c/aee6f3:Living like this is absolute hell. I'll write down this planet's statistics next data log.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog10 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #182501");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Alright. Alkonost. And of course, it's ANOTHER DAMN ICE PLANET!]"
                        + "\n[c/aee6f3:Radius: 6059.58km, Composition: 36.1% titanium, 35.6% iron, 17.5% oxygen,]"
                        + "\n[c/aee6f3:7.4% silicon, 3.4% other metals, trace other elements. High amounts of titanium, huh?]"
                        + "\n[c/aee6f3:That's gonna be useful, Nabu III had barely any titanium.]"
                        + "\n[c/aee6f3:Gravity is 11.13 m/s². A cycle is 32.65 hours, with an axis tilt of 11.58°.]"
                        + "\n[c/aee6f3:Oh god. 100% of the surface is just ice. The atmosphere is toxic, with a pressure of 91.63 kPa.]"
                        + "\n[c/aee6f3:The temperature is -223°C... I don't think even my robotic body can handle that! Oh whatever.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog11 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #182573");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Holy...]"
                        + "\n[c/aee6f3:After exploring Alkonost's surface, I've finally found something other than ice!]"
                        + "\n[c/aee6f3:Took so long since I can't last down there for more than half a minute.]"
                        + "\n[c/aee6f3:From the looks of things, it looks man-made. Or I guess alien-made... Hehe.]"
                        + "\n[c/aee6f3:First time I've felt this amused in forever, but anyways, the structure.]"
                        + "\n[c/aee6f3:It was under the thick ice sheet so I had to drill quite far down.]"
                        + "\n[c/aee6f3:The water under there must be freezing, but curiosity is getting the better of me here.]"
                        + "\n[c/aee6f3:I have found an entrance, inside is just as cold though, so I should go back up into my ship before exploring further.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog12 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #184753");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'I have basically harvested this planet's titanium dry.]"
                        + "\n[c/aee6f3:The alien tech I found in that strange structure has come in handy,]"
                        + "\n[c/aee6f3:I've augmented it into my spaceship's thrusters, so I can reach planets much faster.]"
                        + "\n[c/aee6f3:Hmm... I should give the ship a name... Well, I called robot-self King Slayer III,]"
                        + "\n[c/aee6f3:so the ship must be just as cool. How about: Ship of the Slayer! Or SoS for short?]"
                        + "\n[c/aee6f3:Well, it's finally time to explore beyond the Vorti Star System.]"
                        + "\n[c/aee6f3:I have 999,493.8 years to go... And my overwhelming pain still hasn't settled.]"
                        + "\n[c/aee6f3:I'll just have to live with it forever now.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog13 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #184989");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'On my journey to the nearest solar system, I decided to dabble with AI.]"
                        + "\n[c/aee6f3:I have set up blueprints for a simple android with the purpose of]"
                        + "\n[c/aee6f3:maintaining the SoS while I'm away. It's something for me to do so why not.]"
                        + "\n[c/aee6f3:It's estimated to take 770 years to reach the next solar system,]"
                        + "\n[c/aee6f3:and I haven't encountered another moving thing for 506 years.]"
                        + "\n[c/aee6f3:Having robots going about the SoS would be nice, and I'll be less lonely.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog14 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #466105");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Welp, I've reached the next solar system.]"
                        + "\n[c/aee6f3:3 planets have been scanned, which is quite a disappointment...]"
                        + "\n[c/aee6f3:I was hoping for there to be more so I can have more to do.]"
                        + "\n[c/aee6f3:But it's fine I guess, the androids I've created have been keeping me company.]"
                        + "\n[c/aee6f3:I'll go to the planet nearest the habitable zone, 'cos robots have become pretty boring now,]"
                        + "\n[c/aee6f3:and I'm dying to see actual greenery, not some dull frozen wasteland.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog15 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #466476");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'I have named this planet Asherah, it appears to be iron/silicate-based.]"
                        + "\n[c/aee6f3:A big radius of 8845.27 km, 40.8% iron, 32.9% oxygen, 15.6% silicon, 4.2% carbon, 2.9% magnesium...]"
                        + "\n[c/aee6f3:Quite strong gravity, 34.70 hour cycle, an axis tilt of 53.09°...]"
                        + "\n[c/aee6f3:Only 1% is water, the rest looks like... boring stone and sand... Damn.]"
                        + "\n[c/aee6f3:Oh! The scanner has found life! Microbes, fungi, sentient animals... What is that?]"
                        + "\n[c/aee6f3:Well I've found life here, only problem is they look ugly as hell.]"
                        + "\n[c/aee6f3:2.01 million of these intelligent creatures have been scanned, so they've been around for a while.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog16 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #500198");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'I'm done with Asherah, the aliens there attacked me so I had to make some weaponry.]"
                        + "\n[c/aee6f3:I decided to make a new android, one for military purposes, I've named it the Prototype Silver.]"
                        + "\n[c/aee6f3:Despite its name, it's mainly composed of the spare titanium from Alkonost.]"
                        + "\n[c/aee6f3:I did find a metric ton of coal from Asherah's caverns, so that's nice.]"
                        + "\n[c/aee6f3:Well... Onto the next planet, I just hope THIS one will be lush and green.]"
                        + "\n[c/aee6f3:All the planets I've been to were either frozen or barren.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog17 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #545675");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Wow, this planet blew my expectations away...]"
                        + "\n[c/aee6f3:I have named it Alatar V. It's very small, and on the surface it just looks barren.]"
                        + "\n[c/aee6f3:However, it's cave systems are beautiful. Like, there's so many colours and valuable ores.]"
                        + "\n[c/aee6f3:I've been exploring them overtime for probably years now. But that's fine,]"
                        + "\n[c/aee6f3:Not like I got anything better to do.]"
                        + "\n[c/aee6f3:I'll be leaving this planet soon and moving onto the next solar system.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog18 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #999735");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'I haven't done one of these in forever, but I should explain what happened.]"
                        + "\n[c/aee6f3:I was travelling to the next solar system, when suddenly some wormhole appeared.]"
                        + "\n[c/aee6f3:The SoS couldn't turn fast enough so it got sucked in. Wormholes are like portals of the universe,]"
                        + "\n[c/aee6f3:so I expected to just reach the end instantly, but no, I was stuck in the wormhole for almost 1000 years.]"
                        + "\n[c/aee6f3:God it was boring, but I had the androids to keep me company. Unfortunately, I don't know where I am now.]"
                        + "\n[c/aee6f3:I can't tell how far away I am from home, but I see a nearby star, so hopefully there's some planets.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog19 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #1000000");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Today is the millionth day in space. When I was writing that down,]"
                        + "\n[c/aee6f3:I had a dumb moment where I thought it was a million years... But no.]"
                        + "\n[c/aee6f3:It has only been 2739.7 years, so... 364,000,000? days left... It feels like forever,]"
                        + "\n[c/aee6f3:and yet it's only been 0.27% of a million. Why am I still doing this. What's the point anymore?]"
                        + "\n[c/aee6f3:Every day is a pain, I just want to eat, I want to sleep...]"
                        + "\n[c/aee6f3:I would say I want to be human again, but to be honest... I don't even want to be alive anymore.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog20 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #1012875");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'About damn time. I've finally found a green planet. But am I happy about this?]"
                        + "\n[c/aee6f3:Not really. I thought this would make me feel something, but it's hopeless.]"
                        + "\n[c/aee6f3:I can't remember the last time I felt happy, the memory of my home is starting to get foggy.]"
                        + "\n[c/aee6f3:But anyway, it looks like this planet has intelligent life, so I'll land and see if they're friendly.]"
                        + "\n[c/aee6f3:If they're not, I'll just shoot them. Simple.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog21 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #3650000");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Today is the 10,000th year in space, only 1% of a million...]"
                        + "\n[c/aee6f3:I feel like living beings shouldn't be allowed to live for this long.]"
                        + "\n[c/aee6f3:A hundred years for a human is forever, and I've been around for 100x that!]"
                        + "\n[c/aee6f3:I've redesigned my robotic body again, but I still haven't figured out how to get into my]"
                        + "\n[c/aee6f3:human mind and get rid of this STUPID HUNGER. I DON'T HAVE A STOMACH, WHY AM I HUNGRY!?]"
                        + "\n[c/aee6f3:I DON'T HAVE EYES, WHY AM I SO TIRED!? WHY DO I HAVE TO LIVE THROUGH THIS DAMN IT!?']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog22 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #5385430");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'The SoS got attacked by some Space Pirates.]"
                        + "\n[c/aee6f3:Not like I care, I destroyed their ships so what can they do now?]"
                        + "\n[c/aee6f3:The SoS's scanner picked up a lifeform in the engine room, so I should probably check it out.]"
                        + "\n[c/aee6f3:I can't be asked to do anything really. But whatever.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog23 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #25338300");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Nice.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog24 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #36500001");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'It's been 10% of a million years now. Yay.]"
                        + "\n[c/aee6f3:I accidently skipped a day in the data logs so 10% was really yesterday, but not like I care.]"
                        + "\n[c/aee6f3:I have explored... 2853 planets now, and they are starting to all look the same.]"
                        + "\n[c/aee6f3:I'm sick of ice planets, sick of lush green ones, sick of barren ones... I guess I'm not satisfied anymore.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog25 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #164550614");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'I haven't felt like this in forever.]"
                        + "\n[c/aee6f3:I upgraded my robotic self again, this time with more attack power. Xehito let me test it on him,]"
                        + "\n[c/aee6f3:so we had a fight. The intensity of it was almost exhilarating, lasers firing everywhere,]"
                        + "\n[c/aee6f3:explosions all around, it was generally a fun time. But something tells me he only let me]"
                        + "\n[c/aee6f3:fight him to cheer me up, and I'm sorry Xehito, but that moment didn't last, I still feel empty.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog26 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #364635000");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Only 1000 years until a million, and I can return home.]"
                        + "\n[c/aee6f3:I've already set my course, but the problem is, because of that wormhole,]"
                        + "\n[c/aee6f3:I don't know which way is home... All I can do now is go to a random direction]"
                        + "\n[c/aee6f3:and hope for the best. But the galaxy is vast, I fear by the time I reach home again,]"
                        + "\n[c/aee6f3:The next reset would've already started, and I'd have to wait another million years...]"
                        + "\n[c/aee6f3:If that happens, I won't try anymore, I'll just give up.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }
            }

            public class Datalog27 : ModItem
            {
                public override string Texture => "Redemption/Items/Lore/Datalog";

                public override void SetStaticDefaults()
                {
                    DisplayName.SetDefault("Data Log #365000000");
                    Tooltip.SetDefault("It reads - [c/aee6f3:'Today is the millionth year in space!]"
                        + "\n[c/aee6f3:Unfortunately, I won't be able to see the new Great Era for some time.]"
                        + "\n[c/aee6f3:I'm still lost in space.']");
                    Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                }

                public override void SetDefaults()
                {
                    Item.width = 34;
                    Item.height = 30;
                    Item.maxStack = 1;
                    Item.value = 0;
                    Item.rare = ItemRarityID.Cyan;
                }

                public class Datalog28 : ModItem
                {
                    public override string Texture => "Redemption/Items/Lore/Datalog";

                    public override void SetStaticDefaults()
                    {
                        DisplayName.SetDefault("Data Log #389035250");
                        Tooltip.SetDefault("It reads - [c/aee6f3:'I've made it back, I'm home again.']");
                        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
                    }

                    public override void SetDefaults()
                    {
                        Item.width = 34;
                        Item.height = 30;
                        Item.maxStack = 1;
                        Item.value = 0;
                        Item.rare = ItemRarityID.Cyan;
                    }
                }
            }
        }
    }
}