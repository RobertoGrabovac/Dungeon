using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;

namespace Dungeon.Menu
{
    internal class Button
    {
        private RectangleShape shape;
        private Text buttonText;

        private Vector2f originalSize;
        private Vector2f originalPosition;

        public bool Clicked { get; set; }

        public Vector2f Position
        {
            get => originalPosition; set
            {
                originalPosition = value;
                shape.Position = originalPosition;
            }
        }

        public Button(Vector2f position, Vector2f size, string text, Color buttonColor, Color outlineColor, Texture? texture = null)
        {
            Clicked = false;
            originalSize = size;
            originalPosition = position;
            shape = new RectangleShape(size)
            {
                Position = position,
                FillColor = buttonColor,
                OutlineColor = outlineColor,
                OutlineThickness = 4f
            };
            if (texture != null)
            {
                shape.Texture = texture;
            }

            buttonText = new Text(text, new Font(Path.Combine("assets", "font.ttf")), 20)
            {
                Position = new Vector2f(
                    position.X + (size.X - text.Length * 10) / 2,
                    position.Y + (size.Y - 20) / 2
                ),
                FillColor = SFML.Graphics.Color.White
            };

        }

        public void Draw(RenderWindow window)
        {
            window.Draw(shape);
            window.Draw(buttonText);
        }

        public bool IsMouseOver(RenderWindow window)
        {
            Vector2f mousePosition = window.MapPixelToCoords(Mouse.GetPosition(window));
            FloatRect buttonBounds = shape.GetGlobalBounds();

            bool isMouseOver = buttonBounds.Contains(mousePosition.X, mousePosition.Y);

            if (isMouseOver)
            {
                shape.Size = new Vector2f(originalSize.X + 10, originalSize.Y + 10);
                shape.Position = new Vector2f(originalPosition.X - 5, originalPosition.Y - 5);

                buttonText.Position = new Vector2f(
                    shape.Position.X + (shape.Size.X - buttonText.DisplayedString.Length * 10) / 2,
                    shape.Position.Y + (shape.Size.Y - 20) / 2
                );
            }
            else
            {
                shape.Size = originalSize;
                shape.Position = originalPosition;

                buttonText.Position = new Vector2f(
                    shape.Position.X + (shape.Size.X - buttonText.DisplayedString.Length * 10) / 2,
                    shape.Position.Y + (shape.Size.Y - 20) / 2
                );
            }

            return isMouseOver;
        }
    }
}
