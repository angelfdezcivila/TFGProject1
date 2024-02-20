public record Statistics(
    float meanSteps
    , float meanEvacuationTime
    , float medianSteps
    , float medianEvacuationTime
    , int numberOfEvacuees
    , int numberOfNonEvacuees) 
{
    
    public float meanSteps { get; set; } = meanSteps;
    public float meanEvacuationTime { get; set; } = meanEvacuationTime;
    public float medianSteps { get; set; } = medianSteps;
    public float medianEvacuationTime { get; set; } = medianEvacuationTime;
    public int numberOfEvacuees { get; set; } = numberOfEvacuees;
    public int numberOfNonEvacuees { get; set; } = numberOfNonEvacuees;
}