using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Helpers;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Vector3N = System.Numerics.Vector3;
using Vector2N = System.Numerics.Vector2;

namespace WhereTheWispsAt;

public class WhereTheWispsAt : BaseSettingsPlugin<WhereTheWispsAtSettings>
{
    public record WispData(
        List<Entity> Purple,
        List<Entity> Yellow,
        List<Entity> Blue,
        List<Entity> LightBomb,
        List<Entity> Wells,
        List<Entity> FuelRefill,
        List<Entity> Altars,
        List<Entity> DustConverters,
        List<Entity> Dealer,
        List<Entity> Chests,
        Dictionary<Entity, string> Encounters);

    public WispData Wisps = new([], [], [], [], [], [], [], [], [], [], []);

    public override bool Initialise() => true;

    public WhereTheWispsAt()
    {
        Name = "Where The Wisps At";
    }
    public enum WispType
    {
        None,
        Yellow,
        Purple,
        Blue,
        Chests,
        LightBomb,
        Wells,
        FuelRefill,
        Altars,
        DustConverters,
        Dealer,
        Encounter
    }

    public override Job Tick()
    {
        var wellsToRemove = Wisps.Wells
            .Where(well => well.TryGetComponent<StateMachine>(out var stateComp)
                           && stateComp?.States.Any(x => x.Name == "activated" && x.Value == 1) == true)
            .ToList();

        wellsToRemove.ForEach(well => RemoveEntityFromList(well, Wisps.Wells));

        var altarsToRemove = Wisps.Altars
            .Where(altar => altar.TryGetComponent<StateMachine>(out var stateComp)
                            && stateComp?.States.Any(x => x.Name == "activated" && x.Value == 1) == true)
            .ToList();

        altarsToRemove.ForEach(altar => RemoveEntityFromList(altar, Wisps.Altars));

        var dustConvertersToRemove = Wisps.DustConverters
            .Where(converter => converter.TryGetComponent<StateMachine>(out var stateComp)
                                && stateComp?.States.Any(x => x.Name == "activated" && x.Value == 1) == true)
            .ToList();

        dustConvertersToRemove.ForEach(altar => RemoveEntityFromList(altar, Wisps.DustConverters));

        foreach (var chest in Wisps.Chests.Where(x => x.GetComponent<Chest>()?.IsOpened != false).ToList())
        {
            RemoveEntityFromList(chest, Wisps.Chests);
        }

        return null;
    }

    public override void EntityAdded(Entity entity)
    {
        string path = entity.TryGetComponent<Animated>(out var animatedComp) ? animatedComp?.BaseAnimatedObjectEntity?.Path : null;

        var metadata = entity.Metadata;
        switch (metadata)
        {
            case "Metadata/MiscellaneousObjects/Azmeri/AzmeriResourceBase":
                if (path != null)
                {
                    if (path.Contains("League_Azmeri/resources/wisp_doodads/wisp_primal")) Wisps.Blue.Add(entity);
                    else if (path.Contains("League_Azmeri/resources/wisp_doodads/wisp_warden")) Wisps.Yellow.Add(entity);
                    else if (path.Contains("League_Azmeri/resources/wisp_doodads/wisp_vodoo")) Wisps.Purple.Add(entity);
                }

                break;

            case "Metadata/MiscellaneousObjects/Azmeri/AzmeriLightBomb":
                Wisps.LightBomb.Add(entity);
                break;

            case "Metadata/MiscellaneousObjects/Azmeri/AzmeriFuelResupply":
                Wisps.FuelRefill.Add(entity);
                break;

            case "Metadata/MiscellaneousObjects/Azmeri/AzmeriFlaskRefill":
                Wisps.Wells.Add(entity);
                break;

            case "Metadata/NPC/League/Affliction/GlyphsHarvestTree":
                Wisps.Encounters[entity] = "Harvest";
                break;

            case "Metadata/MiscellaneousObjects/Azmeri/AzmeriBuffEffigySmall":
            case "Metadata/MiscellaneousObjects/Azmeri/AzmeriBuffEffigyMedium":
            case "Metadata/MiscellaneousObjects/Azmeri/AzmeriBuffEffigyLarge":
                Wisps.Encounters[entity] = "Buff";
                break;

            case "Metadata/Monsters/LeagueAzmeri/VoodooKingBoss/VoodooKingBoss":
            case "Metadata/Monsters/LeagueAzmeri/VoodooKingBoss/VoodooKingBoss2":
            case "Metadata/Monsters/LeagueAzmeri/VoodooKingBoss/VoodooKingBoss3":
            case "Metadata/NPC/Ghostrider":
            case "Metadata/NPC/League/Affliction/GlyphsEtching01":
            case "Metadata/NPC/League/Affliction/GlyphsEtching02":
            case "Metadata/NPC/League/Affliction/GlyphsEtching03":
            case "Metadata/NPC/League/Affliction/GlyphsEtching04":
            case "Metadata/NPC/League/Affliction/GlyphsEtching05":
            case "Metadata/NPC/League/Affliction/GlyphsGoddessStatue":
            case "Metadata/NPC/League/Affliction/GlyphsGruthkulShrine":
            case "Metadata/NPC/League/Affliction/GlyphsKingGlyphOne":
            case "Metadata/NPC/League/Affliction/GlyphsKingGlyphTwo":
            case "Metadata/NPC/League/Affliction/GlyphsMajiProclamation01":
            case "Metadata/NPC/League/Affliction/GlyphsMajiProclamation02":
            case "Metadata/NPC/League/Affliction/GlyphsSingleStatue":
            case "Metadata/NPC/League/Affliction/GlyphsWarringSisters":
                Wisps.Encounters[entity] = metadata[(metadata.LastIndexOf('/') + 1)..];
                break;

            case "Metadata/Chests/LeagueAzmeri/OmenChest":
                Wisps.Encounters[entity] = "Omen Chest";
                break;

            case not null when metadata.Contains("Azmeri/SacrificeAltarObjects"):
                Wisps.Altars.Add(entity);
                break;

            case not null when metadata.Contains("Azmeri/AzmeriDustConverter"):
                Wisps.DustConverters.Add(entity);
                break;

            case not null when metadata.Contains("Azmeri/UniqueDealer"):
                Wisps.Dealer.Add(entity);
                break;

            case not null when metadata.StartsWith("Metadata/Chests/LeagueAzmeri/"):
                Wisps.Chests.Add(entity);
                break;
        }
    }

    public override void EntityRemoved(Entity entity)
    {
        new[] { Wisps.Blue, Wisps.Purple, Wisps.Yellow, Wisps.LightBomb, Wisps.Wells, Wisps.FuelRefill }
            .ToList()
            .ForEach(list => RemoveEntityFromList(entity, list));
        Wisps.Encounters.Remove(entity);
    }

    private static void RemoveEntityFromList(Entity entity, List<Entity> list)
    {
        var entityToRemove = list.FirstOrDefault(wisp => wisp.Id == entity.Id);
        if (entityToRemove != null) list.Remove(entityToRemove);
    }

    public override void AreaChange(AreaInstance area) => Wisps = new([], [], [], [], [], [], [], [], [], [], []);

    public override void Render()
    {
        if (!Settings.Enable.Value || !GameController.InGame) return;

        var ingameUi = GameController.Game.IngameState.IngameUi;
        if (!Settings.IgnoreFullscreenPanels && ingameUi.FullscreenPanels.Any(x => x.IsVisible))
        {
            return;
        }

        if (!Settings.IgnoreLargePanels && ingameUi.LargePanels.Any(x => x.IsVisible))
        {
            return;
        }

        foreach (var (list, color, size, text, type) in new[]
        {
            (Wisps.Yellow, Settings.YellowWisp, Settings.YellowSize.Value, null, WispType.Yellow),
            (Wisps.Purple, Settings.PurpleWisp, Settings.PurpleSize.Value, null, WispType.Purple),
            (Wisps.Blue, Settings.BlueWisp, Settings.BlueSize.Value, null, WispType.Blue),
            (Wisps.Chests, Settings.ChestColor, Settings.ChestSize.Value, null, WispType.Chests),
            (Wisps.LightBomb, Settings.LightBomb, 0, "Light Bomb", WispType.LightBomb),
            (Wisps.Wells, Settings.Wells, 0, "Well", WispType.Wells),
            (Wisps.FuelRefill, Settings.FuelRefill, 0, "Fuel Refill", WispType.FuelRefill),
            (Wisps.Altars, Settings.Altars, 0, "Altar", WispType.Altars),
            (Wisps.DustConverters, Settings.DustConverters, 0, "Dust Converter", WispType.DustConverters),
            (Wisps.Dealer, Settings.Dealer, 0, "! TRADER !", WispType.Dealer)
        })
        {
            DrawWisps(list, color, size, text, type);
        }

        foreach (var (entity, text) in Wisps.Encounters)
        {
            DrawWisps([entity], Settings.EncounterColor, 0, text, WispType.Encounter);
        }

        foreach (var chest in Wisps.Chests)
        {
            if (chest.DistancePlayer < Settings.ChestScreenDisplayMaxDistance &&
                chest.TryGetComponent<Render>(out var render))
            {
                Graphics.DrawBoundingBoxInWorld(chest.PosNum, Settings.ChestColor.Value with { A = (byte)Settings.ChestAlpha }, render.BoundsNum, render.RotationNum.X);
            }
        }

        void DrawWisps(List<Entity> entityList, Color color, int size, string text, WispType type = WispType.None)
        {
            // Just run this once, land looks flat.
            float groundZ = entityList.FirstOrDefault()?.GridPosNum is Vector2N gridPosNum
                            ? GameController.IngameState.Data.GetTerrainHeightAt(gridPosNum)
                            : 0;

            foreach (var entity in entityList)
            {
                if (Settings.DrawMap && GameController.IngameState.IngameUi.Map.LargeMap.IsVisibleLocal)
                {
                    var mapPos = GameController.IngameState.Data.GetGridMapScreenPosition(entity.PosNum.WorldToGrid());

                    if (text != null)
                    {
                        var widthPadding = 3;
                        var boxOffset = Graphics.MeasureText(text) / 2f;
                        var textOffset = boxOffset;

                        boxOffset.X += widthPadding;

                        Graphics.DrawBox(mapPos - boxOffset, mapPos + boxOffset, Color.Black);
                        Graphics.DrawText(text, mapPos - textOffset, color);
                    }
                    else
                    {
                        Graphics.DrawBox(new RectangleF(mapPos.X - size / 2, mapPos.Y - size / 2, size, size), color, 1f);
                    }
                }

                var specificWispTypes = new[] { WispType.Yellow, WispType.Purple, WispType.Blue, WispType.LightBomb, WispType.FuelRefill};

                if (Settings.DrawWispsOnGround && specificWispTypes.Contains(type))
                {
                    var entityPos = entity.PosNum;

                    RectangleF screensize = new RectangleF()
                    {
                        X = 0,
                        Y = 0,
                        Width = GameController.Window.GetWindowRectangleTimeCache.Size.Width,
                        Height = GameController.Window.GetWindowRectangleTimeCache.Size.Height
                    };

                    Vector2N entityPosScreen = RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen(entityPos);
                    if (IsEntityWithinScreen(entityPosScreen, screensize, 50))
                    {
                        Graphics.DrawBoundingBoxInWorld(entityPos with { Z = groundZ }, color with { A = (byte)Settings.WispsOnGroundAlpha }, new Vector3N(Settings.WispsOnGroundWidth, Settings.WispsOnGroundWidth, Settings.WispsOnGroundHeight), 0f);
                    }
                }
            }
        }
    }
    private static bool IsEntityWithinScreen(Vector2N entityPos, RectangleF screensize, float allowancePX)
    {
        // Check if the entity position is within the screen bounds with allowance
        float leftBound = screensize.Left - allowancePX;
        float rightBound = screensize.Right + allowancePX;
        float topBound = screensize.Top - allowancePX;
        float bottomBound = screensize.Bottom + allowancePX;

        return entityPos.X >= leftBound && entityPos.X <= rightBound &&
               entityPos.Y >= topBound && entityPos.Y <= bottomBound;
    }
}