namespace iTextSharp.text.rtf.document.output;

/// <summary>
///     The RtfFileCache is a RtfDataCache that uses a temporary file
///     to store the rtf document data. Not so fast, but doesn't use any
///     memory (just disk space).
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfDiskCache : IRtfDataCache
{
    /// <summary>
    ///     The BufferedOutputStream that stores the cache data.
    /// </summary>
    private readonly BufferedStream _data;

    /// <summary>
    ///     The temporary file to store the data in.
    /// </summary>
    private readonly string _tempFile;

    /// <summary>
    ///     Constructs a RtfFileCache. Creates the temp file.
    ///     @throws IOException If the temporary file could not be created.
    /// </summary>
    public RtfDiskCache()
    {
        _tempFile = Path.GetTempFileName();
        _data = new BufferedStream(new FileStream(_tempFile, FileMode.Create));
    }

    /// <summary>
    ///     Gets the BufferedOutputStream to write to.
    /// </summary>
    public Stream GetOutputStream() => _data;

    /// <summary>
    ///     Writes the content of the temporary file into the Stream.
    /// </summary>
    public void WriteTo(Stream target)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        _data.Dispose();
        var tempIn = new BufferedStream(new FileStream(_tempFile, FileMode.Open));
        var buffer = new byte[8192];
        int bytesRead;
        while ((bytesRead = tempIn.Read(buffer, 0, buffer.Length)) > 0)
        {
            target.Write(buffer, 0, bytesRead);
        }

        tempIn.Dispose();
        File.Delete(_tempFile);
    }
}