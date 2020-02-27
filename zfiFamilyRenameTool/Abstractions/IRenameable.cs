namespace zfiFamilyRenameTool.Abstractions
{
    public interface IRenameable
    {
        /// <summary>
        /// ���������
        /// </summary>
        string Title { get; }

        /// <summary>
        /// �������� ��������
        /// </summary>
        string Source { get; }

        /// <summary>
        /// ����� ��������
        /// </summary>
        string Destination { get; }

        /// <summary>
        /// ������� �����������
        /// </summary>
        string GroupCondition { get; }

        /// <summary>
        /// ���������� ����� �������� ��� <see cref="Destination"/>
        /// </summary>
        /// <param name="value">����� ��������</param>
        void SetNewDestination(string value);

        /// <summary>
        /// �������������
        /// </summary>
        void Rename();

        /// <summary>
        /// �������� �� �������������
        /// </summary>
        bool CanRename();
    }
}