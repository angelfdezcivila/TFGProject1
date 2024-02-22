using UnityEngine;

// Una implementación alternativa para la creación del esenario
namespace TestingStageWithBuilder
{
    public sealed class RandomStageWithBuilder : StageWithBuilder
    {
        
        /// <summary>
        /// Crea un escenario con unos argumentos por defecto.
        /// </summary>
        /// <param name="cellsPrefab">Prefab del objeto celda que almacena el tipo a instanciar</param>
        /// <param name="transform">Transform del padre de las celdas a instanciar</param>
        public static StageWithBuilder getRandomStage(GameObject cellsPrefab, Transform transform)
        {
            return getRandomStage(cellsPrefab, transform, 45, 90, new Vector2(0.4f, 0.4f));
        }
        
        /// <summary>
        /// Crea un escenario con las variables que se le pase por argumento.
        /// </summary>
        /// <param name="cellsPrefab">Prefab del objeto celda que almacena el tipo a instanciar</param>
        /// <param name="transform">Transform del padre de las celdas a instanciar</param>
        /// <param name="rows">Número de filas</param>
        /// <param name="columns">Número de columnas</param>
        /// <param name="cellsSize">Dimensión de las celdas</param>
        public static StageWithBuilder getRandomStage(GameObject cellsPrefab, Transform transform, int rows, int columns, Vector2 cellsSize)
        {
            return new StageWithBuilder.Builder()
                .rows(rows)
                .columns(columns)
                .cellsDimension(cellsSize)
                .cellsPrefab(cellsPrefab)
                .parent(transform).build();
        }

        public RandomStageWithBuilder(GameObject cellPrefab, Transform transformParent) : base(cellPrefab, transformParent)
        {
        }

        public RandomStageWithBuilder(GameObject cellPrefab, Vector2 cellsDimension, Transform transformParent, int rows, int columns) : base(cellPrefab, cellsDimension, transformParent, rows, columns)
        {
        }
    }
}