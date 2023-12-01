namespace AdventOfCode
{
    public enum RunMode
    {
        Test,
        Real,
    }

    public abstract class BaseTestableDay: BaseDay
    {
        internal RunMode RunMode { get; set; }

        protected override string InputFileExtension =>
            RunMode switch
            {
                RunMode.Test => "test.txt",
                RunMode.Real => ".txt",
                _ => throw new ArgumentException("Invalid enum value for RunMode", nameof(RunMode)),
            };
    }
}
