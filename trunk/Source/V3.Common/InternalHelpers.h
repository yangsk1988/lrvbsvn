
#pragma once

//For some dumbass reason, VC++ EE does not include these headers.
//SlimDX does not need them right now; if it does, just write a 
//new marshal_as based on the header version.
//#include <msclr/marshal.h>
//#include <msclr/marshal_windows.h>
#include <windows.h>

#define DX_UNREFERENCED_PARAMETER(P) (P)

#ifdef NDEBUG
#	define DX_DEBUG_UNREFERENCED_PARAMETER(P)
#else
#	define DX_DEBUG_UNREFERENCED_PARAMETER(P) (P)
#endif

namespace msclr
{
	namespace interop
	{
		template<typename ToType, typename FromType>
		inline ToType marshal_as(const FromType& from);

		template<>
		inline System::Drawing::Rectangle marshal_as<System::Drawing::Rectangle, RECT>( const RECT &from )
		{
			return System::Drawing::Rectangle::FromLTRB( from.left, from.top, from.right, from.bottom );
		}

		template<class _To_Type>
		inline _To_Type marshal_as(System::Drawing::Rectangle _from_object);

		template<>
		inline RECT marshal_as<RECT>( System::Drawing::Rectangle from )
		{
			RECT to;
			to.left = from.Left;
			to.right = from.Right;
			to.top = from.Top;
			to.bottom = from.Bottom;

			return to;
		}
	}
}