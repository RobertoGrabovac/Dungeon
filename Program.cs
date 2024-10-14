using Dungeon.Game;
using Dungeon.Menu;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

const int DEFAULT_WIDTH = 1280;
const int DEFAULT_HEIGHT = 720;

var win = new RenderWindow(new VideoMode(DEFAULT_WIDTH, DEFAULT_HEIGHT), "Dungeon", Styles.Default);

win.SetVerticalSyncEnabled(true);
win.Closed += (sender, eventArgs) => win.Close();


var backgroundColor = new Color(37, 19, 26);
Game? game = null;
var menu = new Menu(win);
var timer = new Clock();
bool done = false;

while (win.IsOpen)
{
    // WINDOW EVENTS
    win.DispatchEvents();

    // CONTROL PHASE
    if (menu.startGame)
    {
        float dt = timer.ElapsedTime.AsSeconds();
        if (game != null)
            done = game.Update(dt);
        else
            game = new Game(win, menu.personalBest, menu.playerDamage, menu.playerSpeed, menu.playerLives,
                menu.difficulty); // inicijalno pokretanje igre
        
        if (done) // partija gotova (jer je igrač umro)
        {
            done = false; // igra se nastavlja beskonačno nakon smrti, nakon povratka u meni
            menu.coins += game.Gold;
            if (game.HighScore > menu.personalBest)
                menu.personalBest = game.HighScore;
            menu.SaveDataToFile();
            menu.startGame = false; // pokrenut će se meni
            menu.Reset();
            game = null;
        }
    }
    else
        menu.Update();

    win.Clear(backgroundColor);
    timer.Restart();

    // DRAW PHASE
    if (menu.startGame)
        game?.Draw(win, RenderStates.Default);
    else
        menu.Draw();

    win.Display();
}