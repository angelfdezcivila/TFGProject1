using System;
using UnityEngine;

public class RandomStage
{
    private static Vector2 _nodeSize = new Vector2(0.4f, 0.4f);
    private static int _columns = 45;
    private static int _rows = 90;

    
    /// <summary>
    /// Crea un escenario con las últimas variables usadas en esta clase.
    /// En caso de no haber creado antes un escenario con esta clase, se usarán las que vienen definidas por defecto.
    /// </summary>
    public static Stage CreateRandomStage()
    {
        return new Stage.Builder()
            .rows(_rows)
            .columns(_columns)
            .cellsDimension(_nodeSize)
            .build();
    }

    /// <summary>
    /// Crea un escenario con las variables que se le pase por argumento.
    /// </summary>
    /// <param name="rows">Número de filas</param>
    /// <param name="columns">Número de columnas</param>
    /// <param name="nodeSize">Dimensión de las celdas</param>
    public static Stage CreateRandomStage(int rows, int columns, Vector2 nodeSize)
    {
        _rows = rows;
        _columns = columns;
        _nodeSize = nodeSize;
        return CreateRandomStage();
    }
}