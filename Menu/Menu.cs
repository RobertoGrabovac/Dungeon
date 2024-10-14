using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Dungeon.Menu
{
    public enum Difficulty
    {
        EASY = 0,
        MEDIUM = 1,
        HARD = 2,
    }
    
    internal class Menu
    {
        private static readonly string saveGamePath = Path.Combine("assets", "playerData.txt");
        private View menuView;
        private RenderWindow window;
        private const float menuScale = 1.0f;
        private Font font;

        // stvari koje se prikazuju na meniju
        private Button playButton;
        private Button reduceDifficultyButton, increaseDifficultyButton;
        private Text titleText, coinsText, pbText, difficultyText, difficultyLevelText, shopItemsText, playerItemsText, playerLivesText, playerDamageText, playerSpeedText, playerSwordText;
        private List<Button> shopItems = new List<Button>();
        private List<Button> playerItems = new List<Button>();

        // iduce dvije varijable potrebne su za pravilno odabiranje item-a iz trgovine
        private Stopwatch clickCooldownTimer = new Stopwatch();
        private TimeSpan clickCooldownDuration = TimeSpan.FromSeconds(1);

        public int coins { get; set; }
        public int personalBest { get; set; }
        public bool startGame { get; set; }

        // iduca svojstva sluze za set-up igre na osnovu korisnikovih odabira na meniju
        public int playerLives { get; private set; }
        public int playerDamage { get; private set; }
        public int playerSpeed { get; set; }
        public Difficulty difficulty { get; private set; }

        public Menu(RenderWindow window)
        {
            this.window = window;
            menuView = new View(new Vector2f(window.Size.X, window.Size.Y) / (menuScale * 2),
                new Vector2f(window.Size.X, window.Size.Y) / menuScale);
            playButton = new Button(new Vector2f(980, 610), new Vector2f(250, 60), "PLAY!", new Color(61, 37, 59, 255), new Color(110, 74, 72, 255));

            difficulty = Difficulty.EASY;
            LoadDataFromFile();
            startGame = false;
            font = new Font(Path.Combine("assets", "font.ttf"));
            playerLives = 3;
            playerDamage = 1;
            playerSpeed = 60;

            // tekstovi na meniju
            titleText = new Text("DUNGEON", font, 60)
            {
                Position = new Vector2f(500, 20),
                FillColor = Color.White
            };

            coinsText = new Text("COINS: " + coins.ToString(), font, 30)
            {
                Position = new Vector2f(1280, 130),
                FillColor = Color.White
            };
            coinsText.Position = new Vector2f(coinsText.Position.X - coinsText.GetGlobalBounds().Width - 50, coinsText.Position.Y);

            pbText = new Text("PERSONAL BEST: " + personalBest.ToString(), font, 30)
            {
                Position = new Vector2f(1280, 130 + coinsText.GetGlobalBounds().Height + 30),
                FillColor = Color.White
            };
            pbText.Position = new Vector2f(pbText.Position.X - pbText.GetGlobalBounds().Width - 50, pbText.Position.Y);

            difficultyText = new Text("DIFFICULTY", font, 30)
            {
                Position = new Vector2f(1280, 350),
                FillColor = Color.White
            };
            difficultyText.Position = new Vector2f(difficultyText.Position.X - difficultyText.GetGlobalBounds().Width - 100, difficultyText.Position.Y);

            // buttoni za povecavanje i smanjivanje tezine uvijek "prate" lokaciju od teksta DIFFICULTY
            reduceDifficultyButton = new Button(difficultyText.Position + new Vector2f(-55, 35), new Vector2f(50, 50),
                "", Color.White, Color.Transparent, new Texture(Path.Combine("assets", "leftTriangle.png")));
            increaseDifficultyButton =
                new Button(difficultyText.Position + new Vector2f(difficultyText.GetGlobalBounds().Width, 35),
                    new Vector2f(50, 50), "", Color.White, Color.Transparent,
                    new Texture(Path.Combine("assets", "rightTriangle.png")));

            // tekst za tezinu levela je uvijek na sredini izmedju buttona za povecavanje i smanjivanje levela
            difficultyLevelText = new Text("EASY", font, 30)
            {
                FillColor = Color.White
            };

            shopItemsText = new Text("SHOP:", font, 30)
            {
                Position = new Vector2f(10, 130),
                FillColor = Color.White
            };

            playerItemsText = new Text("BOUGHT ITEMS:", font, 30)
            {
                Position = new Vector2f(10, 390),
                FillColor = Color.White
            };

            playerLivesText = new Text("LIVES:", font, 30)
            {
                Position = new Vector2f(10, 625),
                FillColor = Color.White
            };

            playerDamageText = new Text("DAMAGE:", font, 30)
            {
                Position = new Vector2f(450, 625),
                FillColor = Color.White
            };

            playerSpeedText = new Text("SPEED:", font, 30)
            {
                Position = playerLivesText.Position + new Vector2f(0, 45),
                FillColor = Color.White
            };

            playerSwordText = new Text("SWORD:", font, 30)
            {
                Position = playerDamageText.Position + new Vector2f(0, 45),
                FillColor = Color.White
            };

            CreateShopItems();
        }

        // logika menija
        public void Update()
        {
            if (playButton.IsMouseOver(window) && Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                startGame = true;
            }

            if (difficulty == Difficulty.EASY)
            {
                difficultyLevelText.DisplayedString = "EASY";
            }
            else if (difficulty == Difficulty.MEDIUM)
            {
                difficultyLevelText.DisplayedString = "MEDIUM";
            }
            else if (difficulty == Difficulty.HARD)
            {
                difficultyLevelText.DisplayedString = "HARD";
            }

            float z = (difficultyText.GetGlobalBounds().Width - difficultyLevelText.GetGlobalBounds().Width) / 2;
            difficultyLevelText.Position = difficultyText.Position + new Vector2f(z, 40);

            if (reduceDifficultyButton.IsMouseOver(window) && Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (!reduceDifficultyButton.Clicked)
                {
                    if (difficulty == Difficulty.MEDIUM)
                        difficulty = Difficulty.EASY;
                    else if (difficulty == Difficulty.HARD)
                        difficulty = Difficulty.MEDIUM;

                    reduceDifficultyButton.Clicked = true;
                }
            }
            else
                reduceDifficultyButton.Clicked = false;

            if (increaseDifficultyButton.IsMouseOver(window) && Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (!increaseDifficultyButton.Clicked)
                {
                    if (difficulty == Difficulty.EASY)
                        difficulty = Difficulty.MEDIUM;
                    else if (difficulty == Difficulty.MEDIUM)
                        difficulty = Difficulty.HARD;

                    increaseDifficultyButton.Clicked = true;
                }
            }
            else
                increaseDifficultyButton.Clicked = false;

            for (int i = 0; i < shopItems.Count; i++)
            {
                Button item = shopItems[i];
                if (item.IsMouseOver(window) && Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    int itemPrice = 0;
                    if (item is Sword sw)
                        itemPrice = sw.Price;
                    else if (item is Shield sh)
                        itemPrice = sh.Price;

                    if (!item.Clicked && !clickCooldownTimer.IsRunning && coins >= itemPrice)
                    {
                        coins -= itemPrice;
                        coinsText.DisplayedString = "COINS: " + coins.ToString();
                        coinsText.Position = new Vector2f(1280 - coinsText.GetGlobalBounds().Width - 50, coinsText.Position.Y);

                        item.Clicked = true;

                        if (item is DiamondSword d)
                            playerDamage = Math.Max(d.Damage, playerDamage);
                        else if (item is StoneSword s)
                            playerDamage = Math.Max(s.Damage, playerDamage);
                        else if (item is IronHelmet ih)
                            playerLives += ih.ExtraLives;
                        else if (item is IronChestplate ic)
                            playerLives += ic.ExtraLives;
                        else if (item is IronBoots boots)
                            playerSpeed += boots.Speed;

                        playerItems.Add(item);
                        shopItems.Remove(item);
                        clickCooldownTimer.Restart();
                        UpdateItemsPositions();
                    }
                }
                else
                    item.Clicked = false;
            }

            if (clickCooldownTimer.IsRunning && clickCooldownTimer.Elapsed >= clickCooldownDuration)
            {
                clickCooldownTimer.Stop();
                clickCooldownTimer.Reset();
            }

        }

        public void Draw()
        {
            window.SetView(menuView);
            
            DrawButtons();
            DrawMenuText();
            DrawShopItems();
            DrawPlayerItems();
            DrawPlayerLives();
            DrawPlayerDamage();
            DrawPlayerSpeed();
            DrawPlayerSword();
        }

        // funkcija za azuriranje novcica i najboljeg rezultata korisnika nakon zavrsene partije
        public void SaveDataToFile()
        {
            try
            {
                string[] lines = { coins.ToString(), personalBest.ToString() };

                File.WriteAllLines(saveGamePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

        // pomocne funkcije za preglednije izvrsavanje gornjih
        private void LoadDataFromFile()
        {
            try
            {
                string[] lines = File.ReadAllLines(saveGamePath);
                int Coins, PersonalBest;
                if (lines.Length >= 2 && int.TryParse(lines[0], out Coins) && int.TryParse(lines[1], out PersonalBest))
                {
                    coins = Coins;
                    personalBest = PersonalBest;
                }
                else
                {
                    Console.WriteLine("Invalid file format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
            }
        }

        private void DrawMenuText()
        {
            window.Draw(titleText);
            window.Draw(coinsText);
            window.Draw(pbText);
            window.Draw(difficultyText);
            window.Draw(difficultyLevelText);
            window.Draw(shopItemsText);
            window.Draw(playerItemsText);
            window.Draw(playerDamageText);
            window.Draw(playerLivesText);
            window.Draw(playerSpeedText);
            window.Draw(playerSwordText);
        }

        private void DrawButtons()
        {
            playButton.Draw(window);
            increaseDifficultyButton.Draw(window);
            reduceDifficultyButton.Draw(window);
        }

        private void DrawShopItems()
        {
            foreach (Button item in shopItems)
            {
                item.Draw(window);
                if (item is Shield s)
                {
                    Text txt = new Text(s.Price.ToString() + " coins", font, 20);
                    txt.Position = s.Position + new Vector2f(10, 100);
                    window.Draw(txt);
                }
                else if (item is Sword d)
                {
                    Text txt = new Text(d.Price.ToString() + " coins", font, 20);
                    txt.Position = d.Position + new Vector2f(10, 100);
                    window.Draw(txt);
                }
            }
        }

        private void DrawPlayerItems()
        {
            for (int i = 0; i < playerItems.Count; i++)
            {
                playerItems[i].Draw(window);
            }
        }

        private void DrawPlayerLives()
        {
            Vector2f pos = playerLivesText.Position;

            Texture heartTexture = new Texture(Path.Combine("assets", "heart.png"));
            Sprite heartSprite = new Sprite(heartTexture);

            float desiredWidth = 40f;
            float scaleFactorX = desiredWidth / heartSprite.GetGlobalBounds().Width;
            heartSprite.Scale = new Vector2f(scaleFactorX, scaleFactorX);


            for (int i = 0; i < playerLives; i++)
            {
                float offsetX = i * (heartSprite.GetGlobalBounds().Width + 10);
                heartSprite.Position = new Vector2f(pos.X + playerLivesText.GetGlobalBounds().Width + 10 + offsetX, pos.Y);
                window.Draw(heartSprite);
            }
        }

        private void DrawPlayerDamage()
        {
            Vector2f pos = playerDamageText.Position;

            Texture thunderTexture = new Texture(Path.Combine("assets", "damage.png"));
            Sprite thunderSprite = new Sprite(thunderTexture);

            float desiredWidth = 40f;
            float scaleFactorX = desiredWidth / thunderSprite.GetGlobalBounds().Width;
            thunderSprite.Scale = new Vector2f(scaleFactorX, scaleFactorX);


            for (int i = 0; i < playerDamage; i++)
            {
                float offsetX = i * (thunderSprite.GetGlobalBounds().Width + 10);
                thunderSprite.Position = new Vector2f(pos.X + playerDamageText.GetGlobalBounds().Width + 10 + offsetX, pos.Y);
                window.Draw(thunderSprite);
            }
        }

        private void DrawPlayerSpeed()
        {
            Vector2f pos = playerSpeedText.Position;

            Texture speedTexture = new Texture(Path.Combine("assets", "speed.png"));
            Sprite speedSprite = new Sprite(speedTexture);

            float desiredWidth = 40f;
            float scaleFactorX = desiredWidth / speedSprite.GetGlobalBounds().Width;
            speedSprite.Scale = new Vector2f(scaleFactorX, scaleFactorX);

            for (int i = 0; i < (int) (playerSpeed - 50) / 10; i++)
            {
                float offsetX = i * (speedSprite.GetGlobalBounds().Width + 10);
                speedSprite.Position = new Vector2f(pos.X + playerSpeedText.GetGlobalBounds().Width + 10 + offsetX, pos.Y - 5);
                window.Draw(speedSprite);
            }
        }

        private void DrawPlayerSword()
        {
            Vector2f pos = playerSwordText.Position;

            Texture swordTexture = new Texture(Path.Combine("assets", "woodenSword.png"));

            if (playerDamage == 2)
                swordTexture = new Texture(Path.Combine("assets", "stoneSword.png"));
            else if (playerDamage == 3)
                swordTexture = new Texture(Path.Combine("assets", "diamondSword.png"));

            Sprite swordSprite = new Sprite(swordTexture);

            float desiredWidth = 40f;
            float scaleFactorX = desiredWidth / swordSprite.GetGlobalBounds().Width;
            swordSprite.Scale = new Vector2f(scaleFactorX, scaleFactorX);


            swordSprite.Position = new Vector2f(pos.X + playerSwordText.GetGlobalBounds().Width + 10, pos.Y);
            window.Draw(swordSprite);

        }
        // vrlo bitna funkcija koja resetira meni nakon izgubljene partije --> zbog nje sve što je korisnik kupio nestaje, kako i treba biti
        public void Reset()
        {
            difficulty = Difficulty.EASY;
            LoadDataFromFile();
            startGame = false;
            playerLives = 3;
            playerDamage = 1;
            playerSpeed = 60;
            playerItems = new List<Button>();
            CreateShopItems();

            coinsText = new Text("COINS: " + coins.ToString(), font, 30)
            {
                Position = new Vector2f(1280, 130),
                FillColor = Color.White
            };
            coinsText.Position = new Vector2f(coinsText.Position.X - coinsText.GetGlobalBounds().Width - 50, coinsText.Position.Y);

            pbText = new Text("PERSONAL BEST: " + personalBest.ToString(), font, 30)
            {
                Position = new Vector2f(1280, 130 + coinsText.GetGlobalBounds().Height + 30),
                FillColor = Color.White
            };
            pbText.Position = new Vector2f(pbText.Position.X - pbText.GetGlobalBounds().Width - 50, pbText.Position.Y);

        }
        private void CreateShopItems()
        {
            shopItems = new List<Button>();
            shopItems.Add(new StoneSword());
            shopItems.Add(new DiamondSword());
            shopItems.Add(new IronHelmet());
            shopItems.Add(new IronChestplate());
            shopItems.Add(new IronBoots());
            UpdateItemsPositions();
        }

        private void UpdateItemsPositions()
        {
            // update itema u shopu
            Vector2f pos = shopItemsText.Position;
            int size = shopItems.Count;
            float spaceBetween = 40f;
            for (int i = 0; i < size; i++)
            {
                shopItems[i].Position = new Vector2f(pos.X + i * (100 + spaceBetween), pos.Y + 80);
            }

            // update itema u bought items
            Vector2f pos1 = playerItemsText.Position;
            int size1 = playerItems.Count;
            for (int i = 0; i < size1; i++)
            {
                playerItems[i].Position = new Vector2f(pos1.X + i * (100 + spaceBetween), pos1.Y + 80);
                playerItems[i].Clicked = false;
            }
        }

    }
}
