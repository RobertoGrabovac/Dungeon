using Dungeon.sprites;
using SFML.Graphics;
using SFML.System;
using Sprite = SFML.Graphics.Sprite;

namespace Dungeon.Game;

/**
 * Klasa koja implementira sve vezano za sučelje (Heads Up Display) tijekom igre.
 */
public class HUD : Drawable
{
    private Game game;


    private int margin = 5;
    
    private static readonly Texture heartTexture = new Texture(Path.Combine("assets", "heart.png"));
    private static readonly Font font = new(Path.Combine("assets", "font.ttf"));
    private RenderTexture[] fontmap;
    private const float fontScale = 40.0f;
    private const float invFontScale = 1.0f / fontScale;

    public HUD(Game game)
    {
        this.game = game;
        this.fontmap = new RenderTexture[256];
        bakeFontmap();
    }

    /**
     * Budući da SFML.Net ne eksportira SFML-ovu funkcionalnost isključivanja filtriranja renderiranih fontova
     * (unatoč tome što je ona dostupna u nativnoj biblioteci), moramo djelomično reimplementirati tu funkcionalnost
     * tako da sami "spečemo" teksturu koja se sastoji od svih nama relevantnih znakova i koja se ne filtrira pri
     * renderiranju (kako bi bila u skladu s pixel art stilom igre).
     */
    private void bakeFontmap()
    {
        for (int i = 0; i < 256; ++i)
        {
            var letter = new Text((char)i + "", font, (uint)fontScale);
            var letterTexture = new RenderTexture((uint)fontScale, (uint)fontScale);
            letterTexture.Clear(Color.Transparent);
            letterTexture.Draw(letter);
            letterTexture.Display();
            letterTexture.Smooth = false;
            
            fontmap[i] = letterTexture;
        }
    }

    public void DrawText(Text text, RenderTarget target, RenderStates states)
    {
        var letters = text.DisplayedString;
        var position = text.Position;
        
        foreach (char letter in letters)
        {
            var letterSprite = new Sprite(fontmap[letter].Texture);
            float scale = text.CharacterSize * invFontScale;
            letterSprite.Position = position;
            letterSprite.Scale = new Vector2f(scale, scale);
            letterSprite.Color = text.FillColor;
            target.Draw(letterSprite, states);

            position += new Vector2f(letterSprite.Texture.Size.X * scale - 2, 0);
        }
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        DrawHearts(target, states);
        DrawScoreAndCoins(target, states);
    }

    private void DrawHearts(RenderTarget target, RenderStates states) 
    {
        float x, y;
        for (int i = 0; i < game.player.health; i++)
        {
            float scale = 1.0f;
            float spacing = 2;
            y = margin;
            x = margin + i * (heartTexture.Size.X * scale + spacing);

            var heartSprite = new Sprite(heartTexture)
            {
                Position = new Vector2f(x, y),
                Scale = new Vector2f(scale, scale)
            };

            heartSprite.Draw(target, states);
        }
    }

    private void DrawScoreAndCoins(RenderTarget target, RenderStates states)
    {
        var view = target.GetView();
        float spacing = 2;

        var scoreText = new Text("Score: " + game.Score, font);
        scoreText.CharacterSize = 8;
        scoreText.FillColor = Color.White;
        float x = view.Size.X - scoreText.GetGlobalBounds().Width - margin;
        float y = margin;
        scoreText.Position = new Vector2f(x - 15, y);
        DrawText(scoreText, target, states);

        var goldText = new Text("Gold: " + game.Gold, font);
        goldText.CharacterSize = 8;
        goldText.FillColor = Color.White;
        x = view.Size.X - goldText.GetGlobalBounds().Width - margin;
        y = margin + scoreText.GetGlobalBounds().Height + spacing;
        goldText.Position = new Vector2f(x - 15, y);
        DrawText(goldText, target, states);

        var torchesText = new Text("Torches: " + game.TorchesLit + "/" + game.TorchCount, font);
        torchesText.CharacterSize = 8;
        torchesText.FillColor = Color.White;
        x = view.Size.X / 2 - torchesText.GetGlobalBounds().Width / 2;
        y = margin;
        torchesText.Position = new Vector2f(x, y);
        DrawText(torchesText, target, states);

        var levelText = new Text("Level: " + game.Level, font);
        levelText.CharacterSize = 8;
        levelText.FillColor = Color.White;
        x = view.Size.X / 2 - levelText.GetGlobalBounds().Width / 2;
        y = view.Size.Y - levelText.GetGlobalBounds().Height - margin;
        levelText.Position = new Vector2f(x, y);
        DrawText(levelText, target, states);
    }
}