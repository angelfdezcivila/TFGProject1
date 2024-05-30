using Cellular.Neighbourhood;
using StageGenerator;

namespace Cellular.FloorFields
{
    public class DijkstraStaticFloorFieldWithMooreNeighbourhood : DijkstraStaticFloorField {
        public DijkstraStaticFloorFieldWithMooreNeighbourhood(Stage scenario)
            : base(scenario, MooreNeighbourhood.Of) 
        {
        }

        public static DijkstraStaticFloorFieldWithMooreNeighbourhood Of(Stage scenario) {
            return new DijkstraStaticFloorFieldWithMooreNeighbourhood(scenario);
        }
    }
}