#pragma once

namespace V3
{
	// NOTE: The enumerations defined in this file are in alphabetical order. When
	//       adding new enumerations or renaming existing ones, please make sure
	//       the ordering is maintained.
	
	/// <summary>
	/// Specifies possible performance profiling options.
	/// </summary>
	[System::Flags]
	public enum class PerformanceOptions : System::Int32
	{
		/// <summary>
		/// No performance options specified.
		/// </summary>
		None = 0,

		/// <summary>
		/// Do not allow performance profiling.
		/// </summary>
		DoNotAllowProfiling = 1,
	};

	/// <summary>
	/// Specifies possible behaviors of result watches.
	/// </summary>
	[System::Flags]
	public enum class ResultWatchFlags : System::Int32
	{
		/// <summary>
		/// Always ignore the result.
		/// </summary>
		AlwaysIgnore = 0,

		/// <summary>
		/// Break whenever the result occurs.
		/// </summary>
		Break = 1,

		/// <summary>
		/// Throw an exception whenever the result occurs.
		/// </summary>
		Throw = 2,
	};
}