namespace zfiFamilyRenameTool.Abstractions
{
    public interface IRenameable
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Исходное значение
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Новое значение
        /// </summary>
        string Destination { get; set; }

        /// <summary>
        /// Условие группировки
        /// </summary>
        string GroupCondition { get; }

        /// <summary>
        /// Переименовать
        /// </summary>
        void Rename();

        /// <summary>
        /// Возможно ли переименовать
        /// </summary>
        bool CanRename();
    }
}