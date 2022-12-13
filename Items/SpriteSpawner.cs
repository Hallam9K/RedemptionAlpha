using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Tiles.Tiles;
using Redemption.UI;
using Redemption.WorldGeneration;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Redemption.Effects.RenderTargets.BasicLayer;

namespace Redemption.Items
{
    public class SpriteSpawner : ModItem, IBasicSprite
    {
        public static int x;
        public static int y;
        public int divisions;
        public Vector2 offset = new(0f, 0f);
        public bool active = x != 0 && y != 0;
        public bool Active { get => Item.active; set => Item.active = value; }
        public float progress;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sprite Spawner");
            Tooltip.SetDefault("Spawns sprites at the cursor and continuously draws them at that location.\n" +
                               "Can be used to test shaders. You should use Edit and Continue to do this.");
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Purple;
        }
        public override bool AltFunctionUse(Player player) => true;
        public int ge;
        public override bool? UseItem(Player player)
        {
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<AsteroidTile>(),
                [new Color(150, 150, 150)] = -2, //turn into air
                [Color.Black] = -1 //don't touch when genning
            };
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Space/AstGen" + (ge + 1), AssetRequestMode.ImmediateLoad).Value;
            Point16 origin = new((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            ge++;
            if (ge > 24)
                ge = 0;
            /*NPC npc = NPC.NewNPCDirect(Item.GetSource_FromThis(), player.Center, NPCID.GreenSlime);
            npc.position = player.Center;

            DialogueChain chain = new();
            chain.modifier = new(0f, 128f);
            chain.Add(new(npc, "Hey there, don't mind me, I'm just testing this UI. I'll be out of your^20^...[90]uh^20^...[90]^6^hair[30] in no time.", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "It's such a lovely day out! I hope nothing bad happens to me...", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "What could go wrong anyway? Seems pretty safe out here.", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "Plus! I could just run away at any time! I'm SUPER[@BOO!] good at jumping.", Color.LightGreen, Color.DarkCyan, boxFade: true))
                 .Add(new(npc, "[@Gotcha!]Apparently there's a slime out there that can jump a bajillion feet into the air! Something like the King of all slimes...", Color.LightGreen, Color.DarkCyan, boxFade: true));
            chain.OnSymbolTrigger += Chain_OnSymbolTrigger;

            TextBubbleUI.Visible = true;
            TextBubbleUI.Add(chain);
            //Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<AdamPortal>(), 0, 0f);

            if (player.altFunctionUse == 2)
            {
                x = 0;
                y = 0;
                progress = 0f;
                Talk("Coordinates cleared.", new Color(218, 70, 70));
                if (Redemption.Targets.BasicLayer.Sprites.Contains(this))
                    Redemption.Targets.BasicLayer.Sprites.Remove(this);
                return true;
            }
            x = (int)(Main.MouseWorld.X / 16);
            y = (int)(Main.MouseWorld.Y / 16);
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, new Color(218, 70, 70), null);
            Talk($"Drawing sprites at [{x}, {y}]. Right-click to discard.", new Color(218, 70, 70));
            if (!Redemption.Targets.BasicLayer.Sprites.Contains(this))
                Redemption.Targets.BasicLayer.Sprites.Add(this);*/
            return true;
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            Talk(signature, Color.LightGreen);
        }
        public static void Talk(string message, Color color) => Main.NewText(message, color.R, color.G, color.B);
        public void Draw(SpriteBatch spriteBatch)
        {
            //float sin = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) + 1f / 4f + 1f;

            //progress += 1f;

            //if(progress % Main.rand.Next(18, 20) == 0)
            //    ParticleManager.NewParticle(new Vector2(x * 16, y * 16), Vector2.Zero, new EmberParticle(), Color.White, 1f);

            // Draw code here.
            //Effect effect = ModContent.Request<Effect>("Redemption/Effects/Circle").Value;
            //effect.Parameters["uColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            //effect.Parameters["uOpacity"].SetValue(1f);
            //effect.Parameters["uProgress"].SetValue(progress / 100f);
            //effect.CurrentTechnique.Passes[0].Apply();

            //Texture2D circle = ModContent.Request<Texture2D>("Redemption/Textures/EmptyPixel").Value;

            //spriteBatch.Draw(circle, new Vector2(x * 16, y * 16) - Main.screenPosition, Color.White);
            //spriteBatch.Draw(circle, new Vector2(x * 16, y * 16) - Main.screenPosition - new Vector2(progress * 0.5f * sin, progress * 0.5f * sin), new Rectangle(0, 0, 1, 1), Color.White, 0f, Vector2.Zero, progress * sin, SpriteEffects.None, 0f);
        }
    }
}
