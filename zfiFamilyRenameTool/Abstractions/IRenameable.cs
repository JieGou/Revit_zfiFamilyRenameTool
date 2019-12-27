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
        string Destination { get; set; }

        /// <summary>
        /// ������� �����������
        /// </summary>
        string GroupCondition { get; }

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