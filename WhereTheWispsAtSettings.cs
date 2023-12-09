using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace WhereTheWispsAt
{
    public class WhereTheWispsAtSettings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        public ToggleNode DrawMap { get; set; } = new ToggleNode(true);
        public ColorNode YellowWisp { get; set; } = new ColorNode(Color.Yellow);
        public RangeNode<int> YellowSize { get; set; } = new RangeNode<int>(5, 1, 100);
        public ColorNode BlueWisp { get; set; } = new ColorNode(Color.SkyBlue);
        public RangeNode<int> BlueSize { get; set; } = new RangeNode<int>(5, 1, 100);
        public ColorNode PurpleWisp { get; set; } = new ColorNode(Color.Purple);
        public RangeNode<int> PurpleSize { get; set; } = new RangeNode<int>(5, 1, 100);
        public ColorNode LightBomb { get; set; } = new ColorNode(Color.White);
        public RangeNode<int> LightBombSize { get; set; } = new RangeNode<int>(10, 1, 100);
        public ColorNode Wells { get; set; } = new ColorNode(Color.Orange);
        public RangeNode<int> WellsSize { get; set; } = new RangeNode<int>(5, 1, 100);
        public ColorNode FuelRefill { get; set; } = new ColorNode(Color.Green);
        public RangeNode<int> FuelRefillSize { get; set; } = new RangeNode<int>(20, 1, 100);
        public ColorNode Altars { get; set; } = new ColorNode(Color.Red);
        public RangeNode<int> AltarSize { get; set; } = new RangeNode<int>(20, 1, 100);
        public ColorNode DustConverters { get; set; } = new ColorNode(Color.HotPink);
        public RangeNode<int> DustConverterSize { get; set; } = new RangeNode<int>(20, 1, 100);
    }
}