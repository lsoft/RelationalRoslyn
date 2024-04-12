using System.Windows.Threading;

namespace WpfHelpers
{
    /// <summary>
    /// ���������, ������� ������ �������� ����� �������� ���-�������
    /// ����� ���� ��������� ���-������ ���������� ������������ ������, ��� ��� �����������
    /// </summary>
    /// <typeparam name="TFinish">��� ���������, ������� �������� �������� ���-������ � �������� ���������� ������</typeparam>
    public interface IViewModelParent<TFinish>
    {
        /// <summary>
        /// ������������ ������ ������ ��������� ��������� WPF, ����� �������� ���-������ ����� ��� ������������
        /// </summary>
        Dispatcher Dispatcher
        {
            get;
        }

        /// <summary>
        /// ����� ���������� ���-������
        /// </summary>
        /// <param name="result">��������� ������ ���-������</param>
        void Finish(TFinish result);
    }
}