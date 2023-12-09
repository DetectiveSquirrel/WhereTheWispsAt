using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace WhereTheWispsAt
{
    public class WhereTheWispsAtSettings : ISettings
    {
        //Mandatory setting to allow enabling/disabling your plugin
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        public ToggleNode DrawMap { get; set; } = new ToggleNode(true);
        public ColorNode YellowWisp { get; set; } = new ColorNode(Color.Yellow);
        public RangeNode<int> YellowSize { get; set; } = new RangeNode<int>(15, 1, 100);
        public ColorNode BlueWisp { get; set; } = new ColorNode(Color.SkyBlue);
        public RangeNode<int> BlueSize { get; set; } = new RangeNode<int>(15, 1, 100);
        public ColorNode PurpleWisp { get; set; } = new ColorNode(Color.Purple);
        public RangeNode<int> PurpleSize { get; set; } = new RangeNode<int>(15, 1, 100);
        public ColorNode LightBomb { get; set; } = new ColorNode(Color.Orange);
        public RangeNode<int> LightBombSize { get; set; } = new RangeNode<int>(15, 1, 100);
    }
}