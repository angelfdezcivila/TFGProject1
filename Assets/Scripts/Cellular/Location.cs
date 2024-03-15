using System;

namespace Cellular
{
    [Serializable]
    public record Location(int Row, int Column)
    {
        public int Row { get; set; } = Row;
        public int Column { get; set; } = Column;
    }
}