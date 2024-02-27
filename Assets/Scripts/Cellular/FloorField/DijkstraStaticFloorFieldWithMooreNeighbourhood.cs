using StageGenerator;

public class DijkstraStaticFloorFieldWithMooreNeighbourhood : DijkstraStaticFloorField {
    public DijkstraStaticFloorFieldWithMooreNeighbourhood(Stage scenario)
        : base(scenario, MooreNeighbourhood.of) 
    {
    }

    public static DijkstraStaticFloorFieldWithMooreNeighbourhood of(Stage scenario) {
        return new DijkstraStaticFloorFieldWithMooreNeighbourhood(scenario);
    }
}