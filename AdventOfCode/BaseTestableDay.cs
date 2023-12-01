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

    public record Answer(string Value)
    {
        public static implicit operator Answer(string value) => new(value);
        public static implicit operator Answer(int value) => new(value.ToString());
        public static implicit operator Answer(long value) => new(value.ToString());
        public static implicit operator Answer(ulong value) => new(value.ToString());
        public static implicit operator Answer(uint value) => new(value.ToString());

        public static implicit operator string(Answer answer) => answer.Value;
    }
}
