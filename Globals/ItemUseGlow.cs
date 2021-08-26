using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.Globals
{
	class ItemGlowLayer : PlayerDrawLayer // Code by qwerty3.14
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			return true;
		}
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.JustDroppedAnItem)
			{
				return;
			}
			if (drawInfo.drawPlayer.heldProj >= 0 && drawInfo.shadow == 0f && !drawInfo.heldProjOverHand)
			{
				drawInfo.projectileDrawPosition = drawInfo.DrawDataCache.Count;
			}
			Item heldItem = drawInfo.heldItem;
			int itemID = heldItem.type;
			float adjustedItemScale = drawInfo.drawPlayer.GetAdjustedItemScale(heldItem);
			Main.instance.LoadItem(itemID);

            if (heldItem.TryGetGlobalItem(out ItemUseGlow result))
            {
                Texture2D glowTexture = result.glowTexture;

                if (glowTexture != null)
                {

                    Texture2D itemTexture = TextureAssets.Item[itemID].Value;
                    Vector2 position = new((int)(drawInfo.ItemLocation.X - Main.screenPosition.X), (int)(drawInfo.ItemLocation.Y - Main.screenPosition.Y));
                    Rectangle? sourceRect = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);

                    if (ItemID.Sets.IsFood[itemID])
                    {
                        sourceRect = itemTexture.Frame(1, 3, 0, 1);
                    }
                    drawInfo.itemColor = Lighting.GetColor((int)(drawInfo.Position.X + drawInfo.drawPlayer.width * 0.5) / 16, (int)((drawInfo.Position.Y + drawInfo.drawPlayer.height * 0.5) / 16.0));
                    if (drawInfo.drawPlayer.shroomiteStealth && heldItem.CountsAsClass(DamageClass.Ranged))
                    {
                        float num2 = drawInfo.drawPlayer.stealth;
                        if (num2 < 0.03)
                        {
                            num2 = 0.03f;
                        }
                        float num3 = (1f + num2 * 10f) / 11f;
                        drawInfo.itemColor = new Color((byte)(drawInfo.itemColor.R * num2), (byte)(drawInfo.itemColor.G * num2), (byte)(drawInfo.itemColor.B * num3), (byte)(drawInfo.itemColor.A * num2));
                    }
                    if (drawInfo.drawPlayer.setVortex && heldItem.CountsAsClass(DamageClass.Ranged))
                    {
                        float num4 = drawInfo.drawPlayer.stealth;
                        if (num4 < 0.03)
                        {
                            num4 = 0.03f;
                        }
                        //_ = (1f + num4 * 10f) / 11f;
                        drawInfo.itemColor = drawInfo.itemColor.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new Vector4(0f, 0.12f, 0.16f, 0f), 1f - num4)));
                    }
                    bool flag = drawInfo.drawPlayer.itemAnimation > 0 && heldItem.useStyle != 0;
                    bool flag2 = heldItem.holdStyle != 0 && !drawInfo.drawPlayer.pulley;
                    if (!drawInfo.drawPlayer.CanVisuallyHoldItem(heldItem))
                    {
                        flag2 = false;
                    }
                    if (drawInfo.shadow != 0f || drawInfo.drawPlayer.frozen || !(flag || flag2) || itemID <= 0 || drawInfo.drawPlayer.dead || heldItem.noUseGraphic || (drawInfo.drawPlayer.wet && heldItem.noWet) || (drawInfo.drawPlayer.happyFunTorchTime && drawInfo.drawPlayer.inventory[drawInfo.drawPlayer.selectedItem].createTile == 4 && drawInfo.drawPlayer.itemAnimation == 0))
                    {
                        return;
                    }
                    //_ = drawInfo.drawPlayer.name;
                    Color color = new(250, 250, 250, heldItem.alpha);
                    //Vector2 vector = Vector2.Zero;

                    Vector2 origin = new(sourceRect.Value.Width * 0.5f - sourceRect.Value.Width * 0.5f * drawInfo.drawPlayer.direction, sourceRect.Value.Height);
                    if (heldItem.useStyle == 9 && drawInfo.drawPlayer.itemAnimation > 0)
                    {
                        Vector2 value2 = new(0.5f, 0.4f);
                        origin = sourceRect.Value.Size() * value2;
                    }
                    if (drawInfo.drawPlayer.gravDir == -1f)
                    {
                        origin.Y = sourceRect.Value.Height - origin.Y;
                    }
                    //origin += vector;
                    float itemRotation = drawInfo.drawPlayer.itemRotation;
                    if (heldItem.useStyle == 8)
                    {
                        ref float x = ref position.X;
                        float num6 = x;
                        //_ = drawInfo.drawPlayer.direction;
                        x = num6 - 0f;
                        itemRotation -= (float)Math.PI / 2f * drawInfo.drawPlayer.direction;
                        origin.Y = 2f;
                        origin.X += 2 * drawInfo.drawPlayer.direction;
                    }
                    ItemSlot.GetItemLight(ref drawInfo.itemColor, heldItem);
                    DrawData drawData;

                    if (heldItem.useStyle == 5)
                    {
                        if (Item.staff[itemID])
                        {
                            float num9 = drawInfo.drawPlayer.itemRotation + 0.785f * drawInfo.drawPlayer.direction;
                            int num10 = 0;
                            int num11 = 0;
                            Vector2 origin5 = new(0f, itemTexture.Height);
                            if (itemID == 3210)
                            {
                                num10 = 8 * -drawInfo.drawPlayer.direction;
                                num11 = 2 * (int)drawInfo.drawPlayer.gravDir;
                            }
                            if (itemID == 3870)
                            {
                                Vector2 vector2 = (drawInfo.drawPlayer.itemRotation + (float)Math.PI / 4f * drawInfo.drawPlayer.direction).ToRotationVector2() * new Vector2(-drawInfo.drawPlayer.direction * 1.5f, drawInfo.drawPlayer.gravDir) * 3f;
                                num10 = (int)vector2.X;
                                num11 = (int)vector2.Y;
                            }
                            if (drawInfo.drawPlayer.gravDir == -1f)
                            {
                                if (drawInfo.drawPlayer.direction == -1)
                                {
                                    num9 += 1.57f;
                                    origin5 = new Vector2(itemTexture.Width, 0f);
                                    num10 -= itemTexture.Width;
                                }
                                else
                                {
                                    num9 -= 1.57f;
                                    origin5 = Vector2.Zero;
                                }
                            }
                            else if (drawInfo.drawPlayer.direction == -1)
                            {
                                origin5 = new Vector2(itemTexture.Width, itemTexture.Height);
                                num10 -= itemTexture.Width;
                            }
                            drawData = new DrawData(glowTexture, new Vector2((int)(drawInfo.ItemLocation.X - Main.screenPosition.X + origin5.X + num10), (int)(drawInfo.ItemLocation.Y - Main.screenPosition.Y + num11)), sourceRect, new Color(250, 250, 250, heldItem.alpha), num9, origin5, adjustedItemScale, drawInfo.itemEffect, 0);
                            drawInfo.DrawDataCache.Add(drawData);
                            return;
                        }

                        Vector2 vector3 = new(itemTexture.Width / 2, itemTexture.Height / 2);
                        Vector2 vector4 = Main.DrawPlayerItemPos(drawInfo.drawPlayer.gravDir, itemID);
                        int num12 = (int)vector4.X;
                        vector3.Y = vector4.Y;
                        Vector2 origin6 = new(-num12, itemTexture.Height / 2);
                        if (drawInfo.drawPlayer.direction == -1)
                        {
                            origin6 = new Vector2(itemTexture.Width + num12, itemTexture.Height / 2);
                        }
                        drawData = new DrawData(glowTexture, new Vector2((int)(drawInfo.ItemLocation.X - Main.screenPosition.X + vector3.X), (int)(drawInfo.ItemLocation.Y - Main.screenPosition.Y + vector3.Y)), sourceRect, new Color(250, 250, 250, heldItem.alpha), drawInfo.drawPlayer.itemRotation, origin6, adjustedItemScale, drawInfo.itemEffect, 0);
                        drawInfo.DrawDataCache.Add(drawData);
                        if (heldItem.color != default)
                        {
                            drawData = new DrawData(glowTexture, new Vector2((int)(drawInfo.ItemLocation.X - Main.screenPosition.X + vector3.X), (int)(drawInfo.ItemLocation.Y - Main.screenPosition.Y + vector3.Y)), sourceRect, new Color(250, 250, 250, heldItem.alpha), drawInfo.drawPlayer.itemRotation, origin6, adjustedItemScale, drawInfo.itemEffect, 0);
                            drawInfo.DrawDataCache.Add(drawData);
                        }
                        return;
                    }
                    if (drawInfo.drawPlayer.gravDir == -1f)
                    {
                        drawData = new DrawData(glowTexture, position, sourceRect, new Color(250, 250, 250, heldItem.alpha), itemRotation, origin, adjustedItemScale, drawInfo.itemEffect, 0);
                        drawInfo.DrawDataCache.Add(drawData);
                        if (heldItem.color != default)
                        {
                            drawData = new DrawData(glowTexture, position, sourceRect, new Color(250, 250, 250, heldItem.alpha), itemRotation, origin, adjustedItemScale, drawInfo.itemEffect, 0);
                            drawInfo.DrawDataCache.Add(drawData);
                        }
                        return;
                    }
                    drawData = new DrawData(glowTexture, position, sourceRect, new Color(250, 250, 250, heldItem.alpha), itemRotation, origin, adjustedItemScale, drawInfo.itemEffect, 0);
                    drawInfo.DrawDataCache.Add(drawData);
                    if (heldItem.color != default)
                    {
                        drawData = new DrawData(glowTexture, position, sourceRect, new Color(250, 250, 250, heldItem.alpha), itemRotation, origin, adjustedItemScale, drawInfo.itemEffect, 0);
                        drawInfo.DrawDataCache.Add(drawData);
                    }
                }
            }
        }

	}
	public class ItemUseGlow : GlobalItem
	{
		public Texture2D glowTexture = null;
		public int glowOffsetY = 0;
		public int glowOffsetX = 0;
		public override bool InstancePerEntity => true;
		public override GlobalItem Clone(Item item, Item itemClone)
		{
			return base.Clone(item, itemClone);
		}
		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (glowTexture != null)
			{
				Texture2D texture = glowTexture;
				spriteBatch.Draw
				(
					texture,
					new Vector2
					(
						item.position.X - Main.screenPosition.X + item.width * 0.5f,
						item.position.Y - Main.screenPosition.Y + item.height - texture.Height * 0.5f + 2f
					),
					new Rectangle(0, 0, texture.Width, texture.Height),
					Color.White,
					rotation,
					texture.Size() * 0.5f,
					scale,
					SpriteEffects.None,
					0f
				);
			}
			base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
		}
	}
}