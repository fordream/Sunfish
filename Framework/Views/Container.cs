using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;

namespace Sunfish.Views
{
	public class Container : View
	{

		public Sprite BackgroundSprite;

		protected List<View> ChildViews;

		protected Constants.ViewContainerLayout Layout;

		private float NextChildXOffset = 0;

		private float NextChildYOffset = 0;

		private float FloatLeftHeight = 0;

		public bool ShouldExpandHeight;

		public Container (Texture2D backgroundTexture, Constants.ViewContainerLayout layout) :
			this(backgroundTexture, new Vector2(0,0), Constants.ViewLayer.Layer1, layout, true)
		{
		}

		public Container (Texture2D backgroundTexture, Constants.ViewLayer layer, Constants.ViewContainerLayout layout) :
			this(backgroundTexture, new Vector2(0,0), layer, layout, true)
		{
		}

		public Container (Texture2D backgroundTexture, Vector2 position, Constants.ViewContainerLayout layout) :
			this(backgroundTexture, position, Constants.ViewLayer.Layer1, layout, true)
		{
		}

		public Container (int width, int height, Constants.ViewLayer layer, Constants.ViewContainerLayout layout) :
			this(width, height, new Vector2(0,0), layer, layout)
		{
		}

		public Container (int width, int height, Constants.ViewContainerLayout layout) :
		this(width, height, new Vector2(0,0), layout)
		{
		}

		public Container (int width, int height, Vector2 position, Constants.ViewContainerLayout layout) :
			this(width, height, position, Constants.ViewLayer.Layer1, layout)
		{
		}

		public Container (int width, int height, Vector2 position, Constants.ViewLayer layer, Constants.ViewContainerLayout layout) :
		base(position, width, height, layer, true)
		{
			Layout = layout;
			ChildViews = new List<View>();
			ShouldExpandHeight = false;
		}

		public Container (Texture2D backgroundTexture, Vector2 position, Constants.ViewLayer layer, Constants.ViewContainerLayout layout) :
		this(backgroundTexture, position, layer, layout, true)
		{
		}

		public Container (Texture2D backgroundTexture, Vector2 position, Constants.ViewLayer layer, Constants.ViewContainerLayout layout, bool visible) :
			base(position, backgroundTexture.Width, backgroundTexture.Height, layer, visible)
		{
			Layout = layout;
			BackgroundSprite = new Sprite (backgroundTexture, new Vector2 (0, 0), layer);
			ChildViews = new List<View>();
			AddChild (BackgroundSprite, Constants.ViewContainerLayout.Absolute, 0, 0);
			ShouldExpandHeight = false;
		}

		public void AddChild (View view)
		{
			AddChild (view, 0, 0);
		}

		public void AddChild (View view, int marginX, int marginY)
		{
			AddChild (view, Layout, marginX, marginY);
		}

		public override void Draw (GameTime gameTime, GraphicsDeviceManager graphics)
		{
			// Do nothing
		}

//		public virtual void RemoveChildren ()
//		{
//			ChildViews.Clear ();
//			NextChildXOffset = 0;
//			NextChildYOffset = 0;
//			FloatLeftHeight = 0;
//		}

		protected void AddChild (View view, Constants.ViewContainerLayout layout, int marginX, int marginY)
		{

			if (layout == Constants.ViewContainerLayout.Absolute) {
				view.Position = GetChildAbsolutePosition (view, marginX, marginY);
			} else if (layout == Constants.ViewContainerLayout.FloatLeft) {
				view.Position = GetChildFloatLeftPosition (view, marginX, marginY);
			} else if (layout == Constants.ViewContainerLayout.Stack) {
				view.Position = GetChildStackPosition (view, marginX, marginY);
				NextChildYOffset = view.Position.Y + view.Height;
			} else if (layout == Constants.ViewContainerLayout.StackCentered) {
				view.Position = GetChildStackCenterHorizontalPosition (view, marginY);
				NextChildYOffset = view.Position.Y + view.Height;
			}

			view.SetParent (this);
			ChildViews.Add (view);
			SunfishGame.ActiveScreen.AddChildView (view);

			ExpandHeightIfNecessary (view);

		}

		private Vector2 GetChildAbsolutePosition (View child, int marginX, int marginY)
		{
			return new Vector2 (child.Position.X + marginX, child.Position.Y + marginY);
		}

		private Vector2 GetChildFloatLeftPosition (View child, int marginX, int marginY)
		{
			float sameRowXOffset = NextChildXOffset + marginX + child.Position.X;
			if (sameRowXOffset + child.Width > Width) { // Need to start a new row
				NextChildXOffset = marginX + child.Position.X + child.Width;
				NextChildYOffset = FloatLeftHeight;
				return new Vector2 (marginX + child.Position.X, FloatLeftHeight + child.Position.Y + marginY);
			} else { // The child fits in the current row
				FloatLeftHeight = Math.Max (FloatLeftHeight, child.Position.Y + marginY + NextChildYOffset + child.Height);
				NextChildXOffset = sameRowXOffset + child.Width;
				return new Vector2 (sameRowXOffset, child.Position.Y + marginY + NextChildYOffset);
			}
		}

		private Vector2 GetChildStackPosition (View child, int marginX, int marginY)
		{
			return new Vector2 (child.Position.X + marginX, child.Position.Y + marginY + NextChildYOffset);
		}

		private Vector2 GetChildStackCenterHorizontalPosition (View child, int marginY)
		{
			return new Vector2 (((float)Width - (float)child.Width) / 2f, child.Position.Y + marginY + NextChildYOffset);	
		}

		private void ExpandHeightIfNecessary (View childView)
		{
			if (ShouldExpandHeight) {
				int yOffset = (int)Position.Y + Height;
				int childViewYOffset = (int)childView.Position.Y + childView.Height;
				if (childViewYOffset > yOffset) {
					Height = (int)childView.Position.Y + childView.Height - (int)childView.Origin.Y - (int)Position.Y;
				}
			}
		}
	}
}

