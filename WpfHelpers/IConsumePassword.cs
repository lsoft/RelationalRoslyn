using System.Security;

namespace WpfHelpers
{
    /// <summary>
    /// ����������� ������
    /// </summary>
    public interface IConsumePassword
    {
        /// <summary>
        /// ���������� ������ ����, ��� ��� ���������� �� ���� �����, ��� �� ���������
        /// </summary>
        /// <param name="password">������</param>
        void SetPassword(SecureString password);
    }
}