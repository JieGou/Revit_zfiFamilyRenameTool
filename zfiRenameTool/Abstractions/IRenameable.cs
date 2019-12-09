namespace zfiRenameTool.Abstractions
{
    public interface IRenameable
    {
        string Title { get; }

        string Source { get; }

        string Destination { get; set; }

        void Rename();

        bool CanRename();
    }
}