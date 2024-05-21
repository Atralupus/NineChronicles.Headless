using GraphQL.Types;

namespace NineChronicles.Headless.GraphTypes.States;

public class RootStateDiffType : ObjectGraphType<RootStateDiffType.Value>
{
    public class Value
    {
        public string Path { get; }
        public StateDiffType.Value[] Diffs { get; }

        public Value(string path, StateDiffType.Value[] diffs)
        {
            Path = path;
            Diffs = diffs;
        }
    }

    public RootStateDiffType()
    {
        Name = "RootStateDiff";

        Field<NonNullGraphType<StringGraphType>>(
            "Path",
            description: "The path to the root state difference."
        );

        Field<ListGraphType<StateDiffType>>(
            "Diffs",
            description: "List of state differences under this root."
        );
    }
}
