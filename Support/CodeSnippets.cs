Snippets
	//IITEncoder encoder = iTunes.CurrentEncoder;
	//status = iTunes.ConvertFile2(musicFilePath);

	using System;
	using System.Diagnostics;
	using System.IO;

	public class AudioConverter
	{
		public static void ConvertFlacToAlac(string inputFilePath, string outputFilePath)
		{
			// Ensure FFmpeg is installed and available in the system PATH
			if (!File.Exists(inputFilePath))
			{
				throw new FileNotFoundException("Input file not found.", inputFilePath);
			}

			// If output file path is not provided, create one with the same name but .m4a extension
			if (string.IsNullOrEmpty(outputFilePath))
			{
				outputFilePath = Path.ChangeExtension(inputFilePath, ".m4a");
			}

			// Use FFmpeg to convert FLAC to ALAC
			string ffmpegArguments = $"-i \"{inputFilePath}\" -c:a alac \"{outputFilePath}\"";
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				FileName = "ffmpeg",
				Arguments = ffmpegArguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (Process process = new Process { StartInfo = processStartInfo })
			{
				process.Start();

				// Capture output and error streams
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();

				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					throw new Exception($"FFmpeg conversion failed: {error}");
				}

				Console.WriteLine($"Conversion successful: {outputFilePath}");
			}
		}
	}
//////////////////////////////////////////

	3. Consider SafeHandle instead of IntPtr
	While IntPtr is standard, using a SafeHandle can help prevent memory leaks or misuse:

	You can create a SafeAudioSignatureHandle class that overrides SafeHandle and releases the handle via FreeAudioSignature.

	This is especially useful if you're returning ownership of unmanaged memory (as GetAudioSignature seems to do).

	public static class AudioSignatureWrapper
	{
		public static string? GetSignature(string filePath)
		{
			byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(filePath + '\0');
			IntPtr utf8Ptr = Marshal.AllocHGlobal(utf8Bytes.Length);
			Marshal.Copy(utf8Bytes, 0, utf8Ptr, utf8Bytes.Length);

			IntPtr resultPtr = IntPtr.Zero;

			try
			{
				resultPtr = NativeMethods.GetAudioSignature(utf8Ptr);
				return resultPtr != IntPtr.Zero
					? Marshal.PtrToStringAnsi(resultPtr)
					: null;
			}
			finally
			{
				if (resultPtr != IntPtr.Zero)
				{
					NativeMethods.FreeAudioSignature(resultPtr);
				}

				Marshal.FreeHGlobal(utf8Ptr);
			}
		}
	}

	Wrap P/Invoke in higher-level managed methods (optional)
		This depends on how it's used, but consider a managed wrapper like:

		public static byte[] GetSignature(string filePath)
		{
			IntPtr ptr = GetAudioSignature(filePath);
			try
			{
				// Convert ptr to byte[] (depends on known size or convention)
			}
			finally
			{
				FreeAudioSignature(ptr);
			}
		}

	Option 2: Marshal manually using UTF-8 (Encoding.UTF8.GetBytes())
		Wrap your call like this:

		public static string GetSignature(string filePath)
		{
			byte[] utf8Path = System.Text.Encoding.UTF8.GetBytes(filePath + "\0");
			IntPtr unmanagedPath = Marshal.AllocHGlobal(utf8Path.Length);
			Marshal.Copy(utf8Path, 0, unmanagedPath, utf8Path.Length);

			IntPtr resultPtr = IntPtr.Zero;
			try
			{
				resultPtr = NativeMethods.GetAudioSignature(unmanagedPath);
				string result = Marshal.PtrToStringAnsi(resultPtr); // base64-style, safe
				return result;
			}
			finally
			{
				if (resultPtr != IntPtr.Zero)
				{
					NativeMethods.FreeAudioSignature(resultPtr);
				}
				Marshal.FreeHGlobal(unmanagedPath);
			}
		}

	[DllImport("AudioSignature", CharSet = CharSet.Unicode, EntryPoint = "GetAudioSignature")]
	public static extern IntPtr GetAudioSignature(string filePath);

	LIB_API(char*) GetAudioSignature(const wchar_t* filePath);

	Unit Tests
		namespace DigitalZenWorks.MusicToolKit.Tests
		{
			public class AudioSignatureWrapperTests
			{
				[Fact]
				public void GetSignature_ValidFilePath_ReturnsSignature()
				{
					// Arrange
					string testFilePath = "test-audio.wav"; // Replace with a valid test file path

					// Act
					string? signature = AudioSignatureWrapper.GetSignature(testFilePath);

					// Assert
					Assert.False(string.IsNullOrEmpty(signature));
					Assert.Matches("^[A-Za-z0-9+/=]+$", signature!); // assuming base64-like format
				}

				[Fact]
				public void GetSignature_InvalidFilePath_ReturnsNull()
				{
					// Arrange
					string invalidPath = "nonexistentfile.wav";

					// Act
					string? signature = AudioSignatureWrapper.GetSignature(invalidPath);

					// Assert
					Assert.Null(signature);
				}
			}
		}
