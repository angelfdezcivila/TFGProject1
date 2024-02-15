using UnityEngine;

namespace StageWithCells
{
    public sealed class RandomStageWithCells
    {
        public Vector2 _nodeSize = new Vector2(0.4f, 0.4f);


        public static StageWithCells getRandomStage(GameObject cellsPrefab, Transform transform)
        {
            // return new StageWithCells.Builder().parent(transform).build();
            return new StageWithCells.Builder()
                .rows(45)
                .columns(90)
                .cellsDimension(new Vector2(0.4f, 0.4f))
                .cellsPrefab(cellsPrefab)
                .parent(transform).build();
        }

    }
}