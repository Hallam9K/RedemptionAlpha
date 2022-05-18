using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.Particles;
using Redemption.UI;
using System;
using Terraria;
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
		public override bool? UseItem(Player player)
		{
			NPC npc = NPC.NewNPCDirect(Item.GetSource_FromThis(), player.Center, NPCID.GreenSlime);
			npc.position = player.Center;
			Dialogue dialogue1 = new(npc, null, null, null, Color.LightGreen, Color.DarkCyan, null, "Hey there, don't mind me, I'm just testing this UI. I'll be out of your^20^...[90]uh^20^...[90]^6^hair[30] in no time.", 6, 120, 30, true);
			Dialogue dialogue2 = new(npc, null, null, null, Color.LightGreen, Color.DarkCyan, dialogue1, "It's such a lovely day out! I hope nothing bad happens to me...", 6, 120, 30, true);
			TextBubbleUI.Visible = true;
			TextBubbleUI.AddDialogue(dialogue1);
			TextBubbleUI.AddDialogue(dialogue2);

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
				Redemption.Targets.BasicLayer.Sprites.Add(this);
			return true;
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
