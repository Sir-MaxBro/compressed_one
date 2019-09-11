namespace Comp.General.Compression
{
    public interface ICompression<TInput, TResult>
    {
        TResult Compress(TInput source);

        TInput Decompress(TResult compressedSource);
    }
}
