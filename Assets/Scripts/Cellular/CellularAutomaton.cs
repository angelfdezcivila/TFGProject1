// using System;
// using System.Collections.Generic;
// using StageGenerator;
// using TMPro.SpriteAssetUtilities;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// public class CellularAutomaton {
//
//   #region Protected variables
//
//   /**
//    * Scenario where simulation takes place.
//    */
//   protected Stage scenario;
//   /**
//    * Parameters describing this automaton.
//    */
//   protected CellularAutomatonParameters parameters;
//   /**
//    * Neighbourhood relationship used by this automaton.
//    */
//   protected Neighbourhood neighbourhood;
//   /**
//    * {@code true} if cell is occupied by a pedestrian in current discrete state.
//    */
//   protected bool[,] occupied;
//   /**
//    * {@code true} if cell will be occupied by a pedestrian in next discrete state.
//    */
//   protected bool[,] occupiedNextState;
//   /**
//    * Factory for generating pedestrians for this automaton.
//    */
//   protected readonly PedestrianFactory pedestrianFactory;
//   /**
//    * List of pedestrians currently within the scenario.
//    */
//   protected readonly List<Pedestrian> inScenarioPedestrians;
//   /**
//    * List of pedestrians that have evacuated the scenario.
//    */
//   protected readonly List<Pedestrian> outOfScenarioPedestrians;
//   /**
//    * Number of discrete time steps elapsed since the start of the simulation.
//    */
//   protected int timeSteps;
//   
//   #endregion
//
//   #region Properties
//
//   public int Rows => scenario.Rows;
//
//   public int Columns => scenario.Columns;
//
//   #endregion
//
//   #region Constructor
//
//   /**
//    * Creates a new Cellular Automaton with provided parameters.
//    *
//    * @param parameters parameters describing this automaton.
//    */
//   public CellularAutomaton(CellularAutomatonParameters parameters) {
//     this.parameters = parameters;
//     this.scenario = parameters.Scenario;
//     this.neighbourhood = parameters.Neighbourhood;
//     this.occupied = new bool[scenario.Rows,scenario.Columns];
//     this.occupiedNextState = new bool[scenario.Rows,scenario.Columns];
//     this.pedestrianFactory = new PedestrianFactory(this);
//
//     // this.inScenarioPedestrians = Collections.synchronizedList(new List<>());
//     this.inScenarioPedestrians = new List<Pedestrian>();
//     this.outOfScenarioPedestrians = new List<Pedestrian>();
//     Reset();
//   }
//   
//   /**
//  * Resets state of automaton. All pedestrians are removed, cells became non-occupied
//  * and elapsed time steps are set to 0.
//  */
//   private void Reset() {
//     ClearCells(occupied);
//     inScenarioPedestrians.Clear();
//     outOfScenarioPedestrians.Clear();
//     timeSteps = 0;
//   }
//   
//   #endregion
//
//   private void ClearCells(bool[,] cells) {
//
//     for (int i = 0; i < cells.GetLength(0); i++)
//     {
//       for (int j = 0; j < cells.GetLength(1); j++)
//       {
//         cells[i, j] = false;
//       }
//     }
//   }
//
//   /**
//    * Adds a new pedestrian to this automaton.
//    *
//    * @param row        row of scenario where new pedestrian should be placed.
//    * @param column     column of scenario where new pedestrian should be placed.
//    * @param parameters parameters describing new pedestrian.
//    * @return {@code true} if pedestrian could be created (location was neither blocked nor taken by another pedestrian).
//    */
//   public bool AddPedestrian(int row, int column, PedestrianParameters parameters) {
//     if (row < 0 || row >= Rows) throw new ArgumentOutOfRangeException(nameof(row), "AddPedestrian: invalid row");
//     if (column < 0 || column >= Columns) throw new ArgumentOutOfRangeException(nameof(column), "AddPedestrian: invalid column");
//     if (IsCellReachable(row, column)) {
//       var pedestrian = pedestrianFactory.GetInstance(row, column, parameters);
//       occupied[row, column] = true;
//       inScenarioPedestrians.Add(pedestrian);
//       return true;
//     } else {
//       return false;
//     }
//   }
//
//   /**
//    * Adds a new pedestrian to this automaton.
//    *
//    * @param location   location in scenario where new pedestrian should be placed.
//    * @param parameters parameters describing new pedestrian.
//    * @return {@code true} if pedestrian could be created (location was neither blocked nor taken by another pedestrian).
//    */
//   public bool AddPedestrian(Location location, PedestrianParameters parameters) {
//     return AddPedestrian(location.row, location.column, parameters);
//   }
//
//   /**
//    * Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
//    *
//    * @param numberOfPedestrians number of new pedestrian to add.
//    * @param parameters          parameters describing new pedestrians.
//    */
//   public void AddPedestriansUniformly(int numberOfPedestrians, PedestrianParameters parameters) {
//     AddPedestriansUniformly(numberOfPedestrians, () -> parameters);
//   }
//
//   /**
//    * Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
//    *
//    * @param numberOfPedestrians number of new pedestrian to add.
//    * @param parametersSupplier  a supplier providing parameters describing each new pedestrians.
//    */
//   public void AddPedestriansUniformly(int numberOfPedestrians, Supplier<PedestrianParameters> parametersSupplier) {
//     if(numberOfPedestrians < 0)
//       throw new ArgumentException("AddPedestriansUniformly: number of pedestrian cannot be negative");
//     var numberOfPedestriansPlaced = 0;
//     while (numberOfPedestriansPlaced < numberOfPedestrians) {
//       var row = Random.Range(0, Rows);
//       var column = Random.Range(0, Columns);
//
//       if (AddPedestrian(row, column, parametersSupplier.get())) {
//         numberOfPedestriansPlaced++;
//       }
//     }
//   }
//
//   /**
//    * Returns neighbours of a cell in this automaton (will depend on neighbourhood relationship).
//    *
//    * @param row    row of cell.
//    * @param column column of cell.
//    * @return neighbours a cell.
//    */
//   public List<Location> Neighbours(int row, int column) {
//     if (row < 0 || row >= Rows)
//       throw new ArgumentException("Neighbours: invalid row");
//     if (column < 0 || column >= Columns)
//       throw new ArgumentException("Neighbours: invalid column");
//
//     return neighbourhood.Neighbours(row, column);
//   }
//
//   /**
//    * Returns neighbours of a cell in this automaton (will depend on neighbourhood relationship).
//    *
//    * @param location location of cell.
//    * @return neighbours a cell.
//    */
//   public List<Location> Neighbours(Location location) {
//     return Neighbours(location.row, location.column);
//   }
//
//   /**
//    * Checks whether a cell is occupied by some pedestrian.
//    *
//    * @param row    row of cell to check.
//    * @param column column of cell to check.
//    * @return {@code true} if cell is occupied by some pedestrian.
//    */
//   public bool IsCellOccupied(int row, int column) {
//     if (row < 0 || row >= Rows)
//       throw new ArgumentException("IsCellOccupied: invalid row");
//     if (column < 0 || column >= Columns)
//       throw new ArgumentException("IsCellOccupied: invalid column");
//
//     return occupied[row, column];
//   }
//
//   /**
//    * Checks whether a cell is occupied by some pedestrian.
//    *
//    * @param location location of cell to check.
//    * @return {@code true} if cell is occupied by some pedestrian.
//    */
//   public bool IsCellOccupied(Location location) {
//     return IsCellOccupied(location.row, location.column);
//   }
//
//   /**
//    * Checks whether a cell can be reached by some pedestrian (i.e. there is no pedestrian occupying the cell and the
//    * cell is not blocked in the scenario).
//    *
//    * @param row    row of cell to check.
//    * @param column column of cell to check.
//    * @return {@code true} if cell can be reached by some pedestrian.
//    */
//   public bool IsCellReachable(int row, int column) {
//     if (row < 0 || row >= Rows)
//       throw new ArgumentException("isCellReachable: invalid row");
//     if (column < 0 || column >= Columns)
//       throw new ArgumentException("isCellReachable: invalid column");
//     
//     return !occupied[row,column] && !scenario.IsCellBlocked(row, column);
//   }
//
//   /**
//    * Checks whether a cell can be reached by some pedestrian (i.e. there is no pedestrian occupying the cell and the
//    * cell is not blocked in the scenario).
//    *
//    * @param location location of cell to check.
//    * @return {@code true} if cell can be reached by some pedestrian.
//    */
//   public bool IsCellReachable(Location location) {
//     return IsCellReachable(location.row, location.column);
//   }
//
//   /**
//    * Checks whether some pedestrian has decided already to move to a cell in next discrete time step of simulation.
//    *
//    * @param row    row of cell to check.
//    * @param column column of cell to check.
//    * @return {@code true} if some pedestrian has decided already to move to cell in next discrete time step of
//    * simulation.
//    */
//   public bool WillBeOccupied(int row, int column) {
//     if (row < 0 || row >= Rows)
//       throw new ArgumentException("willBeOccupied: invalid row");
//     if (column < 0 || column >= Columns)
//       throw new ArgumentException("willBeOccupied: invalid column");
//     
//     return occupiedNextState[row,column];
//   }
//
//   /**
//    * Checks whether some pedestrian has decided already to move to a cell in next discrete time step of simulation.
//    *
//    * @param location location of cell to check.
//    * @return {@code true} if some pedestrian has decided already to move to cell in next discrete time step of
//    * simulation.
//    */
//   public bool WillBeOccupied(Location location) {
//     return WillBeOccupied(location.row, location.column);
//   }
//
//   /**
//    * Scenario where automaton is running.
//    *
//    * @return scenario where automaton is running.
//    */
//   public Stage GetScenario() {
//     return scenario;
//   }
//
//   /**
//    * Runs one discrete time step for this automaton.
//    */
//   public void timeStep() {
//     // clear new state
//     ClearCells(occupiedNextState);
//
//     // move each pedestrian
//     synchronized (inScenarioPedestrians) {
//       // in order to process pedestrians in random order
//       random.shuffle(inScenarioPedestrians);
//
//       var pedestriansIterator = inScenarioPedestrians.iterator();
//       while (pedestriansIterator.hasNext()) {
//         var pedestrian = pedestriansIterator.next();
//         int row = pedestrian.getRow();
//         int column = pedestrian.getColumn();
//
//         if (scenario.IsCellExit(row, column)) {
//           // pedestrian exits scenario
//           pedestrian.setExitTimeSteps(timeSteps);
//           outOfScenarioPedestrians.Add(pedestrian);
//           pedestriansIterator.remove();
//         } else {
//           pedestrian.chooseMovement().ifPresentOrElse(
//               location -> {
//                 if (WillBeOccupied(location)) {
//                   // new location already taken by another pedestrian. Don't move
//                   occupiedNextState[row,column] = true;
//                   pedestrian.doNotMove();
//                 } else {
//                   // move to new location
//                   occupiedNextState[location.row(),location.column()] = true;
//                   pedestrian.moveTo(location);
//                 }
//               },
//               // no new location to consider. Don't move
//               () -> {
//                 occupiedNextState[row,column] = true;
//                 pedestrian.doNotMove();
//               }
//           );
//         }
//       }
//     }
//     // make next state current one
//     var temp = occupied;
//     occupied = occupiedNextState;
//     occupiedNextState = temp;
//
//     timeSteps++;
//   }
//
//   /**
//    * Thread for running the simulation.
//    */
//   private class RunThread : Thread {
//     final Canvas canvas;
//
//     public RunThread(Canvas canvas) {
//       this.canvas = canvas;
//     }
//
//     public void run() {
//       scenario.getStaticFloorField().initialize();
//       timeSteps = 0;
//       var maximalTimeSteps = parameters.TimeLimit() / parameters.TimePerTick();
//
//       if (canvas != null) {
//         // show initial configuration for 1.5 seconds
//         canvas.update();
//         try {
//           Thread.sleep(1500);
//         } catch (Exception ignored) {
//         }
//       }
//
//       var millisBefore = System.currentTimeMillis();
//       while (!inScenarioPedestrians.isEmpty() && timeSteps < maximalTimeSteps) {
//         timeStep();
//         if (canvas != null) {
//           canvas.update();
//           var elapsedMillis = (System.currentTimeMillis() - millisBefore);
//           try {
//             // wait some milliseconds to synchronize animation
//             Thread.sleep(((int) (parameters.TimePerTick() * 1000) - elapsedMillis) / parameters.GUITimeFactor());
//             millisBefore = System.currentTimeMillis();
//           } catch (Exception ignored) {
//           }
//         }
//       }
//       if (canvas != null) {
//         // show final configuration
//         canvas.update();
//       }
//     }
//   }
//
//   /**
//    * Runs this automaton until end conditions are met.
//    *
//    * @param gui if this parameter is {@code true} the simulation is displayed in a GUI.
//    */
//   private void run(bool gui) {
//     Canvas canvas = null;
//     if (gui) {
//       canvas =
//           new Canvas.Builder()
//               .rows(scenario.getRows())
//               .columns(scenario.getColumns())
//               .pixelsPerCell(10)
//               .paint(CellularAutomaton.this::paint)
//               .build();
//
//       new TexturePacker_JsonArray.Frame(canvas);
//     }
//     var thread = new RunThread(canvas);
//     thread.start();
//     try {
//       thread.join(); // wait for thread to complete
//     } catch (InterruptedException e) {
//       System.out.println("Interrupted!");
//     }
//   }
//
//   /**
//    * Runs this automaton until end conditions are met.
//    */
//   public void run() {
//     run(false);
//   }
//
//   /**
//    * Runs this automaton until end conditions are met and displays simulation in a GUI.
//    */
//   public void runGUI() {
//     run(true);
//   }
//
//   /**
//    * Returns number of evacuees (number of pedestrians that have evacuated scenario).
//    *
//    * @return number of evacuees.
//    */
//   public int numberOfEvacuees() {
//     return outOfScenarioPedestrians.size();
//   }
//
//   /**
//    * Returns number of non evacuees (number of pedestrians still inside scenario).
//    *
//    * @return number of non evacuees.
//    */
//   public int numberOfNonEvacuees() {
//     return inScenarioPedestrians.size();
//   }
//
//   /**
//    * Returns evacuation times for evacuees.
//    * @return evacuation times for evacuees.
//    */
//   public double[] evacuationTimes() {
//     int numberOfEvacuees = numberOfEvacuees();
//     double[] times = new double[numberOfEvacuees];
//
//     int i = 0;
//     for (var evacuee : outOfScenarioPedestrians) {
//       times[i] = evacuee.getExitTimeSteps() * parameters.timePerTick();
//       i += 1;
//     }
//
//     return times;
//   }
//
//   /**
//    * Returns distances to the closest exit for each non evacuee.
//    *
//    * @return distances to closest exit for each non evacuee.
//    */
//   public double[] distancesToClosestExit() {
//     int numberOfNonEvacuees = numberOfNonEvacuees();
//     double[] shortestDistances = new double[numberOfNonEvacuees];
//
//     int i = 0;
//     for (var nonEvacuee : inScenarioPedestrians) {
//       var shortestDistance = Double.MAX_VALUE;
//       for(var exit : GetScenario().exits()) {
//         var distance = exit.distance(nonEvacuee.getLocation());
//         if (distance < shortestDistance)
//           shortestDistance = distance;
//       }
//       shortestDistances[i] = shortestDistance * GetScenario().getCellDimension();
//       i += 1;
//     }
//
//     return shortestDistances;
//   }
//
//   /**
//    * Computes some statistics regarding the execution of the simulation.
//    *
//    * @return statistics collected after running simulation.
//    */
//   public Statistics computeStatistics() {
//     int numberOfEvacuees = numberOfEvacuees();
//     double[] evacuationTimes = evacuationTimes();
//     int[] steps = new int[numberOfEvacuees];
//
//     int i = 0;
//     for (var pedestrian : outOfScenarioPedestrians) {
//       steps[i] = pedestrian.getNumberOfSteps();
//       i += 1;
//     }
//     double meanSteps = Descriptive.mean(steps);
//     double meanEvacuationTime = Descriptive.mean(evacuationTimes);
//     double medianSteps = Descriptive.median(steps);
//     double medianEvacuationTime = Descriptive.median(evacuationTimes);
//     int numberOfNonEvacuees = numberOfNonEvacuees();
//
//     return new Statistics(meanSteps, meanEvacuationTime
//         , medianSteps, medianEvacuationTime
//         , numberOfEvacuees, numberOfNonEvacuees);
//   }
//
//   private static final Color
//       darkBlue = new Color(0, 71, 189),
//       lightBlue = new Color(0, 120, 227);
//
//   /**
//    * Paints this automaton in GUI representing the simulation.
//    *
//    * @param canvas Graphical canvas where pedestrian should be drawn.
//    */
//   void paint(Canvas canvas) {
//     scenario.paint(canvas);
//     synchronized (inScenarioPedestrians) {
//       for (var pedestrian : inScenarioPedestrians) {
//         pedestrian.paint(canvas, lightBlue, darkBlue);
//       }
//     }
//   }
//
//   private static JsonObject jsonPedestrian(int id, int domain, int row, int column) {
//     JsonObject pedestrian = new JsonObject();
//     pedestrian.put("id", id);
//
//     JsonObject location = new JsonObject();
//     location.put("domain", domain);
//
//     JsonObject coordinates = new JsonObject();
//     coordinates.put("X", column);
//     coordinates.put("Y", row);
//
//     location.put("coordinates", coordinates);
//     pedestrian.put("location", location);
//
//     return pedestrian;
//   }
//
//   private static JsonObject jsonSnapshot(double timestamp, JsonArray crowd) {
//     JsonObject snapshot = new JsonObject();
//     snapshot.put("timestamp", timestamp);
//     snapshot.put("crowd", crowd);
//     return snapshot;
//   }
//
//   /**
//    * Json representing traces of all pedestrians through the scenario.
//    *
//    * @return Json representing traces of all pedestrians through the scenario.
//    */
//   public JsonObject jsonTrace() {
//     var domain = 0; // todo currently there is only a single domain
//
//     // Create an empty JsonArray for the snapshots
//     JsonArray snapshots = new JsonArray();
//
//     List<Pedestrian> allPedestrians = new ArrayList<>();
//     allPedestrians.addAll(inScenarioPedestrians);
//     allPedestrians.addAll(outOfScenarioPedestrians);
//     allPedestrians.sort(Comparator.comparing(Pedestrian::getIdentifier));
//
//     // Create snapshots
//     for (int t = 0; t < timeSteps; t++) {
//       JsonArray crowd = new JsonArray();
//       for (var pedestrian : allPedestrians) {
//         var path = pedestrian.getPath();
//         if (path.size() > t) {
//           var location = path.get(t);
//           crowd.add(jsonPedestrian(pedestrian.getIdentifier()
//               , domain
//               , location.row()
//               , location.column()));
//         }
//       }
//       snapshots.add(jsonSnapshot(t, crowd));
//     }
//
//     // Create the final JsonObject with the snapshots array
//     JsonObject result = new JsonObject();
//     result.put("snapshots", snapshots);
//
//     return result;
//   }
// }