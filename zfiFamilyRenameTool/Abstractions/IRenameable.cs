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
        string Destination { get; }

        /// <summary>
        /// Условие группировки
        /// </summary>
        string GroupCondition { get; }

        /// <summary>
        /// Установить новое значение для <see cref="Destination"/>
        /// </summary>
        /// <param name="value">Новое значение</param>
        void SetNewDestination(string value);

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