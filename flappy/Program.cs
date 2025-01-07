using Microsoft.VisualBasic;
using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

//Main Class
class Program
{
    static int fps = 120; //Mit Wieviel FPS Das Spiel Laufen soll
    static Color BackgroundColor = Color.SKYBLUE;   //HinterGrundFarbe einrichten

    public static void Main()
    {
        //Fenster Initialisiern/Erstellen und auf richtige Groesse bringen
        Raylib.InitWindow(1,1, "Jumping Ball");
        Raylib.SetTargetFPS(fps);
        int height = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor());
        int width = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor());
        Raylib.SetWindowSize(width/4*3, height/4*3);
        Raylib.SetWindowPosition(width / 8, height / 8);

        //Spiel auf Pausiert Stellen
        bool paused = true;

        //game Erstellen
        Game game = new Game();

        int[] scores = LoadScores("Scores.txt");
        //ein Kommentar um Nutzer die das spiel noch Compilieren wollen/muessen darauf hinzuweisen fps einzustellen
        Console.WriteLine("Im Falle von lag problemen die fps Variable in der Program Klasse Senken " +
            "(Hoehere fps = mehr ziwchen schritte/updates welche zu einem angenehmeren spiel verlauf fuehren koennen)" +
            "Aktuell gibt es fehler mit den punkte zaehler punkte sind fuer 120 fps optimiert. (Siehe code Kommentar in Line 534");
        //Schleife waerend das Fenster Offen ist

        while (!Raylib.WindowShouldClose())
        {
            //Zeichenen Beginnen
            Raylib.BeginDrawing();
            //M fuer Pause Status aenderung
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_M) && game.IsRunning())
            {
                paused = !paused;
            }
            //durchlaeuft alle updates und Draws waernd nicht in pause oder game laeuft
            if (game.IsRunning() && !paused) {
                game.Update();
                Raylib.ClearBackground(BackgroundColor);
                game.Draw();
            }else
            {
                //falls game pausiert oder nicht laeuft
                Raylib.ClearBackground(BackgroundColor);
                game.Draw();
                //wenn nicht pausiert ausgabe "Verloren" und der Score (Geschrieben in Extra Methode)
                if (!paused)
                {
                    ArrayAddAndSort(ref scores, game.GetHighscore());
                    DrawFail(scores, game.GetHighscore() ,game.GetScore());
                    //Wenn Space Taste Gedrueckt Wird Spiel Reseted
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_M))
                    {
                        game.Reset();
                    }
                }
                else
                {
                    //Wenn pausiert ausgabe "Pausiert" (Geschrieben in Extra Methode)
                    DrawPause(scores, game.GetHighscore());
                }
            }
            //Zeichenen Beenden
            Raylib.EndDrawing();
        }
        SaveScores("Scores.txt", scores ,game.GetHighscore()); //speichert scores in eine txt datei
    }

    //Sortiert Score mit neuem pers Highscore
    static void ArrayAddAndSort(ref int[] scores, int playerHigh)
    {
        if (playerHigh > scores[scores.Length-1] && !Array.Exists(scores, item => item == playerHigh))   //wenn grösser alls niedrigster score und nicht in liste
        {
            scores[scores.Length-1] = playerHigh; //Kleinste zahl = Player Highscore
        }
        scores = Sort(scores);
    }

    //sortiert ein array von integers
    public static int[] Sort(int[] arr) //sortiert  ein array
    {
        for (int i = 0; i < arr.Length - 1; i++)    //Sortier algorythmus (Aus dem internet Kopiert) Selection Sort
        {
            int min = i;
            for (int j = i + 1; j < arr.Length; j++)
            {
                if (arr[j] > arr[min]) 
                {
                    min = j;
                }
            }
            int temp = arr[min];
            arr[min] = arr[i];
            arr[i] = temp;
        }
        return arr;
    }

    //Laedt die top 5 Highscores
    public static int[] LoadScores(string filePath)
    {
        int[] Scores = new int[5];
        //Falls keine Score Datei existiert wird eine mit 5 nullen erstellt
        if(!File.Exists(filePath)){
            using (StreamWriter sw = File.CreateText(filePath))
            {
                for(int i = 0; i<5; i++)
                {
                    sw.WriteLine(0);
                }
            }
        }
        //Liest die datei zeile fuer zeile und speichert sie in einer lisste
        using(StreamReader sr = File.OpenText(filePath))
        {
            string line;
            int counter = 0;
            while((line = sr.ReadLine()) != null)
            {
                //falls nicht nummer wird zahl ignoriert
                try
                {
                    int score = int.Parse(line);
                    Scores[counter] = score;
                }catch(Exception e)
                {
                    Console.WriteLine("Score Is not a Number");
                    break;
                }finally { counter++; }
            }
        }

        Scores = Sort(Scores);
        return Scores;
    }

    //Speichert Highscores
    public static void SaveScores(string filePath, int[] scores, int newScore)
    {
        ArrayAddAndSort(ref scores, newScore);

        using (StreamWriter sw = File.CreateText(filePath)) //nur fuer diese sequenz greift das programm auf die datei zu existent
        {
            for (int i = 0; i < scores.Length; i++)
            {
                sw.WriteLine(scores[i]);    //speichert score in txt
            }
        }

    }

    //Zeichnet Das ScoreBoard
    static void DrawScore(int[] scores, int High)
    {
        Raylib.DrawText("ScoreBoard",
            Raylib.GetScreenWidth() / 15 * 10 + 3,
            Raylib.GetScreenHeight() / 15 * 4 + 3,
            30, Color.GRAY);

        Raylib.DrawText("ScoreBoard",
        Raylib.GetScreenWidth() / 15 * 10,
        Raylib.GetScreenHeight() / 15 * 4,
        30, Color.BLACK);

        for (int i = 0; i < scores.Length; i++)
        {
            Raylib.DrawText(scores[i].ToString(),
                Raylib.GetScreenWidth() / 15 * 11 + 3,
                Raylib.GetScreenHeight() / 15 * (i + 5) + 3,
                30, Color.GRAY);

            Raylib.DrawText(scores[i].ToString(),
                Raylib.GetScreenWidth() / 15 * 11,
                Raylib.GetScreenHeight() / 15 * (i + 5),
                30, Color.BLACK);
        }

        Raylib.DrawText("Your HighScore: " + High,
            Raylib.GetScreenWidth() / 15 * 10 + 3,
            Raylib.GetScreenHeight() / 15 * 10 + 3,
            30, Color.GRAY);

        Raylib.DrawText("Your HighScore: " + High,
            Raylib.GetScreenWidth() / 15 * 10,
            Raylib.GetScreenHeight() / 15 * 10,
            30, Color.BLACK);

    }

    //Zeichnet Loose Screen
    static void DrawFail(int[] scores,int High, int score)
    {

        DrawScore(scores, High);

        //Schatten Mallen
        Raylib.DrawText("Verloren",
            Raylib.GetScreenWidth() / 15 * 2 + 3,
            Raylib.GetScreenHeight() / 15 * 5 + 3,
            100, Color.DARKGRAY);

        Raylib.DrawText("Score:  " + score,
            Raylib.GetScreenWidth() / 15 * 2 + 3,
            Raylib.GetScreenHeight() / 15 * 7 + 3,
            100, Color.DARKGRAY);

        Raylib.DrawText(" Neustarten: ",
            Raylib.GetScreenWidth() / 15 * 2+3,
            Raylib.GetScreenHeight() / 15 * 9+3,
            40, Color.LIGHTGRAY);

        Raylib.DrawText(" M ",
            Raylib.GetScreenWidth() / 15 * 2 + 3,
            Raylib.GetScreenHeight() / 15 * 10 + 3,
            40, Color.GRAY);
        //Mallen
        Raylib.DrawText("Verloren",
            Raylib.GetScreenWidth() / 15 * 2,
            Raylib.GetScreenHeight() / 15 * 5,
            100, Color.RED);
         
        Raylib.DrawText("Score:  " + score,
            Raylib.GetScreenWidth() / 15 * 2,
            Raylib.GetScreenHeight() / 15 * 7,
            100, Color.RED);

        Raylib.DrawText(" Neustarten: ",
            Raylib.GetScreenWidth() / 15 * 2,
            Raylib.GetScreenHeight() / 15 * 9,
            40, Color.BLACK);

        Raylib.DrawText(" M ",
            Raylib.GetScreenWidth() / 15 * 2,
            Raylib.GetScreenHeight() / 15 * 10,
            40, Color.BLACK);
    }

    //Zeichnet Pause Screen
    static void DrawPause(int[] scores, int High)
    {
        DrawScore(scores, High);

        //Schatten Mallen
        Raylib.DrawText(" Zum Fortfahren/Pausieren: ",
            3,
            Raylib.GetScreenHeight() / 20 * 1 + 3,
            30, Color.LIGHTGRAY);

        Raylib.DrawText(" M ",
            3,
            Raylib.GetScreenHeight() / 20 * 2 + 3,
            30, Color.GRAY);

        Raylib.DrawText(" Springen: ",
            3,
            Raylib.GetScreenHeight() / 20 * 4+3,
            30, Color.LIGHTGRAY);

        Raylib.DrawText(" Leertaste ",
            3,
            Raylib.GetScreenHeight() / 20 * 5 + 3,
            30, Color.GRAY);

        Raylib.DrawText(" Druecken ",
            3,
            Raylib.GetScreenHeight() / 20 * 7 + 3,
            30, Color.LIGHTGRAY);

        //Mallen
        Raylib.DrawText(" Zum Fortfahren/Pausieren: ",
            0,
            Raylib.GetScreenHeight() / 20 * 1,
            30, Color.BLACK);

        Raylib.DrawText(" M ",
            0,
            Raylib.GetScreenHeight() / 20 * 2,
            30, Color.BLACK);


        Raylib.DrawText(" Springen: ",
            0,
            Raylib.GetScreenHeight() / 20 * 4,
            30, Color.BLACK);

        Raylib.DrawText(" Leertaste ",
            0,
            Raylib.GetScreenHeight() / 20 * 5,
            30, Color.BLACK);

        Raylib.DrawText(" Druecken ",
            0,
            Raylib.GetScreenHeight() / 20 * 7,
            30, Color.BLACK);
    }

    // Gibt fps zurueck
    public static int GetFps()
    {
        return fps;
    }

    //Rechnet Geschwindigkeit in richtige groesse fuer target fps
    //Ziel: Spiel bleibt bei Egal welchen fps gleich schnell
    public static float PerFps(float num)
    {
        return num * (60f / fps); //als normal bedachte geschwindigkeit sind 60 fps
    }

    //Gibt BackgroundColor fuer andere Classen aus ohne Public zu Stellen
    public static Color GetBackgroundColor() 
    { 
        return BackgroundColor; 
    }
}

//Game Class
class Game
{
    //definieren und initialisieren von Variablen innerhalb von Game
    Player player = new Player();
    List<Obstacle> obstacles = new List<Obstacle>(6);
    float obstacleTimer = 2f;
    bool running = true;
    int Highscore = 0;
    //Updaten von Game
    public void Update()
    {
        //Checkt ob Spieler noch Lebt mit Extra Methode
        if (!player.IsAlive())
        {
            running = false;
        }
        Obstacle obstacle;  //reserviert speicherplatz fuer spaetere nutzung
        obstacleTimer += Raylib.GetFrameTime(); //rechnet timer hoch und kontrollier ihn
                                                //um alle 2 sekunden ein hindernis zu erzeugen
        if (obstacleTimer > 2f)
        {
            obstacleTimer = 0;
            obstacles.Add(new Obstacle());  //fuegt neues Hinderniss hinzu und setzt obstacleTimer auf 0
        }

        //Zaehlt alle obstacles rueckwearts runter da auch welche entfernt werden koennten
        player.Update();
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            obstacle = obstacles[i];    //reference von obstacles[i] zu obstacles fuer einfacherere schreib weise
            obstacle.Update();  //Updated das hindernis

            if (obstacle.IsOutOfScreen()) //Loescht Hinderniss wenn es ausser halb des bildes ist
            {
                RemoveObstacle(obstacle);
            }
            //kontrolliert ob der spieler den gefaehrlichen bereich beruehrt
            if (Raylib.CheckCollisionCircleRec(player.GetPos(), player.GetPlayerRad(), obstacle.GetNonSave()))
            {
                //wenn gefaehrlicher bereich beruert ist aber sicherer nicht wird player.Hit() ausgefuert
                if (!Raylib.CheckCollisionCircleRec(player.GetPos(), player.GetPlayerRad(), obstacle.GetSave()))
                {
                    player.Hit();
                }
                //falls spieler nichts beruhrt aber "point" bereich bekommt der score + 1
            }
            else if (Raylib.CheckCollisionCircleRec(player.GetPos(), player.GetPlayerRad(), obstacle.GetPoint()))
            {
                player.Points();
                if (player.GetScore() > Highscore)
                {
                    Highscore = player.GetScore();
                }
            }
        }
    }

    //loescht Hinderniss aus der lisste
    public void RemoveObstacle(Obstacle obstacle)
    {
        obstacles.Remove(obstacle);
    }

    //gibt vom spieler den Score weiter
    public int GetScore()
    {
        return player.GetScore();
    }

    //Gibt Highscore zurueck
    public int GetHighscore()
    {
        return Highscore;
    }

    //setzt alle variablen auf anfang zurueck um spiel neu zu starten
    public void Reset()
    {
        player = new Player();
        obstacles = new List<Obstacle>(6);
        obstacleTimer = 3;
        running = true;
    }

    //gibt zurueck ob das spiel noch laeuft
    public bool IsRunning() 
    {  
        return running; 
    }

    //Zeichnet Das Spiel
    public void Draw()
    {
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            obstacles[i].Draw(player.GetPlayerRad());
        }
        player.Draw();
        Raylib.DrawText("Score: " + player.GetScore(), Raylib.GetScreenWidth() / 15 * 7+3, Raylib.GetScreenHeight() / 15 * 2+3, 40, Color.GRAY); //Zeichnet Score Schatten
        Raylib.DrawText("Score: " + player.GetScore(), Raylib.GetScreenWidth()/15*7, Raylib.GetScreenHeight()/15*2, 40, Color.BLACK);   //Zeichnet Score
    }
}

//Player Class
class Player
{
    //Definiert und initialisiert alle Variablen der Klasse
    int score = 0;  //speichert den score
    bool isAlive = true;    // variable die speichert ob spieler lebt
    static float radius = 10f; //radius des spielers
    float speed = Program.PerFps(0f); //setzt anfangs geschwindigkeit auf 0
    Vector2 Pos = new Vector2(Raylib.GetScreenWidth() / 10, Raylib.GetScreenHeight() / 2); //Position des Spielers als Vector2 Gespeichert

    //Updated spieler
    public void Update()
    {
        //fuegt spieler die geschwindigkeit zu Y Position hinzu
        Pos.Y += speed;
        //wenn Space Taste Gedrueckt ist ist speed sofort auf -10
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            speed = Program.PerFps(-10);
        }
        else
        //Wenn nicht dann wird speed um 0.3 gesteigert
        {
            speed += Program.PerFps(Program.PerFps(0.6f)); //!!UNSICHER OB 2MAL PerFps DA ES BESCHLEUNIGUNG IST!! (PerFps^2 da es beschleunigung ist?)
        }
        //Wenn Y ausserhalb bildschirm ist Stirbt der Spieler
        if (Pos.Y < 0 || Pos.Y > Raylib.GetScreenHeight())
        {
            isAlive = false;
        }
    }

    //Gibt die Position als vector zurueck
    public Vector2 GetPos()
    {
        return Pos;
    }
    
    //Erhoeht der Score um 1
    public void Points()
    {
        score++;
    }

    //Setzt isAlive auf falsch
    public void Hit()
    {
        isAlive = false;
    }

    //Gibt den Radius des Spielers zurueck fuer kollision
    public float GetPlayerRad()
    {
        return radius;
    }

    //Gibt Radius zurueck damit andere Klassen ohne Player Variable drauf zugreifen koennen
    public static float GetRadius()
    {
        return radius;
    }
    //Gibt zurueck ob spieler noch am leben ist
    public bool IsAlive()
    {
        return isAlive;
    }

    //gibt Score zurueck
    public int GetScore()
    {
        return score;
    }

    //Zeichnet Spieler
    public void Draw()
    {
        Raylib.DrawCircleV(Pos, radius, Color.RED);
    }

}

//Obstacle Class
class Obstacle
{
    //Definiert und initialisiert Klassen Variablen
    static Random rnd = new Random(); //Random fuer die zufaellige luecke
    static float width = 50f; // Breite des Hindernisses
    bool OutOfScreen = false; //Speichert ob das Hinderniss im ausserhalb des screens ist
    static int height= Raylib.GetScreenHeight(); //speichert hoehe als int

    //Erstellt die Rechtecke: nonSave(Nicht sicher), save(Sicher) und point(Bereich in dem Score +1)
    Rectangle nonSave = new Rectangle(Raylib.GetScreenWidth(),
        0,
        width,
        height);

    Rectangle save = new Rectangle(Raylib.GetScreenWidth(),
        rnd.Next(0, Raylib.GetScreenHeight()-height/5), //zufaellige hoehe des sicheren bereiches
        width+2,
        height / 9);

    Rectangle point = new Rectangle(Raylib.GetScreenWidth() + width,
        0,
        Program.PerFps(Player.GetRadius()) * Program.PerFps(1f), //Ich finde die richtige formel fuer die verschiedensten fps nicht. ist fuer 120 fps optimiert
        height);

    //Updated Obstacle
    public void Update()
    {
        //bewegt die Hindernisse weiter nach links
        nonSave.X -= Program.PerFps(4f);
        save.X -= Program.PerFps(4f);
        point.X -= Program.PerFps(4f);
        //kontrolliert ob nonSave ausserhalb des screens ist ( - width fuer fluessigen uebergang)
        if (nonSave.X < 0 - width){
            OutOfScreen = true;
        }
    }

    //Zeichnet Hinderniss
    public void Draw(float playerRad)
    {
        Raylib.DrawRectangleRec(nonSave, Color.DARKGREEN);
        Raylib.DrawRectangleRec(new Rectangle(save.X, save.Y-playerRad, save.Width, save.Height + playerRad*2), //Damit der player nicht mit Hindernis ueberlabt
            Program.GetBackgroundColor()); //nutzt HinterGrundfarbe um wie eine luecke auszusehen
    }

    //Gibt zurueck ob Hinderniss aus dem Screen ist
    public bool IsOutOfScreen()
    {
        return OutOfScreen;
    }

    //Gibt das Rechteck der luecke zurueck
    public Rectangle GetSave()
    {
        return save;
    }

    //Gibt das Rechteck der Nicht sicheren Hindernisse zurueck
    public Rectangle GetNonSave()
    {
        return nonSave;
    }

    //Gibt Den Scoring bereich zurueck
    public Rectangle GetPoint()
    {
        return point;
    }
}