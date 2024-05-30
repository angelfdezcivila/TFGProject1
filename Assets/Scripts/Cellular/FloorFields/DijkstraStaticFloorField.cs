using System;
using System.Collections.Generic;
using System.Linq;
using Cellular.Neighbourhood;
using StageGenerator;
using UnityEngine;

namespace Cellular.FloorFields
{
  public class DijkstraStaticFloorField : StaticFloorField {
    private readonly INeighbourhood _neighbourhood;

    public DijkstraStaticFloorField(Stage scenario, Func<Stage, INeighbourhood> buildNeighbourhood)
      : base(new double[scenario.Rows,scenario.Columns], scenario)
    {
      this._neighbourhood = buildNeighbourhood.Invoke(scenario);
    }

    public static DijkstraStaticFloorField Of(Stage scenario, Func<Stage, INeighbourhood> buildNeighbourhood) {
      return new DijkstraStaticFloorField(scenario, buildNeighbourhood);
    }
  
    public override void Initialize() {
      // Compute the shortest distances to any exit from each node
      Queue<MyNode> priorityQueue = new Queue<MyNode>();
      // List<MyNode> priorityQueue = new List<MyNode>();
  
      // Initially distance to any exit is 0 and to any other non-blocked cell is Infinity
      for (int i = 0; i < GetRows(); i++) {
        for (int j = 0; j < GetColumns(); j++) {
          if (scenario.IsCellExit(i, j)) {
            staticFloorField[i,j] = 0;
            priorityQueue.Enqueue(new MyNode(i, j, staticFloorField[i,j]));
            // priorityQueue.Add(new MyNode(i, j, staticFloorField[i,j]));  // Using a list instead of a Queue
          } else if (scenario.IsCellObstacle(i, j)) {
            staticFloorField[i,j] = (float)Double.MaxValue;
          } else {
            staticFloorField[i,j] = (float)Double.MaxValue;
          }
        }
      }
    
      priorityQueue.OrderBy(node => node.Priority);
  
      double maxDistance = 0; // will store distance for non-blocked cell that is furthest away from an exit
  
      while (priorityQueue.Count != 0) {
        var node = priorityQueue.Dequeue();
        // var node = priorityQueue[0];
        // priorityQueue.RemoveAt(0);   // Using a list instead of a Queue
        double nodeDistance = staticFloorField[node.Row,node.Column];
        if (node.Priority == nodeDistance) {
          // This is first extraction of node from PQ, hence it corresponds to its optimal cost, which is already
          // recorded in staticFloorField.
          // Now that we know optimal cost for node, let's compute alternative costs to its neighbours and
          // update if they improve current ones
          foreach (var neighbour in _neighbourhood.Neighbours(node.Row, node.Column)) {
            if (!scenario.IsCellObstacle(neighbour)) {
              int rowdiff = neighbour.Row - node.Row;
              int coldiff = neighbour.Column - node.Column;
              float delta = Mathf.Sqrt(rowdiff*rowdiff + coldiff*coldiff);
              double newNeighbourDistance = nodeDistance + delta;
              if (newNeighbourDistance < staticFloorField[neighbour.Row, neighbour.Column]) {
                // Shorter distance to neighbour was found: update
                staticFloorField[neighbour.Row, neighbour.Column] = newNeighbourDistance;
                priorityQueue.Enqueue(new MyNode(neighbour.Row, neighbour.Column, newNeighbourDistance));
                // priorityQueue.Add(new MyNode(neighbour.row, neighbour.column, newNeighbourDistance));  // Using a list instead of a Queue
              }
            }
            priorityQueue.OrderBy(node => node.Priority);
          }
          if (nodeDistance > maxDistance) {
            // A cell that is furthest away from an exit was found
            maxDistance = nodeDistance;
          }
        }
      }
  
      // Normalize so that the closer to an exit the larger the static field
      for (int i = 0; i < GetRows(); i++) {
        for (int j = 0; j < GetColumns(); j++) {
          if (!scenario.IsCellObstacle(i, j)) {
            staticFloorField[i,j] = 1 - staticFloorField[i,j] / maxDistance;
          }
        }
      }
    }
  
    private record MyNode(int Row, int Column, double Priority){
      public int CompareTo(MyNode that) {
        if (this.Priority > that.Priority)
          return 1;
        else if (this.Priority < that.Priority)
          return -1;
        else
          return 0;
      }

      public double Priority { get; } = Priority;
      public int Column { get; } = Column;
      public int Row { get; } = Row;
    }
  }
}
