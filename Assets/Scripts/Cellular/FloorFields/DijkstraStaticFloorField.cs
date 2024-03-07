using System;
using System.Collections.Generic;
using System.Linq;
using StageGenerator;
using UnityEngine;

namespace FloorFields
{
  public class DijkstraStaticFloorField : StaticFloorField {
    protected readonly Neighbourhood neighbourhood;

    public DijkstraStaticFloorField(Stage scenario, Func<Stage, Neighbourhood> buildNeighbourhood)
      : base(new float[scenario.Rows,scenario.Columns], scenario)
    {
      this.neighbourhood = buildNeighbourhood.Invoke(scenario);
    }

    public static DijkstraStaticFloorField of(Stage scenario, Func<Stage, Neighbourhood> buildNeighbourhood) {
      return new DijkstraStaticFloorField(scenario, buildNeighbourhood);
    }
  
    public override void initialize() {
      // Compute the shortest distances to any exit from each node
      // var priorityQueue = new PriorityQueue<MyNode>();
      Queue<MyNode> priorityQueue = new Queue<MyNode>();
      // List<MyNode> priorityQueue = new List<MyNode>();
  
      // Initially distance to any exit is 0 and to any other non-blocked cell is Infinity
      for (int i = 0; i < getRows(); i++) {
        for (int j = 0; j < getColumns(); j++) {
          if (scenario.IsCellExit(i, j)) {
            staticFloorField[i,j] = 0;
            priorityQueue.Enqueue(new MyNode(i, j, staticFloorField[i,j]));
            // priorityQueue.Add(new MyNode(i, j, staticFloorField[i,j]));  // Using a list instead of a Queue
          } else if (scenario.IsCellBlocked(i, j)) {
            staticFloorField[i,j] = (float)Double.MaxValue;
          } else {
            staticFloorField[i,j] = (float)Double.MaxValue;
          }
        }
      }
    
      priorityQueue.OrderBy(node => node.priority);
  
      float maxDistance = 0; // will store distance for non-blocked cell that is furthest away from an exit
  
      while (priorityQueue.Count != 0) {
        var node = priorityQueue.Dequeue();
        // var node = priorityQueue[0];
        // priorityQueue.RemoveAt(0);   // Using a list instead of a Queue
        float nodeDistance = staticFloorField[node.row,node.column];
        if (node.priority == nodeDistance) {
          // This is first extraction of node from PQ, hence it corresponds to its optimal cost, which is already
          // recorded in staticFloorField.
          // Now that we know optimal cost for node, let's compute alternative costs to its neighbours and
          // update if they improve current ones
          foreach (var neighbour in neighbourhood.Neighbours(node.row, node.column)) {
            if (!scenario.IsCellBlocked(neighbour)) {
              int rowdiff = neighbour.Row - node.row;
              int coldiff = neighbour.Column - node.column;
              float delta = Mathf.Sqrt(rowdiff*rowdiff + coldiff*coldiff);
              float newNeighbourDistance = nodeDistance + delta;
              if (newNeighbourDistance < staticFloorField[neighbour.Row, neighbour.Column]) {
                // Shorter distance to neighbour was found: update
                staticFloorField[neighbour.Row, neighbour.Column] = newNeighbourDistance;
                priorityQueue.Enqueue(new MyNode(neighbour.Row, neighbour.Column, newNeighbourDistance));
                // priorityQueue.Add(new MyNode(neighbour.row, neighbour.column, newNeighbourDistance));  // Using a list instead of a Queue
              }
            }
            priorityQueue.OrderBy(node => node.priority);
          }
          if (nodeDistance > maxDistance) {
            // A cell that is furthest away from an exit was found
            maxDistance = nodeDistance;
          }
        }
      }
  
      // Normalize so that the closer to an exit the larger the static field
      for (int i = 0; i < getRows(); i++) {
        for (int j = 0; j < getColumns(); j++) {
          if (!scenario.IsCellBlocked(i, j)) {
            staticFloorField[i,j] = 1 - staticFloorField[i,j] / maxDistance;
          }
        }
      }
    }
  
    private record MyNode(int row, int column, double priority){
      public int compareTo(MyNode that) {
        if (this.priority > that.priority)
          return 1;
        else if (this.priority < that.priority)
          return -1;
        else
          return 0;
      }

      public double priority { get; } = priority;
      public int column { get; } = column;
      public int row { get; } = row;
    }
  }
}
