
#pragma once

#include <windows.h>
#include <d3dx9.h>
#include <dxgi.h>


#ifdef XMLDOCS
using System::InvalidOperationException;
using System::ArgumentException;
using System::ArgumentNullException;
using System::ArgumentOutOfRangeException;
using System::NotSupportedException;
using System::IO::EndOfStreamException;
#endif

namespace V3
{
	ref class Utilities sealed
	{
	private:
		Utilities();
		
	public:
		static System::Guid ConvertNativeGuid( const GUID &guid );
		static GUID ConvertManagedGuid( System::Guid guid );
		
		static int SizeOfFormatElement( DXGI_FORMAT format );
		
		static System::Drawing::Rectangle ConvertRect(RECT rect);

		static System::String^ BufferToString( ID3DXBuffer *buffer );


		generic<typename T> where T : value class
		static array<T>^ ReadRange( ID3DXBuffer *buffer, int count );

		//These doc comments are mostly intended to copy to other places.

		/// <summary>
		/// Checks that a range to be read are within the boundaries of a source.
		/// </summary>
		/// <param name="lowerBound">The minimum bound that can be read from the source, and fills in the number of elements to read if necessary.</param>
		/// <param name="size">The total size of the source.</param>
		/// <param name="offset">The index at which the caller intends to begin reading from the source.</param>
		/// <param name="count">The number of elements intended to be read from the source. If 0 is passed, count will be adjusted to be size - offset.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset" /> is less than <paramref name="lowerBound" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="count" /> is negative.</exception>
		/// <exception cref="ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.</exception>
		static void CheckBounds( int lowerBound, int size, int offset, int% count );

		/// <summary>
		/// Checks that a range to be read is within the boundaries of a source array, and fills in the number of elements to read if necessary.
		/// </summary>
		/// <param name="data">The source array to be read from.</param>
		/// <param name="offset">The index at which the caller intends to begin reading from the source.</param>
		/// <param name="count">The number of elements intended to be read from the source. If 0 is passed, count will be adjusted to be size - offset.</param>
		/// <exception cref="ArgumentNullException"><paramref name="data" /> is a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.</exception>
		static void CheckArrayBounds( System::Array^ data, int offset, int% count );
		
		generic<typename T>
		static bool CheckElementEquality( array<T>^ left, array<T>^ right );
		generic<typename T>
		static bool CheckElementEquality( System::Collections::Generic::IList<T>^ left, System::Collections::Generic::IList<T>^ right );
		
		static void FreeNativeString( LPCSTR string );
		static void FreeNativeString( LPSTR string );

		static LPSTR AllocateNativeString( System::String^ string );

		generic<typename T> where T : value class
		static T FromIntToT( int value );
	};
}
