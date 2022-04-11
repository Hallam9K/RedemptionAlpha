using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.StructureHelper.GUI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.StructureHelper.ChestHelper.GUI
{ 
    class ChestCustomizerState : UIState
    {
        public static bool Visible;

        internal UIList ruleElements = new UIList();
        internal UIScrollbar scrollBar = new UIScrollbar();

        UIImageButton NewGuaranteed = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/PlusR"));
        UIImageButton NewChance = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/PlusG"));
        UIImageButton NewPool = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/PlusP"));
        UIImageButton NewPoolChance = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/PlusB"));

        public static UIImageButton closeButton = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/Cross"));

        public override void OnInitialize()
		{
            ManualGeneratorMenu.SetDims(ruleElements, -200, 0.5f, 0, 0.1f, 400, 0, 0, 0.8f);
            ManualGeneratorMenu.SetDims(scrollBar, 232, 0.5f, 0, 0.1f, 32, 0, 0, 0.8f);
            ruleElements.SetScrollbar(scrollBar);
            Append(ruleElements);
            Append(scrollBar);

            ManualGeneratorMenu.SetDims(NewGuaranteed, -200, 0.5f, -50, 0.1f, 32, 0, 32, 0);
            NewGuaranteed.OnClick += (n, m) => ruleElements.Add(new GuaranteedRuleElement());
            Append(NewGuaranteed);

            ManualGeneratorMenu.SetDims(NewChance, -160, 0.5f, -50, 0.1f, 32, 0, 32, 0);
            NewChance.OnClick += (n, m) => ruleElements.Add(new ChanceRuleElement());
            Append(NewChance);

            ManualGeneratorMenu.SetDims(NewPool, -120, 0.5f, -50, 0.1f, 32, 0, 32, 0);
            NewPool.OnClick += (n, m) => ruleElements.Add(new PoolRuleElement());
            Append(NewPool);

            ManualGeneratorMenu.SetDims(NewPoolChance, -80, 0.5f, -50, 0.1f, 32, 0, 32, 0);
            NewPoolChance.OnClick += (n, m) => ruleElements.Add(new PoolChanceRuleElement());
            Append(NewPoolChance);

            ManualGeneratorMenu.SetDims(closeButton, 200 - 32, 0.5f, -50, 0.1f, 32, 0, 32, 0);
            closeButton.OnClick += (n, m) => Visible = false;
            Append(closeButton);
        }

		public bool SetData(ChestEntity entity)
        {
            entity.rules.Clear();

            if (ruleElements.Count == 0)
			{
                entity.Kill(entity.Position.X, entity.Position.Y);
                return false;
			}

            for(int k = 0; k < ruleElements.Count; k++)
            {
                entity.rules.Add((ruleElements._items[k] as ChestRuleElement).rule.Clone());
            }

            return true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();

            var color = new Color(49, 84, 141);

            ManualGeneratorMenu.DrawBox(spriteBatch, NewGuaranteed.GetDimensions().ToRectangle(), NewGuaranteed.IsMouseHovering ? color : color * 0.8f);
            ManualGeneratorMenu.DrawBox(spriteBatch, NewChance.GetDimensions().ToRectangle(), NewChance.IsMouseHovering ? color : color * 0.8f);
            ManualGeneratorMenu.DrawBox(spriteBatch, NewPool.GetDimensions().ToRectangle(), NewPool.IsMouseHovering ? color : color * 0.8f);
            ManualGeneratorMenu.DrawBox(spriteBatch, NewPoolChance.GetDimensions().ToRectangle(), NewPoolChance.IsMouseHovering ? color : color * 0.8f);

            ManualGeneratorMenu.DrawBox(spriteBatch, closeButton.GetDimensions().ToRectangle(), closeButton.IsMouseHovering ? color : color * 0.8f);

            var rect = ruleElements.GetDimensions().ToRectangle();
            rect.Inflate(30, 10);
            ManualGeneratorMenu.DrawBox(spriteBatch, rect, new Color(20, 40, 60) * 0.8f);

            if(rect.Contains(Main.MouseScreen.ToPoint()))
                Main.LocalPlayer.mouseInterface = true;

            if (NewGuaranteed.IsMouseHovering)
            {
                Main.hoverItemName = "Add New Guaranteed Rule";
                Main.LocalPlayer.mouseInterface = true;
            }

            if (NewChance.IsMouseHovering)
            {
                Main.hoverItemName = "Add New Chance Rule";
                Main.LocalPlayer.mouseInterface = true;
            }

            if (NewPool.IsMouseHovering)
            {
                Main.hoverItemName = "Add New Pool Rule";
                Main.LocalPlayer.mouseInterface = true;
            }

            if (NewPoolChance.IsMouseHovering)
            {
                Main.hoverItemName = "Add New Pool + Chance Rule";
                Main.LocalPlayer.mouseInterface = true;
            }

            if (closeButton.IsMouseHovering)
            {
                Main.hoverItemName = "Close";
                Main.LocalPlayer.mouseInterface = true;
            }

            base.Draw(spriteBatch);
        }
	}
}
