
#pragma once

#include "Enums.h"
#include "Result.h"

using System::Runtime::InteropServices::OutAttribute;

namespace V3
{
	/// <summary>
	/// Used to control global options that affect all of SlimDX.
	/// </summary>
	public ref class Configuration sealed
	{
	private:
		static System::Collections::Generic::Dictionary<Result,ResultWatchFlags>^ m_Watches;
	
		static Configuration();
		
		Configuration();
	
	internal:
		static bool TryGetResultWatch( Result result, [Out] ResultWatchFlags% flags );

	public:
		/// <summary>
		/// Gets or sets whether SlimDX is currently tracking call stacks from object creation. If set to <c>false</c>,
		/// objects will not carry a call stack from when they were created. The default value is <c>false</c>.
		/// </summary>
		/// <remarks>Object tracking is a useful debugging facility, but may have a significant negative
		/// impact on performance. The default value is <c>false</c>.</remarks>
		static property bool EnableObjectTracking;

		/// <summary>
		/// Gets or sets whether SlimDX defaults to throwing exceptions on <see cref="Result">result codes</see>
		/// that indicate errors. The default value is <c>true</c>.
		/// </summary>
		static property bool ThrowOnError;

		/// <summary>
		/// Gets or sets whether SlimDX detects double disposal of objects. If set to <c>true</c>, SlimDX will throw
		/// an <see cref="System::ObjectDisposedException"/> when an already disposed object is disposed. If set to <c>false</c>, double
		/// disposals will be silently ignored. The default value is <c>false</c>.
		/// </summary>
		static property bool DetectDoubleDispose;

		/// <summary>
		/// Gets or sets the SlimDX wide timer object.
		/// </summary>
		/// <remarks>This Timer is automatically created and started when SlimDX is loaded.</remarks>
		static property System::Diagnostics::Stopwatch^ Timer;

		/// <summary>
		/// Adds a watch indicating the action to be taken when a method returns the
		/// specified <see cref="Result">result code</see>.
		/// </summary>
		/// <param name="result">The result code to watch for.</param>
		/// <param name="flags">The action to take when the specified result code occurs.</param>
		static void AddResultWatch( Result result, ResultWatchFlags flags );

		/// <summary>
		/// Removes a watch on the specified <see cref="Result">result code</see>.
		/// If no watch was set, this call is ignored.
		/// </summary>
		/// <param>The result code to stop watching.</param>
		static void RemoveResultWatch( Result result );

		/// <summary>
		/// Clears out all watches on all <see cref="Result">result codes</see>.
		/// </summary>
		static void ClearResultWatches();
	};
}