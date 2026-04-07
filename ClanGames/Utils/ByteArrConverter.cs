namespace ClanGames.Utils;

public static class ByteArrConverter
{
	public static byte[] FileToByteArray(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
			throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
		if (!File.Exists(filePath))
			throw new FileNotFoundException($"File not found: {filePath}", filePath);
		return File.ReadAllBytes(filePath);
	}
}
