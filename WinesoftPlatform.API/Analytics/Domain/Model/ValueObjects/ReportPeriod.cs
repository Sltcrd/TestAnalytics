namespace WinesoftPlatform.API.Analytics.Domain.Model.ValueObjects;

/// <summary>
/// Value object representing a report period
/// </summary>
/// <param name="StartDate">Start date of the period</param>
/// <param name="EndDate">End date of the period</param>
public record ReportPeriod(DateTime StartDate, DateTime EndDate)
{
    public ReportPeriod() : this(DateTime.MinValue, DateTime.MinValue)
    {
    }

    public int DaysDuration => (EndDate - StartDate).Days;

    public void Validate()
    {
        if (StartDate > EndDate)
            throw new ArgumentException("Start date cannot be after end date");
        
        if (EndDate > DateTime.UtcNow)
            throw new ArgumentException("End date cannot be in the future");
    }
}