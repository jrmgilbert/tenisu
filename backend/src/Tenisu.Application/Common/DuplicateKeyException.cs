namespace Tenisu.Application.Common;

public sealed class DuplicateKeyException(Exception innerException) : Exception("A unique key is already taken.", innerException);