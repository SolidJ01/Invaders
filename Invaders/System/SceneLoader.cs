using System.Globalization;
using System.Text;
using Invaders.Model;
using Invaders.Model.Entities;
using Invaders.Model.Entities.Components;
using SFML.Graphics;
using SFML.System;

namespace Invaders.System;

public class SceneLoader
{
    private const string BasePath = "Assets";
    private Dictionary<string, Func<Dictionary<string, string>?, Entity>> _entityLoaders;

    public SceneLoader()
    {
        _entityLoaders = new Dictionary<string, Func<Dictionary<string, string>?, Entity>>();
        _entityLoaders.Add("bullet", (args) =>
        {
            Entity bullet = new Entity("bullet");
            if (args is not null && args.ContainsKey("animated") && bool.Parse(args?["animated"]))
            {
                List<Vector2i> offsets = new List<Vector2i>();
                if (args.ContainsKey("offsets"))
                {
                    var strings = args["offsets"].Split(':');
                    foreach (var str in strings)
                    {
                        var parsed = str.Split('.');
                        offsets.Add(new Vector2i(int.Parse(parsed[0]), int.Parse(parsed[1])));
                    }
                }

                bullet.AddComponent(new AnimatedRenderableComponent("Spritesheet",
                    new IntRect(int.Parse(args?["spritex"]), int.Parse(args?["spritey"]),
                        int.Parse(args?["spritewidth"]), int.Parse(args?["spriteheight"])), bullet, 0.4f, offsets));
            }
            else
                bullet.AddComponent(new RenderableComponent("Spritesheet",
                new IntRect(int.Parse(args?["spritex"]), int.Parse(args?["spritey"]), int.Parse(args?["spritewidth"]),
                    int.Parse(args?["spriteheight"])), bullet));
            bullet.AddComponent(new VelocityComponent(bullet));
            bullet.AddComponent(new BulletLogicComponent(float.Parse(args?["damage"]), bullet));
            List<string> tagBlacklist = new List<string>();
            if (args.ContainsKey("nocollide"))
                tagBlacklist = args?["nocollide"].Split(":").ToList();
            bullet.AddComponent(new CollidableComponent(bullet,
            [
                new Collider(
                    new RectangleShape(new Vector2f(float.Parse(args?["colliderw"]), float.Parse(args?["colliderh"]))),
                    new Vector2D(float.Parse(args?["colliderx"]), float.Parse(args?["collidery"])))
            ], tagBlacklist));
            return bullet;
        });
        _entityLoaders.Add("player", args =>
        {
            Entity player = new Entity("player", 2);
            player.Position = new Vector2D(float.Parse(args?["x"]), float.Parse(args?["y"]));
            player.AddComponent(new RenderableComponent("Spritesheet", new IntRect(0, 0, 64, 64), player));
            player.AddComponent(new VelocityComponent(new Vector2D(), player));
            player.AddComponent(new PlayerLogicComponent(player));
            player.AddComponent(new WeaponComponent(player, _entityLoaders["bullet"], new Vector2D(13, 0),
                new Vector2D(0, -500), 0.075f, new Dictionary<string, string>
                {
                    { "spritex", "64" }, { "spritey", "0" }, { "spritewidth", "5" }, { "spriteheight", "16" },
                    { "colliderw", "5" }, { "colliderh", "6" }, { "colliderx", "0" }, { "collidery", "-3" },
                    { "damage", "10" }, { "nocollide", "player" }
                }));
            player.AddComponent(new WeaponComponent(player, _entityLoaders["bullet"], new Vector2D(-13, 0),
                new Vector2D(0, -500), 0.075f, new Dictionary<string, string>
                {
                    { "spritex", "64" }, { "spritey", "0" }, { "spritewidth", "5" }, { "spriteheight", "16" },
                    { "colliderw", "5" }, { "colliderh", "6" }, { "colliderx", "0" }, { "collidery", "-3" },
                    { "damage", "10" }, { "nocollide", "player" }
                }));
            player.AddComponent(new CollidableComponent(player, [
                new Collider(new RectangleShape(new Vector2f(14, 60)), new Vector2D()),
                new Collider(new RectangleShape(new Vector2f(24, 8)), new Vector2D(19, 22)),
                new Collider(new RectangleShape(new Vector2f(24, 8)), new Vector2D(-19, 22))
            ], true));
            player.AddComponent(new HealthComponent(player, 100f));
            player.AddComponent(new ScoreComponent(player));
            return player;
        });
        _entityLoaders.Add("enemy", (args) =>
        {
            Entity enemy = new Entity("enemy", 1);
            enemy.Position = new Vector2D(float.Parse(args?["x"]), float.Parse(args?["y"]));
            enemy.AddComponent(new RenderableComponent("Spritesheet", new IntRect(128, 0, 64, 64), enemy));
            enemy.AddComponent(new VelocityComponent(new Vector2D(180, 90), enemy));
            enemy.AddComponent(new EnemyLogicComponent(enemy));
            enemy.AddComponent(new WeaponComponent(enemy, _entityLoaders["bullet"], new Vector2D(0, 16),
                new Vector2D(0, 100), 0.5f, new Dictionary<string, string>
                {
                    { "spritex", "69" }, { "spritey", "0" }, { "spritewidth", "16" }, { "spriteheight", "16" },
                    { "colliderw", "14" }, { "colliderh", "14" }, { "colliderx", "0" }, { "collidery", "0" },
                    { "damage", "25" }, { "nocollide", "enemy" }, {"animated", "true"}, {"offsets", "0.0:16.0:32.0:16.0"}
                }));
            enemy.AddComponent(
                new CollidableComponent(enemy, [new Collider(new CircleShape(30), new Vector2D())], true));
            enemy.AddComponent(new HealthComponent(enemy, 100f));
            return enemy;
        });
        _entityLoaders.Add("wall", (args) =>
        {
            Entity wall = new Entity("wall");
            wall.Position = new Vector2D(float.Parse(args?["x"]), float.Parse(args?["y"]));
            List<string> tagBlacklist = new List<string>();
            if (args.ContainsKey("nocollide"))
                tagBlacklist = args?["nocollide"].Split(":").ToList();
            wall.AddComponent(new CollidableComponent(wall, [
                new Collider(
                    new RectangleShape(new Vector2f(float.Parse(args?["width"]), float.Parse(args?["height"]))),
                    new Vector2D())
            ], tagBlacklist, true));
            return wall;
        });
        _entityLoaders.Add("teleporter", (args) =>
        {
            Entity teleporter = new Entity("teleporter");
            teleporter.Position = new Vector2D(float.Parse(args?["x"]), float.Parse(args?["y"]));
            teleporter.AddComponent(new TriggerVolumeLogicComponent((entity, scene) =>
            {
                bool relative = bool.Parse(args?["relative"]);
                Vector2D target = new Vector2D(float.Parse(args?["targetx"]), float.Parse(args?["targety"]));
                if (relative)
                    entity.Position += target;
                else
                    entity.Position = target;
            }, teleporter));
            teleporter.AddComponent(new CollidableComponent(teleporter,
            [
                new Collider(
                    new RectangleShape(new Vector2f(float.Parse(args?["width"]), float.Parse(args?["height"]))),
                    new Vector2D())
            ], ["wall"]));
            return teleporter;
        });

        _entityLoaders.Add("spawner", (args) =>
        {
            Entity spawner = new Entity("spawner");
            spawner.AddComponent(new SpawnerLogicComponent(_entityLoaders[args?["entity"]],
                int.Parse(args?["maxspawned"]), args?["entity"], float.Parse(args?["basespawninterval"]), float.Parse(args?["targetspawninterval"], CultureInfo.InvariantCulture), float.Parse(args?["deltaspawninterval"], CultureInfo.InvariantCulture),
                int.Parse(args?["xmin"]), int.Parse(args?["xmax"]), int.Parse(args?["ymin"]), int.Parse(args?["ymax"]),
                spawner));
            return spawner;
        });
        _entityLoaders.Add("hud", (args) =>
        {
            Entity hud = new Entity("hud", 5);
            UIRenderableComponent healthElement = new UIRenderableComponent("pixel-font", 18, hud);
            healthElement.SetPosition(new Vector2D(float.Parse(args?["healthx"]), float.Parse(args?["healthy"])));
            UIRenderableComponent scoreElement = new UIRenderableComponent("pixel-font", 18, hud, UIRenderableComponent.Alignment.Right);
            scoreElement.SetPosition(new Vector2D(float.Parse(args?["scorex"]), float.Parse(args?["scorey"])));
            hud.AddComponent(new HUDLogicComponent(healthElement, scoreElement, hud));
            hud.AddComponent(healthElement);
            hud.AddComponent(scoreElement);
            return hud;
        });
        _entityLoaders.Add("gameoverui", (args) =>
        {
            Entity ui = new Entity("gameoverui", 5);
            UIMenuLogicComponent uiLogic = new UIMenuLogicComponent(
                [
                    new MenuItem("Restart", (scene) => { scene.QueueLoading(scene.CurrentScene); }),
                    new MenuItem("Main menu", (scene) => { scene.QueueLoading("MainMenu"); })
                ],
                new Vector2D(float.Parse(args?["x"]), float.Parse(args?["y"])), "pixel-font", 26, ui,
                UIRenderableComponent.Alignment.Center, false
            );
            ui.AddComponent(new GameOverUILogicComponent(uiLogic, ui));
            ui.AddComponent(uiLogic);
            return ui;
        });
        _entityLoaders.Add("mainmenuui", (args) =>
        {
            Entity ui = new Entity("mainmenuui", 5);
            ui.AddComponent(new UIMenuLogicComponent(
                [
                new MenuItem("New Game", (scene) => { scene.QueueLoading("Level1"); }),
                new MenuItem("Exit to Desktop", (scene) => {})
                ],
                new Vector2D(float.Parse(args?["x"]), float.Parse(args?["y"])), "pixel-font", 26, ui, UIRenderableComponent.Alignment.Center
            ));
            return ui;
        });
    }

    public void LoadScene(Scene scene, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        scene.Clear();

        string file = $"{BasePath}/{fileName}.txt";
        string[] lines = File.ReadAllLines(file, Encoding.UTF8);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] entity = line.Split(' ');
            if (entity.Length > 2)
            {
                Console.WriteLine("Incorrect entity formatting");
                continue;
            }

            string[] args = entity[1].Split(',');
            Dictionary<string, string> parsedArgs = new Dictionary<string, string>();
            foreach (string arg in args)
            {
                string[] argParts = arg.Split('=');
                parsedArgs.Add(argParts[0], argParts[1]);
            }

            Entity? created = null;
            if (_entityLoaders.TryGetValue(entity[0], out var loader))
            {
                created = loader(parsedArgs);
            }

            if (created is not null)
                scene.QueueSpawn(created);
        }

        scene.CurrentScene = fileName;
    }

    public void ReloadScene(Scene scene)
    {
        LoadScene(scene, scene.CurrentScene);
    }
}