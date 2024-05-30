using System;

namespace JsonDataManager.Stage.ShapeType
{
    [Serializable]
    public abstract class ShapeType
    {
        public string NameRepresentation { get; private set; }

        protected ShapeType(string nameRepresentation)
        {
            this.NameRepresentation = nameRepresentation;
        }

        public override string ToString()
        {
            return NameRepresentation;
        }
    }
}