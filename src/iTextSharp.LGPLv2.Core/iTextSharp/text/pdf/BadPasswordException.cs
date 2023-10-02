namespace iTextSharp.text.pdf;

public class BadPasswordException : IOException
{
    public BadPasswordException(string message) : base(message)
    {
    }

    public BadPasswordException()
    {
    }

    public BadPasswordException(string message, Exception innerException) : base(message, innerException)
    {
    }

	public BadPasswordException(Func<byte[], PasswordTestResult> tester) : this()
	{
		this.Tester = tester;
	}

	public BadPasswordException(string message, Func<byte[], PasswordTestResult> tester) : this(message)
	{
		this.Tester = tester;
	}

	public BadPasswordException(string message, Exception innerException, Func<byte[], PasswordTestResult> tester) : this(message, innerException)
	{
		this.Tester = tester;
	}

	public PasswordTestResult? TryPassword(byte[] password)
	{
		if (Tester is null) return null;
		return Tester.Invoke(password);
	}

	public bool CanTryPassword => Tester is not null;

	internal Func<byte[], PasswordTestResult> Tester { get; }

	public enum PasswordTestResult
	{
		SuccessOwnerPassword, SuccessUserPassword, Fail,
	}
}