using System.Text;

namespace DotNetDotEnv.AspNetCore.Tests;

public sealed class TempFile : IDisposable
{
    private bool _disposed;

    public FileInfo File { get; }

    public TempFile() : this(Path.GetTempFileName()) { }

    public TempFile(string fileName)
    {
        File = new FileInfo(fileName);

        using (File.Open(FileMode.OpenOrCreate))
        {
            // Ensure file exists.
        }
    }

    public TempFile(string fileName, ReadOnlySpan<byte> content)
    {
        File = new FileInfo(fileName);

        using var stream = File.Open(FileMode.Create);
        stream.Write(content);
    }

    public TempFile(string fileName, ReadOnlySpan<char> content, Encoding? encoding = null)
    {
        File = new FileInfo(fileName);

        using var stream = File.Open(FileMode.Create);
        using var writer = new StreamWriter(stream, encoding ?? Encoding.UTF8);
        writer.Write(content);
    }

    ~TempFile() => Dispose(disposing: false);

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                try
                {
                    File.Delete();
                }
                catch
                {
                    // It's more graceful if we don't throw in dispose code.
                }
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
