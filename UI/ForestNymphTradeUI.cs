using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Friendly;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class ForestNymphTradeUI : UIState
    {
        private readonly UIImage BgSprite = new(ModContent.Request<Texture2D>(Redemption.EMPTY_TEXTURE));
        public static bool Visible = false;
        public static bool Appended = false;
        public Vector2 lastScreenSize;

        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            BgSprite.Width.Set(200, 0f);
            BgSprite.Height.Set(300, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) - 164f, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) - 103f, 0f);
        }
        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.releaseInventory || Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<ForestNymph_Friendly>())
                Visible = false;

            if (!Appended)
            {
                RemoveAllChildren();
                int pad = 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.HerbBag), new Item(ModContent.ItemType<ForestCore>())));
                BgSprite.Append(new TradePanelUI(new Item(ModContent.ItemType<AnglonicMysticBlossom>()), new Item(ModContent.ItemType<ForestNymphsSickle>())) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.JungleRose), new Item(ItemID.NaturesGift)) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.NaturesGift), new Item(ItemID.JungleRose)) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.Fireblossom, 10), new Item(ItemID.ObsidianRose), 10) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ModContent.ItemType<LostSoul>(), 8), new Item(ItemID.HerbBag), 8) { Top = new StyleDimension(pad, 0) });
                Append(BgSprite);
                Appended = true;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - 164f;
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - 103f;
                BgSprite.Recalculate();
            }

            base.Draw(spriteBatch);
        }
    }
}
