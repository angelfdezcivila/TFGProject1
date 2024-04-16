using System;

namespace JsonDataManager.Stage.ShapeType
{
    [Serializable]
    public abstract class ShapeType
    {
        public string NameRepresentation { get; private set; }

        public ShapeType(string nameRepresentation)
        {
            this.NameRepresentation = nameRepresentation;
        }

        public override string ToString()
        {
            return NameRepresentation;
        }
    }
}