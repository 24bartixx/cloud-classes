namespace Shared;

public abstract class CustomEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public abstract string Message { get; set; }
}

public class Type1Event:  CustomEvent
{
    public override string Message { get; set; } = "Type 1 Data";
}

public class Type2Event:  CustomEvent
{
    public override string Message { get; set; } = "Type 2 Data";
}

public class Type3Event:  CustomEvent
{
    public override string Message { get; set; } = "Type 3 Data";
}

public class Type4Event:  CustomEvent
{
    public override string Message { get; set; } = "Type 4 Data - Generated from Type 3";
}
